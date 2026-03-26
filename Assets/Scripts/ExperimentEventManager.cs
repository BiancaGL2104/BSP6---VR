using UnityEngine;

public class ExperimentEventManager : MonoBehaviour
{
    public static ExperimentEventManager Instance { get; private set; }

    [SerializeField] private EventLogger eventLogger;

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
    }

    public void LogExperimentStart()
    {
        eventLogger.LogEvent(
            eventType: "EXPERIMENT_START",
            roundIndex: -1,
            conditionId: "NA",
            objectId: "SYSTEM"
        );
    }

    public void LogExperimentEnd()
    {
        eventLogger.LogEvent(
            eventType: "EXPERIMENT_END",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: "SYSTEM"
        );
    }

    public void LogRoundStart(int roundIndex, string conditionId)
    {
        currentRoundIndex = roundIndex;
        currentConditionId = conditionId;

        eventLogger.LogEvent(
            eventType: "ROUND_START",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: "ROUND"
        );
    }

    public void LogRoundEnd(string roundTime = "NA", string mismatchCount = "NA")
    {
        eventLogger.LogEvent(
            eventType: "ROUND_END",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: "ROUND",
            extra1: roundTime,
            extra2: mismatchCount
        );
    }

    public void LogCardFlip(string cardId, string pairId, string selectionOrder)
    {
        eventLogger.LogEvent(
            eventType: "CARD_FLIP",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: cardId,
            extra1: pairId,
            extra2: selectionOrder
        );
    }

    public void LogMatch(string firstCardId, string secondCardId)
    {
        eventLogger.LogEvent(
            eventType: "MATCH",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: firstCardId,
            extra1: secondCardId
        );
    }

    public void LogMismatch(string firstCardId, string secondCardId)
    {
        eventLogger.LogEvent(
            eventType: "MISMATCH",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: firstCardId,
            extra1: secondCardId
        );
    }

    public void LogDistractorOn(string distractorId, string distractorType)
    {
        eventLogger.LogEvent(
            eventType: "DISTRACTOR_ON",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: distractorId,
            extra1: distractorType
        );
    }

    public void LogDistractorOff(string distractorId, string distractorType)
    {
        eventLogger.LogEvent(
            eventType: "DISTRACTOR_OFF",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: distractorId,
            extra1: distractorType
        );
    }

    public void LogQuestionnaireStart(string questionnaireId)
    {
        eventLogger.LogEvent(
            eventType: "QUESTIONNAIRE_START",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: questionnaireId
        );
    }

    public void LogQuestionnaireEnd(string questionnaireId)
    {
        eventLogger.LogEvent(
            eventType: "QUESTIONNAIRE_END",
            roundIndex: currentRoundIndex,
            conditionId: currentConditionId,
            objectId: questionnaireId
        );
    }
}