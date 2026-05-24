using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SessionManager : MonoBehaviour
{
    [System.Serializable]
    public class SessionCondition
    {
        public ConditionManager.VisualCondition visualCondition;
        public ConditionManager.AudioCondition audioCondition;
    }

    [Header("Session Setup")]
    public List<SessionCondition> sessionConditions = new List<SessionCondition>();

    [Header("Questionnaire")]
    public QuestionnaireManager questionnaireManager;

    [Header("References")]
    public ConditionManager conditionManager;
    public MemoryGameManager memoryGameManager;
    public VisualDistractorManager visualDistractorManager;
    public AudioDistractorManager audioDistractorManager;
    public ExperimentStartScreen experimentStartScreen;

    [Header("UI")]
    public TMP_Text sessionStatusText;

    [Header("Timing")]
    public int predictableVisualZoneIndex = 0;
    public int predictableAudioZoneIndex = 0;

    private int currentConditionIndex = -1;
    private Coroutine visualLoopCoroutine;
    private Coroutine audioLoopCoroutine;
    private bool sessionActive = false;

    private void Start()
    {
        Debug.Log("SessionManager Start on: " + gameObject.name 
                + " conditions count: " + sessionConditions.Count);

        if (sessionConditions.Count == 0)
        {
            BuildDefaultNineConditions();
            Debug.Log("After build, conditions count: " + sessionConditions.Count);
        }

        SetSessionStatus("Waiting for session start");
    }

    public void StartSession()
    {
        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogExperimentStart();
        }
        else
        {
            Debug.LogWarning("ExperimentEventManager.Instance is NULL at session start");
        }

        if (sessionConditions.Count == 0)
        {
            Debug.Log("Session conditions empty, building default nine conditions now.");
            BuildDefaultNineConditions();
        }

        if (sessionConditions.Count == 0)
        {
            Debug.LogWarning("No session conditions defined.");
            return;
        }

        sessionActive = true;
        currentConditionIndex = -1;

        SetSessionStatus("Session started");
        StartNextCondition();
    }

    public void StartNextCondition()
    {
        StopAllDistractorsAndLoops();

        currentConditionIndex++;

        if (currentConditionIndex >= sessionConditions.Count)
        {
            CompleteSession();
            return;
        }

        SessionCondition condition = sessionConditions[currentConditionIndex];

        conditionManager.SetCondition(condition.visualCondition, condition.audioCondition);

        Debug.Log("=== READY FOR SESSION CONDITION ===");
        Debug.Log("Index: " + currentConditionIndex);
        Debug.Log("Condition ID: " + conditionManager.GetConditionId());

        SetSessionStatus("Ready for " + conditionManager.GetConditionId());

        if (currentConditionIndex == 0)
        {
            ConfirmConditionStart();
            return;
        }

        if (experimentStartScreen != null)
        {
            experimentStartScreen.ShowConditionStartScreen(
                currentConditionIndex + 1,
                conditionManager.GetConditionId()
            );
        }
        else
        {
            Debug.LogWarning("ExperimentStartScreen is not assigned. Starting condition immediately.");
            ConfirmConditionStart();
        }
    }

    public void ConfirmConditionStart()
    {
        if (!sessionActive)
            return;

        Debug.Log("=== STARTING SESSION CONDITION ===");
        Debug.Log("Index: " + currentConditionIndex);
        Debug.Log("Condition ID: " + conditionManager.GetConditionId());

        SetSessionStatus("Running " + conditionManager.GetConditionId());

        StartDistractorsForCurrentCondition();

        if (memoryGameManager != null)
        {
            memoryGameManager.roundIndex = currentConditionIndex + 1;
            memoryGameManager.StartRound();
            StartDistractorsForCurrentCondition();
        }
    }

    public void OnConditionCompleted()
    {
        if (!sessionActive)
            return;

        Debug.Log("Condition complete: " + conditionManager.GetConditionId());

        StopAllDistractorsAndLoops();

        if (questionnaireManager != null)
        {
            SetSessionStatus("Questionnaire active");
            questionnaireManager.BeginQuestionnaire(this,memoryGameManager.roundIndex,conditionManager.GetConditionId());
        }
        else
        {
            StartNextCondition();
        }
    }

    public void OnQuestionnaireSubmitted()
    {
        if (!sessionActive)
            return;

        Debug.Log("Questionnaire submitted. Continuing session.");
        StartNextCondition();
    }

    private void StartDistractorsForCurrentCondition()
    {
        if (conditionManager == null) return;

        if (visualDistractorManager != null)
        {
            if (conditionManager.IsVisualPredictable())
            {
                visualDistractorManager.ShowDistractorAtZone(0);
            }
            else if (conditionManager.IsVisualUnpredictable())
            {
                visualLoopCoroutine = StartCoroutine(HandleUnpredictableVisualLoop());
            }
            else
            {
                visualDistractorManager.HideDistractor();
            }
        }

        if (audioDistractorManager != null)
        {
            if (conditionManager.IsAudioPredictable())
            {
                audioDistractorManager.PlayPredictableAtZone(0);
            }
            else if (conditionManager.IsAudioUnpredictable())
            {
                audioLoopCoroutine = StartCoroutine(HandleUnpredictableAudioLoop());
            }
            else
            {
                audioDistractorManager.StopAudioDistractor();
            }
        }
    }

    private IEnumerator HandleUnpredictableVisualLoop()
    {
        while (sessionActive && conditionManager != null && conditionManager.IsVisualUnpredictable())
        {
            visualDistractorManager.ShowDistractorAtRandomZone();
            yield return new WaitForSeconds(12f);

            visualDistractorManager.HideDistractor();
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator HandleUnpredictableAudioLoop()
    {
        while (sessionActive && conditionManager != null && conditionManager.IsAudioUnpredictable())
        {
            audioDistractorManager.PlayUnpredictableOrbitAudio();
            yield return new WaitForSeconds(12f);

            audioDistractorManager.StopAudioDistractor();
            yield return new WaitForSeconds(2f);
        }
    }

    private void StopAllDistractorsAndLoops()
    {
        if (visualLoopCoroutine != null)
        {
            StopCoroutine(visualLoopCoroutine);
            visualLoopCoroutine = null;
        }

        if (audioLoopCoroutine != null)
        {
            StopCoroutine(audioLoopCoroutine);
            audioLoopCoroutine = null;
        }

        if (visualDistractorManager != null)
        {
            visualDistractorManager.HideDistractor();
        }

        if (audioDistractorManager != null)
        {
            audioDistractorManager.StopAudioDistractor();
        }
    }

    private void CompleteSession()
    {
        sessionActive = false;
        StopAllDistractorsAndLoops();
        SetSessionStatus("Session complete");

        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogExperimentEnd();
        }

        Debug.Log("=== SESSION COMPLETE ===");
    }

    private void SetSessionStatus(string message)
    {
        Debug.Log("SESSION STATUS: " + message);

        if (sessionStatusText != null)
        {
            sessionStatusText.text = message;
        }
    }

    private void BuildDefaultNineConditions()
    {
        sessionConditions.Clear();

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Off,
            audioCondition = ConditionManager.AudioCondition.Off
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Predictable,
            audioCondition = ConditionManager.AudioCondition.Off
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Unpredictable,
            audioCondition = ConditionManager.AudioCondition.Off
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Off,
            audioCondition = ConditionManager.AudioCondition.Predictable
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Predictable,
            audioCondition = ConditionManager.AudioCondition.Predictable
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Unpredictable,
            audioCondition = ConditionManager.AudioCondition.Predictable
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Off,
            audioCondition = ConditionManager.AudioCondition.Unpredictable
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Predictable,
            audioCondition = ConditionManager.AudioCondition.Unpredictable
        });

        sessionConditions.Add(new SessionCondition
        {
            visualCondition = ConditionManager.VisualCondition.Unpredictable,
            audioCondition = ConditionManager.AudioCondition.Unpredictable
        });
    }
}