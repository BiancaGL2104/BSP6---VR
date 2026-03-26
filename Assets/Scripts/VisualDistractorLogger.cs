using UnityEngine;

public class VisualDistractorLogger : MonoBehaviour
{
    private void OnEnable()
    {
        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogDistractorOn(
                gameObject.name,
                "VISUAL"
            );
        }
    }

    private void OnDisable()
    {
        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogDistractorOff(
                gameObject.name,
                "VISUAL"
            );
        }
    }
}