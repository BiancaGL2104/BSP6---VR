using System.Collections.Generic;
using UnityEngine;

public class VisualDistractorManager : MonoBehaviour
{
    [Header("Zone Anchors")]
    public List<Transform> visualZones = new List<Transform>();

    [Header("Distractor Object")]
    public GameObject distractorObject;

    [Header("Logging")]
    public EventLogger eventLogger;
    public ConditionManager conditionManager;
    public int roundIndex = 1;

    public void HideDistractor()
    {
        if (distractorObject != null)
        {
            distractorObject.SetActive(false);

            if (eventLogger != null && conditionManager != null)
            {
                eventLogger.LogEvent(
                    "VISUAL_DISTRACTOR_OFF",
                    roundIndex,
                    conditionManager.GetConditionId(),
                    "NA"
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

        Debug.Log("Visual distractor shown at zone: " + visualZones[zoneIndex].name);

        if (eventLogger != null && conditionManager != null)
        {
            eventLogger.LogEvent(
                "VISUAL_DISTRACTOR_ON",
                roundIndex,
                conditionManager.GetConditionId(),
                visualZones[zoneIndex].name
            );
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowDistractorAtZone(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowDistractorAtZone(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            HideDistractor();
        }
    }

    public void ShowDistractorAtRandomZone()
    {
        if (distractorObject == null) return;
        if (visualZones == null || visualZones.Count == 0) return;

        int randomIndex = Random.Range(0, visualZones.Count);
        ShowDistractorAtZone(randomIndex);

        Debug.Log("Random visual distractor zone selected: " + randomIndex);
    }


}