using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Cards in Scene")]
    public List<MemoryCard> allCards = new List<MemoryCard>();

    [Header("Logging")]
    public int roundIndex = 1;
    public string conditionId = "C1";

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

    [Header("Distractors")]
    public GameObject visualDistractor;

    public enum ConditionType
    {
        C1_NoDistractor,
        C2_VisualDistractor
    }

    [Header("Condition Settings")]
    public ConditionType currentCondition = ConditionType.C1_NoDistractor;

    private void Start()
    {
        SetStatus("Waiting for round start");

        if (visualDistractor != null)
        {
            visualDistractor.SetActive(false);
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

        roundStartTime = Time.realtimeSinceStartup;
        roundActive = true;
        SetStatus("Round active");
        conditionId = currentCondition.ToString();

        Debug.Log("ROUND START");

        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogRoundStart(roundIndex, conditionId);
        }

        if (visualDistractor != null)
        {
            bool shouldShow = (currentCondition == ConditionType.C2_VisualDistractor);
            visualDistractor.SetActive(shouldShow);
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

        if (ExperimentEventManager.Instance != null)
        {
            string selectionOrder = (firstSelected == null) ? "FIRST" : "SECOND";
            ExperimentEventManager.Instance.LogCardFlip(
                card.name,
                "pair_" + card.cardId,
                selectionOrder
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

            if (ExperimentEventManager.Instance != null)
            {
                ExperimentEventManager.Instance.LogMatch(
                    firstSelected.name,
                    secondSelected.name
                );
            }
        }
        else
        {
            firstSelected.Flip();
            secondSelected.Flip();
            mismatchCount++;

            Debug.Log("MISMATCH");
            SetStatus("Mismatch");
            Debug.Log("Mismatch Count: " + mismatchCount);

            if (ExperimentEventManager.Instance != null)
            {
                ExperimentEventManager.Instance.LogMismatch(
                    firstSelected.name,
                    secondSelected.name
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
        float roundTime = Time.realtimeSinceStartup - roundStartTime;

        Debug.Log("=== ROUND COMPLETE ===");
        SetStatus("Round complete");
        Debug.Log("Round Time: " + roundTime.ToString("F2") + " seconds");
        Debug.Log("Final Flips: " + totalFlips);
        Debug.Log("Final Mismatches: " + mismatchCount);

        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogRoundEnd(
                roundTime.ToString("F2"),
                mismatchCount.ToString()
            );
        }

        if (visualDistractor != null)
        {
            visualDistractor.SetActive(false);
        }
    }
}