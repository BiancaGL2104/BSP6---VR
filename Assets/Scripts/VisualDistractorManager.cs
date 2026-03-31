using System.Collections.Generic;
using UnityEngine;

public class VisualDistractorManager : MonoBehaviour
{
    [Header("Zone Anchors")]
    public List<Transform> visualZones = new List<Transform>();

    [Header("Distractor Object")]
    public GameObject distractorObject;

    [Header("Condition Reference")]
    public ConditionManager conditionManager;

    [Header("Predictable Movement")]
    public float predictableAmplitude = 3f;
    public float predictableSpeed = 2f;

    [Header("Unpredictable Movement")]
    public float unpredictableSpeed = 2f;
    public float targetReachThreshold = 0.3f;
    public Vector3 randomMoveArea = new Vector3(3f, 1.5f, 3f);

    [Header("Movement Bounds")]
    public BoxCollider movementBounds;

    private enum VisualMovementMode
    {
        None,
        Predictable,
        Unpredictable
    }

    private VisualMovementMode currentMode = VisualMovementMode.None;

    private Vector3 predictableBasePosition;
    private Vector3 unpredictableTargetPosition;

    private void Update()
    {
        if (distractorObject == null || !distractorObject.activeSelf)
            return;

        if (currentMode == VisualMovementMode.Predictable)
        {
            Vector3 pos = predictableBasePosition;
            pos.y += Mathf.Sin(Time.time * predictableSpeed) * predictableAmplitude;
            distractorObject.transform.position = pos;
        }
        else if (currentMode == VisualMovementMode.Unpredictable)
        {
            Vector3 direction = (unpredictableTargetPosition - distractorObject.transform.position).normalized;
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.2f, 0.2f),
                Random.Range(-0.3f, 0.3f)
            );

            Vector3 movement = (direction + randomOffset) * unpredictableSpeed * Time.deltaTime;

            distractorObject.transform.position += movement;

            if (Vector3.Distance(distractorObject.transform.position, unpredictableTargetPosition) < targetReachThreshold)
            {
                PickNewRandomTarget();
            }
        }
    }

    public void HideDistractor()
    {
        currentMode = VisualMovementMode.None;

        if (distractorObject != null)
        {
            distractorObject.SetActive(false);

            if (ExperimentEventManager.Instance != null)
            {
                ExperimentEventManager.Instance.LogDistractorOff(
                    "VisualDistractor",
                    "VISUAL"
                );
            }
        }
    }

    public void ShowDistractorAtZone(int zoneIndex)
    {
        if (distractorObject == null) return;
        if (visualZones == null || visualZones.Count == 0) return;
        if (zoneIndex < 0 || zoneIndex >= visualZones.Count) return;

        distractorObject.transform.position = visualZones[zoneIndex].position;
        distractorObject.transform.rotation = visualZones[zoneIndex].rotation;
        distractorObject.SetActive(true);

        string zoneName = visualZones[zoneIndex].name;
        Debug.Log("Visual distractor shown at zone: " + zoneName);

        if (conditionManager != null && conditionManager.IsVisualPredictable())
        {
            predictableBasePosition = visualZones[zoneIndex].position;
            currentMode = VisualMovementMode.Predictable;
        }
        else
        {
            currentMode = VisualMovementMode.None;
        }

        if (ExperimentEventManager.Instance != null)
        {
            string distractorType = "VISUAL";

            if (conditionManager != null)
            {
                if (conditionManager.IsVisualPredictable())
                {
                    distractorType = "VISUAL_PREDICTABLE";
                }
                else if (conditionManager.IsVisualUnpredictable())
                {
                    distractorType = "VISUAL_UNPREDICTABLE";
                }
                else
                {
                    distractorType = "VISUAL_OFF";
                }
            }

            ExperimentEventManager.Instance.LogDistractorOn(
                zoneName,
                distractorType
            );
        }
    }

    public void ShowDistractorAtRandomZone()
    {
        if (distractorObject == null) return;
        if (visualZones == null || visualZones.Count == 0) return;

        int randomIndex = Random.Range(0, visualZones.Count);

        distractorObject.transform.position = visualZones[randomIndex].position;
        distractorObject.transform.rotation = visualZones[randomIndex].rotation;
        distractorObject.SetActive(true);

        currentMode = VisualMovementMode.Unpredictable;
        PickNewRandomTarget();

        string zoneName = visualZones[randomIndex].name;
        Debug.Log("Random visual distractor started from zone: " + zoneName);

        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogDistractorOn(
                zoneName,
                "VISUAL_UNPREDICTABLE"
            );
        }
    }

    private void PickNewRandomTarget()
    {
        if (movementBounds == null) return;

        Bounds bounds = movementBounds.bounds;

        unpredictableTargetPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}