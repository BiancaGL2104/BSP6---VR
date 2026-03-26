using UnityEngine;

public class EventTester : MonoBehaviour
{
    public void TestExperimentStart()
    {
        ExperimentEventManager.Instance.LogExperimentStart();
    }

    public void TestRoundStart()
    {
        ExperimentEventManager.Instance.LogRoundStart(1, "VISUAL_LEFT");
    }

    public void TestCardFlip()
    {
        ExperimentEventManager.Instance.LogCardFlip("Card_01", "Pair_A", "FIRST");
    }

    public void TestMatch()
    {
        ExperimentEventManager.Instance.LogMatch("Card_01", "Card_02");
    }

    public void TestMismatch()
    {
        ExperimentEventManager.Instance.LogMismatch("Card_01", "Card_07");
    }

    public void TestDistractorOn()
    {
        ExperimentEventManager.Instance.LogDistractorOn("Distractor_Left", "VISUAL");
    }

    public void TestDistractorOff()
    {
        ExperimentEventManager.Instance.LogDistractorOff("Distractor_Left", "VISUAL");
    }

    public void TestRoundEnd()
    {
        ExperimentEventManager.Instance.LogRoundEnd();
    }

    public void TestExperimentEnd()
    {
        ExperimentEventManager.Instance.LogExperimentEnd();
    }
}