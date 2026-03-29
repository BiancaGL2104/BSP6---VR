using UnityEngine;

public class ExperimentEventManager : MonoBehaviour
{
    public static ExperimentEventManager Instance { get; private set; }

    [SerializeField] private EventLogger eventLogger;
    [SerializeField] private LSLEventStreamer lslEventStreamer;

    private int currentRoundIndex = -1;
    private string currentConditionId = "NA";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (eventLogger == null)
        {
            eventLogger = GetComponent<EventLogger>();
        }

        if (lslEventStreamer == null)
        {
            lslEventStreamer = FindFirstObjectByType<LSLEventStreamer>();
        }
    }

    private void LogToAllOutputs(
        string eventType,
        int roundIndex,
        string conditionId,
        string objectId,
        string extra1 = "NA",
        string extra2 = "NA"
    )
    {
        if (eventLogger != null)
        {
            eventLogger.LogEvent(
                eventType: eventType,
                roundIndex: roundIndex,
                conditionId: conditionId,
                objectId: objectId,
                extra1: extra1,
                extra2: extra2
            );
        }
        else
        {
            Debug.LogWarning("[ExperimentEventManager] EventLogger reference missing.");
        }

        if (lslEventStreamer != null)
        {
            lslEventStreamer.SendStructuredMarker(
                eventType: eventType,
                roundIndex: roundIndex,
                conditionId: conditionId,
                objectId: objectId,
                extra1: extra1,
                extra2: extra2
            );
        }
        else
        {
            Debug.LogWarning("[ExperimentEventManager] LSLEventStreamer reference missing.");
        }
    }

    public void LogExperimentStart()
    {
        LogToAllOutputs("EXPERIMENT_START", -1, "NA", "SYSTEM");
    }

    public void LogExperimentEnd()
    {
        LogToAllOutputs("EXPERIMENT_END", currentRoundIndex, currentConditionId, "SYSTEM");
    }

    public void LogRoundStart(int roundIndex, string conditionId)
    {
        currentRoundIndex = roundIndex;
        currentConditionId = conditionId;

        LogToAllOutputs("ROUND_START", currentRoundIndex, currentConditionId, "ROUND");
    }

    public void LogRoundEnd(string roundTime = "NA", string mismatchCount = "NA")
    {
        LogToAllOutputs(
            "ROUND_END",
            currentRoundIndex,
            currentConditionId,
            "ROUND",
            roundTime,
            mismatchCount
        );
    }

    public void LogCardFlip(string cardId, string pairId, string selectionOrder)
    {
        LogToAllOutputs(
            "CARD_FLIP",
            currentRoundIndex,
            currentConditionId,
            cardId,
            pairId,
            selectionOrder
        );
    }

    public void LogMatch(string firstCardId, string secondCardId)
    {
        LogToAllOutputs(
            "MATCH",
            currentRoundIndex,
            currentConditionId,
            firstCardId,
            secondCardId
        );
    }

    public void LogMismatch(string firstCardId, string secondCardId)
    {
        LogToAllOutputs(
            "MISMATCH",
            currentRoundIndex,
            currentConditionId,
            firstCardId,
            secondCardId
        );
    }

    public void LogDistractorOn(string distractorId, string distractorType)
    {
        LogToAllOutputs(
            "DISTRACTOR_ON",
            currentRoundIndex,
            currentConditionId,
            distractorId,
            distractorType
        );
    }

    public void LogDistractorOff(string distractorId, string distractorType)
    {
        LogToAllOutputs(
            "DISTRACTOR_OFF",
            currentRoundIndex,
            currentConditionId,
            distractorId,
            distractorType
        );
    }

    public void LogQuestionnaireStart(string questionnaireId)
    {
        LogToAllOutputs(
            "QUESTIONNAIRE_START",
            currentRoundIndex,
            currentConditionId,
            questionnaireId
        );
    }

    public void LogQuestionnaireEnd(string questionnaireId)
    {
        LogToAllOutputs(
            "QUESTIONNAIRE_END",
            currentRoundIndex,
            currentConditionId,
            questionnaireId
        );
    }
}