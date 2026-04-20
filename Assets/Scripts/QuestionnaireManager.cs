using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionnaireManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public string questionText;
        public List<string> answerOptions = new List<string>();
    }

    [Header("Question Setup")]
    public List<QuestionData> questions = new List<QuestionData>();

    [Header("UI References")]
    public GameObject questionnairePanel;
    public TMP_Text questionCounterText;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public TMP_Text[] answerButtonTexts;
    public Button previousButton;
    public Button nextButton;
    public Button submitButton;

    [Header("Selection Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.blue;

    private int currentQuestionIndex = 0;
    private List<int> selectedAnswers = new List<int>();
    private SessionManager sessionManager;

    private int currentRoundIndex = -1;
    private string currentConditionId = "NONE";

    private void Start()
    {
        if (questionnairePanel != null)
        {
            questionnairePanel.SetActive(false);
        }

        InitializeDefaultQuestions();
    }

    public void BeginQuestionnaire(SessionManager manager, int roundIndex, string conditionId)
    {
        sessionManager = manager;
        currentRoundIndex = roundIndex;
        currentConditionId = conditionId;

        selectedAnswers.Clear();
        for (int i = 0; i < questions.Count; i++)
        {
            selectedAnswers.Add(-1);
        }

        currentQuestionIndex = 0;

        if (questionnairePanel != null)
        {
            questionnairePanel.SetActive(true);
        }

        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogQuestionnaireStart("POST_CONDITION_QUESTIONNAIRE");
        }

        ShowCurrentQuestion();
    }
    

    public void HideQuestionnaire()
    {
        if (questionnairePanel != null)
        {
            questionnairePanel.SetActive(false);
        }
    }

    public void SelectAnswer(int answerIndex)
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= selectedAnswers.Count)
            return;

        selectedAnswers[currentQuestionIndex] = answerIndex;
        UpdateAnswerButtonVisuals();
    }

    public void NextQuestion()
    {
        if (currentQuestionIndex < questions.Count - 1)
        {
            currentQuestionIndex++;
            ShowCurrentQuestion();
        }
    }

    public void PreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            ShowCurrentQuestion();
        }
    }

    public void SubmitQuestionnaire()
    {
        Debug.Log("QUESTIONNAIRE SUBMITTED");

        for (int i = 0; i < questions.Count; i++)
        {
            int selectedIndex = selectedAnswers[i];

            string answerLabel = "NO_ANSWER";

            if (selectedIndex >= 0 && selectedIndex < questions[i].answerOptions.Count)
            {
                answerLabel = questions[i].answerOptions[selectedIndex];
            }

            Debug.Log(
                "Q" + (i + 1) +
                " | Condition: " + currentConditionId +
                " | Answer Index: " + selectedIndex +
                " | Answer Label: " + answerLabel
            );

            if (ExperimentEventManager.Instance != null)
            {
                ExperimentEventManager.Instance.LogQuestionnaireAnswer(
                    currentRoundIndex,
                    currentConditionId,
                    i + 1,
                    questions[i].questionText,
                    selectedIndex,
                    answerLabel
                );
            }
        }
        if (ExperimentEventManager.Instance != null)
        {
            ExperimentEventManager.Instance.LogQuestionnaireEnd("POST_CONDITION_QUESTIONNAIRE");
        }

        HideQuestionnaire();

        if (sessionManager != null)
        {
            sessionManager.OnQuestionnaireSubmitted();
        }
    }

    private void ShowCurrentQuestion()
    {
        if (questions.Count == 0) return;
        if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count) return;

        QuestionData currentQuestion = questions[currentQuestionIndex];

        if (questionCounterText != null)
        {
            questionCounterText.text = "Question " + (currentQuestionIndex + 1) + " / " + questions.Count;
        }

        if (questionText != null)
        {
            questionText.text = currentQuestion.questionText;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasOption = i < currentQuestion.answerOptions.Count;

            answerButtons[i].gameObject.SetActive(hasOption);

            if (hasOption && i < answerButtonTexts.Length)
            {
                answerButtonTexts[i].text = currentQuestion.answerOptions[i];
            }
        }

        if (previousButton != null)
        {
            previousButton.gameObject.SetActive(currentQuestionIndex > 0);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(currentQuestionIndex < questions.Count - 1);
        }

        if (submitButton != null)
        {
            submitButton.gameObject.SetActive(currentQuestionIndex == questions.Count - 1);
        }

        UpdateAnswerButtonVisuals();
    }

    private void UpdateAnswerButtonVisuals()
    {
        int selectedIndex = selectedAnswers[currentQuestionIndex];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            Image buttonImage = answerButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = (i == selectedIndex) ? selectedColor : normalColor;
            }
        }
    }

    private void InitializeDefaultQuestions()
    {
        if (questions.Count > 0)
            return;

        List<string> defaultOptions = new List<string>
        {
            "Not at all",
            "A little",
            "Moderately",
            "A lot"
        };

        questions.Add(new QuestionData
        {
            questionText = "How distracting was this condition?",
            answerOptions = new List<string>(defaultOptions)
        });

        questions.Add(new QuestionData
        {
            questionText = "How difficult did this condition feel?",
            answerOptions = new List<string>(defaultOptions)
        });

        questions.Add(new QuestionData
        {
            questionText = "How mentally demanding was this condition?",
            answerOptions = new List<string>(defaultOptions)
        });

        questions.Add(new QuestionData
        {
            questionText = "How noticeable were the distractors?",
            answerOptions = new List<string>(defaultOptions)
        });

        questions.Add(new QuestionData
        {
            questionText = "How confident are you in your performance?",
            answerOptions = new List<string>(defaultOptions)
        });
    }
}