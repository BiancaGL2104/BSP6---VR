using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    public enum ConditionType
    {
        C1_NoDistractor,
        C2_VisualPredictable,
        C3_VisualUnpredictable,
        C4_AudioPredictable,
        C5_AudioUnpredictable
        // C6_Multimodal   // optional later
    }

    [Header("Current Condition")]
    public ConditionType currentCondition = ConditionType.C1_NoDistractor;

    public bool UseVisualDistractor()
    {
        return currentCondition == ConditionType.C2_VisualPredictable ||
               currentCondition == ConditionType.C3_VisualUnpredictable;
    }

    public bool UseAudioDistractor()
    {
        return currentCondition == ConditionType.C4_AudioPredictable ||
               currentCondition == ConditionType.C5_AudioUnpredictable;
    }

    public bool IsPredictable()
    {
        return currentCondition == ConditionType.C2_VisualPredictable ||
               currentCondition == ConditionType.C4_AudioPredictable;
    }

    public bool IsUnpredictable()
    {
        return currentCondition == ConditionType.C3_VisualUnpredictable ||
               currentCondition == ConditionType.C5_AudioUnpredictable;
    }

    public string GetConditionId()
    {
        return currentCondition.ToString();
    }
}