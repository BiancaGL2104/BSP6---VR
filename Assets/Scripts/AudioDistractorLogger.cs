using UnityEngine;

public class AudioDistractorLogger : MonoBehaviour
{
    private void OnEnable()
    {
        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogDistractorOn(
                gameObject.name,
                "AUDIO"
            );
        }
    }

    private void OnDisable()
    {
        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogDistractorOff(
                gameObject.name,
                "AUDIO"
            );
        }
    }
}