using UnityEngine;

public class EventTester : MonoBehaviour
{
    public EventLogger logger;

    public void LogRoundStart()
    {
        logger.LogEvent("ROUND_START", 1, "C1", "NA");
    }

    public void LogCard()
    {
        logger.LogEvent("CARD_SELECTED", 1, "C1", "Card_1_2", "pair_3", "1");
    }

    public void LogMatch()
    {
        logger.LogEvent("MATCH", 1, "C1", "pair_3");
    }

    public void LogMismatch()
    {
        logger.LogEvent("MISMATCH", 1, "C1", "Card_1_2|Card_3_1");
    }

    public void LogDistractor()
    {
        logger.LogEvent("DISTRACTOR_ON", 1, "C1", "visual_left", "peripheral");
    }

    public void LogRoundEnd()
    {
        logger.LogEvent("ROUND_END", 1, "C1", "NA");
    }
}