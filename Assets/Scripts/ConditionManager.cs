using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    public enum VisualCondition
    {
        Off,
        Predictable,
        Unpredictable
    }

    public enum AudioCondition
    {
        Off,
        Predictable,
        Unpredictable
    }

    [Header("Current Condition")]
    public VisualCondition currentVisualCondition = VisualCondition.Off;
    public AudioCondition currentAudioCondition = AudioCondition.Off;

    public void SetCondition(VisualCondition visual, AudioCondition audio)
    {
        currentVisualCondition = visual;
        currentAudioCondition = audio;

        Debug.Log("Condition set to: " + GetConditionId());
    }

    public bool UseVisualDistractor()
    {
        return currentVisualCondition != VisualCondition.Off;
    }

    public bool UseAudioDistractor()
    {
        return currentAudioCondition != AudioCondition.Off;
    }

    public bool IsVisualPredictable()
    {
        return currentVisualCondition == VisualCondition.Predictable;
    }

    public bool IsVisualUnpredictable()
    {
        return currentVisualCondition == VisualCondition.Unpredictable;
    }

    public bool IsAudioPredictable()
    {
        return currentAudioCondition == AudioCondition.Predictable;
    }

    public bool IsAudioUnpredictable()
    {
        return currentAudioCondition == AudioCondition.Unpredictable;
    }

    public string GetConditionId()
    {
        return GetCombinedConditionId(currentVisualCondition, currentAudioCondition);
    }

    public static string GetCombinedConditionId(VisualCondition visual, AudioCondition audio)
    {
        int conditionNumber = GetConditionNumber(visual, audio);
        return $"C{conditionNumber}_{GetVisualLabel(visual)}_{GetAudioLabel(audio)}";
    }

    private static int GetConditionNumber(VisualCondition visual, AudioCondition audio)
    {
        if (visual == VisualCondition.Off && audio == AudioCondition.Off) return 1;
        if (visual == VisualCondition.Predictable && audio == AudioCondition.Off) return 2;
        if (visual == VisualCondition.Unpredictable && audio == AudioCondition.Off) return 3;

        if (visual == VisualCondition.Off && audio == AudioCondition.Predictable) return 4;
        if (visual == VisualCondition.Predictable && audio == AudioCondition.Predictable) return 5;
        if (visual == VisualCondition.Unpredictable && audio == AudioCondition.Predictable) return 6;

        if (visual == VisualCondition.Off && audio == AudioCondition.Unpredictable) return 7;
        if (visual == VisualCondition.Predictable && audio == AudioCondition.Unpredictable) return 8;
        if (visual == VisualCondition.Unpredictable && audio == AudioCondition.Unpredictable) return 9;

        return 0;
    }

    private static string GetVisualLabel(VisualCondition visual)
    {
        switch (visual)
        {
            case VisualCondition.Off: return "VisualOff";
            case VisualCondition.Predictable: return "VisualPredictable";
            case VisualCondition.Unpredictable: return "VisualUnpredictable";
            default: return "VisualUnknown";
        }
    }

    private static string GetAudioLabel(AudioCondition audio)
    {
        switch (audio)
        {
            case AudioCondition.Off: return "AudioOff";
            case AudioCondition.Predictable: return "AudioPredictable";
            case AudioCondition.Unpredictable: return "AudioUnpredictable";
            default: return "AudioUnknown";
        }
    }
}