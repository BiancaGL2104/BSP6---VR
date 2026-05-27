using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using ViveSR.anipal.Eye;

public class SRanipalRecord : MonoBehaviour
{
    private static bool eye_callback_registered = false;
    private static SRanipal_Eye.CallbackBasic _eyeCallbackDelegate = EyeCallback;
    private static IntPtr eyeCallbackPtr = IntPtr.Zero;

    public bool isRecording = false;
    private Transform _cam;

    public string pathPrefix = "C:\\Users\\johannes\\Desktop\\";
    private readonly string logFilePath = "eyetracking-raw.csv";
    private string strFilePath = @"C:\Users\johannes\Desktop\eyetracking-raw.csv";

    // Pre-allocate 30MB in RAM - just in case
    private static StringBuilder sbOutput = new StringBuilder(30_000_000);
    private static int recordingNumber = 0;

    // Raycast requirements
    private Vector3 _worldPosition;
    private Vector3 _worldDirection;
    private int _interestedLayer;
    private string _currentCollider = "";
    private Vector3 _currentLocalPosition = Vector3.zero;

    public struct PoseSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public string colliderName;
        public Vector3 localHitPosition;
    }

    private static PoseSnapshot _currentPose;
    private static readonly object _poseLock = new object();

    private void Start()
    {
        _cam = Camera.main.transform;
        _interestedLayer = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        if (Keyboard.current.f12Key.wasPressedThisFrame) 
        {
            if (isRecording)
            {
                StopRecording();
            }
            else 
            {
                StartRecord();
            }
        }

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

        // Create raycast based on eye direction
        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out Ray gazeRay))
        {
            _worldPosition = _cam.TransformPoint(gazeRay.origin);
            _worldDirection = _cam.TransformDirection(gazeRay.direction);

            // Debug.DrawLine(_worldPosition, _worldPosition + _worldDirection * 10, Color.red, 5.0f);

            if (Physics.Raycast(_worldPosition, _worldDirection, out RaycastHit hit, 50f, _interestedLayer))
            {
                _currentCollider = hit.collider.name;
                _currentLocalPosition = hit.point;
            }
            else
            {
                _currentCollider = "x";
            }
        }

        // Create a new update pose
        PoseSnapshot updatePose = new PoseSnapshot
        {
            position = _cam.position,
            rotation = _cam.rotation,
            colliderName = _currentCollider,
            localHitPosition = _currentLocalPosition
        };

        lock (_poseLock)
        {
            _currentPose = updatePose;
        }

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && !eye_callback_registered && isRecording)
        {
            // Register the callback
            eyeCallbackPtr = Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback);
            SRanipal_Eye.WrapperRegisterEyeDataCallback(eyeCallbackPtr);
            eye_callback_registered = true;
        }
        else if (!SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && eye_callback_registered && !isRecording)
        {
            // Unregister the callback
            if (eyeCallbackPtr != IntPtr.Zero)
            {
                SRanipal_Eye.WrapperUnRegisterEyeDataCallback(eyeCallbackPtr);
                eyeCallbackPtr = IntPtr.Zero;
            }
            eye_callback_registered = false;
        }

    }
    public static PoseSnapshot GetLatestPose()
    {
        lock (_poseLock)
        {
            return _currentPose;
        }
    }

    public void StartRecord()
    {
        if (!isRecording)
        {
            // Set file name based on settings
            if (recordingNumber == 0)
            {
                strFilePath = pathPrefix + logFilePath;
            }
            Debug.Log($"Starting the recording of Recording: {recordingNumber}");

            // initialize file if neccessary
            if (!File.Exists(strFilePath))
            {
                string header = "ticks,recording," + // Time domain
                    "pose_x,pose_y,pose_z,pose_rot_w,pose_rot_x,pose_rot_y,pose_rot_z," + // Camera Transform Domain
                    // "pose_rot_w,pose_rot_x,pose_rot_y,pose_rot_z," + // Camera Transform Domain
                    "collider_name,world_hit_x,world_hit_y,world_hit_z," + // Collider Domain
                    "L_pupil_diamm,L_eye_openness,L_eyepos_x,L_eyepos_y,L_eyepos_z,L_eyerot_x,L_eyerot_y,L_eyerot_z,L_pupilpos_x,L_pupilpos_y," + // Left Eye Domain
                    "R_pupil_diamm,R_eye_openness,R_eyepos_x,R_eyepos_y,R_eyepos_z,R_eyerot_x,R_eyerot_y,R_eyerot_z,R_pupilpos_x,R_pupilpos_y\n"; // Right Eye Domain
                File.AppendAllText(strFilePath, header);
            }

            recordingNumber++;
            isRecording = true;
            sbOutput.Clear();
        }
        else
        {
            Debug.LogWarning("Tried to start recording again");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Debug.Log($"Stopping the recording of Recording: {recordingNumber - 1}");
            isRecording = false;
            File.AppendAllText(strFilePath, sbOutput.ToString());
        }
        else
        {
            Debug.LogWarning("Tried to stop recording while it is already stopped");
        }
    }

    private void OnDisable()
    {
        if (isRecording)
        {
            StopRecording();
        }
        Release();
    }

    void OnApplicationQuit()
    {
        if (isRecording) 
        {
            StopRecording();
        }
        Release();
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    /// <summary>
    /// Required class for IL2CPP scripting backend support
    /// </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData eye_data)
    {
        // keep the stringbuilder as efficient as possible -> No additional allocations
        // Timing
        sbOutput.Append(DateTime.Now.Ticks);
        sbOutput.Append(",");
        sbOutput.Append(recordingNumber);
        sbOutput.Append(",");

        // Camera Location
        PoseSnapshot pose = GetLatestPose();
        sbOutput.Append(pose.position.x);
        sbOutput.Append(",");
        sbOutput.Append(pose.position.y);
        sbOutput.Append(",");
        sbOutput.Append(pose.position.z);
        sbOutput.Append(",");
        sbOutput.Append(pose.rotation.w);
        sbOutput.Append(",");
        sbOutput.Append(pose.rotation.x);
        sbOutput.Append(",");
        sbOutput.Append(pose.rotation.y);
        sbOutput.Append(",");
        sbOutput.Append(pose.rotation.z);
        sbOutput.Append(",");

        // Colliders Info
        sbOutput.Append(pose.colliderName);
        sbOutput.Append(",");
        sbOutput.Append(pose.localHitPosition.x);
        sbOutput.Append(",");
        sbOutput.Append(pose.localHitPosition.y);
        sbOutput.Append(",");
        sbOutput.Append(pose.localHitPosition.z);
        sbOutput.Append(",");

        // left
        sbOutput.Append(eye_data.verbose_data.left.pupil_diameter_mm);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.eye_openness);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.gaze_origin_mm.x);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.gaze_origin_mm.y);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.gaze_origin_mm.z);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.gaze_direction_normalized.x);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.gaze_direction_normalized.y);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.gaze_direction_normalized.z);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.pupil_position_in_sensor_area.x);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.left.pupil_position_in_sensor_area.y);
        sbOutput.Append(",");

        // Right
        sbOutput.Append(eye_data.verbose_data.right.pupil_diameter_mm);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.eye_openness);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.gaze_origin_mm.x);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.gaze_origin_mm.y);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.gaze_origin_mm.z);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.gaze_direction_normalized.x);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.gaze_direction_normalized.y);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.gaze_direction_normalized.z);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.pupil_position_in_sensor_area.x);
        sbOutput.Append(",");
        sbOutput.Append(eye_data.verbose_data.right.pupil_position_in_sensor_area.y);
        sbOutput.Append("\n");
    }
}
