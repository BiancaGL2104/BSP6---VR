using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Cards in Scene")]
    public List<MemoryCard> allCards = new List<MemoryCard>();

    [Header("Logging")]
    public EventLogger eventLogger;
    public int roundIndex = 1;
    public string conditionId = "C1";

    [Header("Condition Reference")]
    public ConditionManager conditionManager;

    [Header("Selection")]
    public MemoryCard firstSelected;
    public MemoryCard secondSelected;
    private bool isBusy = false;

    [Header("Round Tracking")]
    public int totalPairs = 8;
    public int matchedPairs = 0;

    [Header("Metrics")]
    public int totalFlips = 0;
    public int mismatchCount = 0;

    [Header("Timing")]
    public float roundStartTime;
    public bool roundActive = false;

    [Header("UI")]
    public TMP_Text statusText;
    public TMP_Text conditionText;

    [Header("Manager References")]
    public VisualDistractorManager visualDistractorManager;

    private void Start()
    {
        SetStatus("Waiting for round start");

        if (visualDistractorManager != null)
        {
            visualDistractorManager.HideDistractor();
        }

        if (conditionText != null)
        {
            conditionText.text = "Condition: Waiting";
        }
    }

    public void StartRound()
    {
        matchedPairs = 0;
        totalFlips = 0;
        mismatchCount = 0;

        firstSelected = null;
        secondSelected = null;
        isBusy = false;

        AssignShuffledCardIds();

        roundStartTime = Time.time;
        roundActive = true;
        SetStatus("Round active");

        if (conditionManager != null)
        {
            conditionId = conditionManager.GetConditionId();
        }
        else
        {
            conditionId = "NO_CONDITION_MANAGER";
        }

        UpdateConditionText();

        Debug.Log("ROUND START");

        if (eventLogger != null)
        {
            eventLogger.LogEvent(
                "ROUND_START",
                roundIndex,
                conditionId,
                "NA"
            );
        }

        if (visualDistractorManager != null && conditionManager != null)
        {
            Debug.Log("Condition is: " + conditionManager.GetConditionId());

            if (conditionManager.currentCondition == ConditionManager.ConditionType.C2_VisualPredictable)
            {
                Debug.Log("Showing predictable visual distractor at zone 0");
                visualDistractorManager.ShowDistractorAtZone(0);
            }
            else if (conditionManager.currentCondition == ConditionManager.ConditionType.C3_VisualUnpredictable)
            {
                Debug.Log("Showing unpredictable visual distractor at random zone");
                visualDistractorManager.ShowDistractorAtRandomZone();
            }
            else
            {
                Debug.Log("No visual distractor for this condition");
                visualDistractorManager.HideDistractor();
            }
        }
        else
        {
            Debug.Log("Missing visualDistractorManager or conditionManager reference");
        }
    }

    private void SetStatus(string message)
    {
        Debug.Log("STATUS: " + message);

        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void RestartRound()
    {
        Debug.Log("RESTART BUTTON PRESSED");
        StartRound();
    }

    private void AssignShuffledCardIds()
    {
        List<int> ids = new List<int>();

        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        for (int i = 0; i < ids.Count; i++)
        {
            int randomIndex = Random.Range(i, ids.Count);
            int temp = ids[i];
            ids[i] = ids[randomIndex];
            ids[randomIndex] = temp;
        }

        for (int i = 0; i < allCards.Count; i++)
        {
            allCards[i].cardId = ids[i];
            allCards[i].ResetCard();

            Debug.Log(allCards[i].name + " assigned ID: " + allCards[i].cardId);
        }
    }

    public void SelectCard(MemoryCard card)
    {
        if (!roundActive) return;
        if (isBusy) return;
        if (card == null) return;
        if (card.isFlipped || card.isMatched) return;

        card.Flip();
        totalFlips++;

        Debug.Log("Total Flips: " + totalFlips);

        if (eventLogger != null)
        {
            eventLogger.LogEvent(
                "CARD_SELECTED",
                roundIndex,
                conditionId,
                card.name,
                "pair_" + card.cardId,
                totalFlips.ToString()
            );
        }

        if (firstSelected == null)
        {
            firstSelected = card;
            Debug.Log("First card selected: " + card.name + " ID=" + card.cardId);
            return;
        }

        if (secondSelected == null)
        {
            secondSelected = card;
            Debug.Log("Second card selected: " + card.name + " ID=" + card.cardId);
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isBusy = true;

        yield return new WaitForSeconds(1f);

        if (firstSelected.cardId == secondSelected.cardId)
        {
            firstSelected.SetMatched();
            secondSelected.SetMatched();
            matchedPairs++;

            Debug.Log("MATCH");
            SetStatus("Match");
            Debug.Log("Matched Pairs: " + matchedPairs + " / " + totalPairs);

            if (eventLogger != null)
            {
                eventLogger.LogEvent(
                    "MATCH",
                    roundIndex,
                    conditionId,
                    "pair_" + firstSelected.cardId
                );
            }
        }
        else
        {
            string mismatchObjectIds = firstSelected.name + "|" + secondSelected.name;

            firstSelected.Flip();
            secondSelected.Flip();
            mismatchCount++;

            Debug.Log("MISMATCH");
            SetStatus("Mismatch");
            Debug.Log("Mismatch Count: " + mismatchCount);

            if (eventLogger != null)
            {
                eventLogger.LogEvent(
                    "MISMATCH",
                    roundIndex,
                    conditionId,
                    mismatchObjectIds
                );
            }
        }

        if (matchedPairs >= totalPairs)
        {
            CompleteRound();
        }

        firstSelected = null;
        secondSelected = null;
        isBusy = false;
    }

    private void CompleteRound()
    {
        roundActive = false;
        float roundTime = Time.time - roundStartTime;

        Debug.Log("=== ROUND COMPLETE ===");
        SetStatus("Round complete");
        Debug.Log("Round Time: " + roundTime.ToString("F2") + " seconds");
        Debug.Log("Final Flips: " + totalFlips);
        Debug.Log("Final Mismatches: " + mismatchCount);

        if (eventLogger != null)
        {
            eventLogger.LogEvent(
                "ROUND_END",
                roundIndex,
                conditionId,
                "NA",
                roundTime.ToString("F2"),
                mismatchCount.ToString()
            );
        }

        if (visualDistractorManager != null)
        {
            visualDistractorManager.HideDistractor();
        }

        conditionId = "NONE";
        UpdateConditionText();
    }

    private void UpdateConditionText()
    {
        if (conditionText != null)
        {
            conditionText.text = "Condition: " + conditionId;
        }
    }

    public void EndRoundForDebug()
    {
        Debug.Log("DEBUG ROUND END BUTTON PRESSED");
        CompleteRound();
    }
}