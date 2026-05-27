using UnityEngine;
using System;
using LSL;

public class LSLEventStreamer : MonoBehaviour
{
    public static LSLEventStreamer Instance { get; private set; }

    [Header("LSL Stream Settings")]
    [SerializeField] private string streamName = "DataSyncMarker";
    [SerializeField] private string streamType = "Markers";
    [SerializeField] private string sourceId = "12345";

    private StreamOutlet outlet;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeOutlet();
    }

    private void InitializeOutlet()
    {
        try
        {
            StreamInfo info = new StreamInfo(
                streamName,
                streamType,
                1,
                LSL.LSL.IRREGULAR_RATE,
                channel_format_t.cf_string,
                sourceId
            );

            XMLElement channels = info.desc().append_child("channels");
            channels.append_child("channel")
                .append_child_value("label", "marker")
                .append_child_value("type", "event");

            outlet = new StreamOutlet(info);
            isInitialized = true;

            Debug.Log("[LSL] Outlet initialized: " + streamName);

            SendMarker("UNITY_STREAM_STARTED");
        }
        catch (Exception e)
        {
            isInitialized = false;
            Debug.LogError("[LSL] Failed to initialize outlet: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        outlet?.Dispose();
        outlet = null;
        isInitialized = false;
    }

    public void SendMarker(string marker)
    {
        if (!isInitialized || outlet == null)
        {
            Debug.LogWarning("[LSL] Marker not sent, outlet not initialized: " + marker);
            return;
        }

        try
        {
            string[] sample = { marker };
            outlet.push_sample(sample);
            Debug.Log("[LSL] Marker sent: " + marker);
        }
        catch (Exception e)
        {
            Debug.LogError("[LSL] Failed to send marker: " + e.Message);
        }
    }

    public void SendStructuredMarker(
        string eventType,
        int roundIndex,
        string conditionId,
        string objectId,
        string extra1 = "NA",
        string extra2 = "NA"
    )
    {
        string sessionId = "NO_SESSION";

        if (ExperimentSessionInfo.Instance != null)
        {
            sessionId = ExperimentSessionInfo.Instance.SessionId;
        }

        string extra1Part = extra1 == "NA" ? "" : $"|{extra1}";
        string extra2Part = extra2 == "NA" ? "" : $"|{extra2}";
        string marker = $"{sessionId}|{eventType}|{roundIndex}|{conditionId}|{objectId}{extra1Part}{extra2Part}";

        SendMarker(marker);
    }
}