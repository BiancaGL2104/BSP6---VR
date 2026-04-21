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
        public string minLabel = "0";
        public string maxLabel = "100";
    }

    [Header("Question Setup")]
    public List<QuestionData> questions = new List<QuestionData>();

    [Header("UI References")]
    public GameObject questionnairePanel;
    public TMP_Text questionCounterText;
    public TMP_Text questionText;
    public Slider responseSlider;
    public TMP_Text sliderValueText;
    public TMP_Text sliderMinText;
    public TMP_Text sliderMaxText;
    public Button previousButton;
    public Button nextButton;
    public Button submitButton;

    private int currentQuestionIndex = 0;
    private List<int> sliderAnswers = new List<int>();
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

        sliderAnswers.Clear();
        for (int i = 0; i < questions.Count; i++)
        {
            sliderAnswers.Add(50);
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

    public void OnSliderValueChanged()
    {
        if (responseSlider != null && sliderValueText != null)
        {
            sliderValueText.text = ((int)responseSlider.value).ToString();
        }
    }

    private void SaveCurrentSliderValue()
    {
        if (responseSlider == null) return;
        if (currentQuestionIndex < 0 || currentQuestionIndex >= sliderAnswers.Count) return;

        sliderAnswers[currentQuestionIndex] = (int)responseSlider.value;
    }

    public void NextQuestion()
    {
        SaveCurrentSliderValue();

        if (currentQuestionIndex < questions.Count - 1)
        {
            currentQuestionIndex++;
            ShowCurrentQuestion();
        }
    }

    public void PreviousQuestion()
    {
        SaveCurrentSliderValue();

        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            ShowCurrentQuestion();
        }
    }

    public void SubmitQuestionnaire()
    {
        SaveCurrentSliderValue();

        Debug.Log("QUESTIONNAIRE SUBMITTED");

        for (int i = 0; i < questions.Count; i++)
        {
            int value = sliderAnswers[i];

            Debug.Log(
                "Q" + (i + 1) +
                " | Condition: " + currentConditionId +
                " | Slider Value: " + value
            );

            if (ExperimentEventManager.Instance != null)
            {
                ExperimentEventManager.Instance.LogQuestionnaireAnswer(
                    currentRoundIndex,
                    currentConditionId,
                    i + 1,
                    questions[i].questionText,
                    value
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

        if (sliderMinText != null)
        {
            sliderMinText.text = currentQuestion.minLabel;
        }

        if (sliderMaxText != null)
        {
            sliderMaxText.text = currentQuestion.maxLabel;
        }

        if (responseSlider != null)
        {
            responseSlider.value = sliderAnswers[currentQuestionIndex];
        }

        if (sliderValueText != null)
        {
            sliderValueText.text = sliderAnswers[currentQuestionIndex].ToString();
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
    }

    private void InitializeDefaultQuestions()
    {
        if (questions.Count > 0)
            return;

        questions.Add(new QuestionData
        {
            questionText = "How much mental and perceptual activity was required (e.g. thinking, deciding, calculating, remembering, looking, searching, etc)? Was the task easy or demanding, simple or complex, exacting or forgiving?",
            minLabel = "Very low",
            maxLabel = "Very high"
        });

        questions.Add(new QuestionData
        {
            questionText = "How much physical activity was required (e.g. pushing, pulling, turning, controlling, activating, etc)? Was the task easy or demanding, slow or brisk, slack or strenuous, restful or laborious?",
            minLabel = "Very low",
            maxLabel = "Very high"
        });

        questions.Add(new QuestionData
        {
            questionText = "How much time pressure did you feel due to the rate of pace at which the tasks or task elements occurred? Was the pace slow and leisurely or rapid and frantic?",
            minLabel = "Very low",
            maxLabel = "Very high"
        });

        questions.Add(new QuestionData
        {
            questionText = "How successful do you think you were in accomplishing the goals of the task set by the experimenter (or yourself)? How satisfied were you with your performance in accomplishing these goals?",
            minLabel = "Very low",
            maxLabel = "Very high"
        });

        questions.Add(new QuestionData
        {
            questionText = "How hard did you have to work (mentally and physically) to accomplish your level of performance?",
            minLabel = "Very low",
            maxLabel = "Very high"
        });

        questions.Add(new QuestionData
        {
            questionText = "How insecure, discouraged, irritated, stressed and annoyed versus secure, gratified, content, relaxed and complacent did you feel during the task?",
            minLabel = "Very low",
            maxLabel = "Very high"
        });
    }
}