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

    public void HideDistractor()
    {
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
        ShowDistractorAtZone(randomIndex);

        Debug.Log("Random visual distractor zone selected: " + randomIndex);
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
}