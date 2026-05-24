using UnityEngine;
using TMPro;

public class ExperimentStartScreen : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode sessionStartKey = KeyCode.Space;
    public KeyCode conditionStartKey = KeyCode.Return;

    [Header("UI")]
    public GameObject blackScreenPanel;
    public TMP_Text messageText;

    [Header("References")]
    public SessionManager sessionManager;

    private bool waitingForSessionStart = true;
    private bool waitingForConditionStart = false;

    void Start()
    {
        ShowSessionStartScreen();
    }

    void Update()
    {
        if (waitingForSessionStart && Input.GetKeyDown(sessionStartKey))
        {
            waitingForSessionStart = false;
            HideBlackScreen();

            if (sessionManager != null)
                sessionManager.StartSession();
            else
                Debug.LogWarning("SessionManager is not assigned on ExperimentStartScreen.");
        }

        if (waitingForConditionStart && Input.GetKeyDown(conditionStartKey))
        {
            waitingForConditionStart = false;
            HideBlackScreen();

            if (sessionManager != null)
                sessionManager.ConfirmConditionStart();
        }
    }

    private void ShowSessionStartScreen()
    {
        waitingForSessionStart = true;
        waitingForConditionStart = false;

        ShowBlackScreen("Press SPACE to start the experiment");
    }

    public void ShowConditionStartScreen(int conditionNumber, string conditionId)
    {
        waitingForSessionStart = false;
        waitingForConditionStart = true;

        ShowBlackScreen(
            "Condition " + conditionNumber + "\n" +
            conditionId + "\n\n" +
            "Press ENTER to start"
        );
    }

    private void ShowBlackScreen(string message)
    {
        if (blackScreenPanel != null)
        {
            blackScreenPanel.SetActive(true);

            Collider col = blackScreenPanel.GetComponent<Collider>();
            if (col != null)
                col.enabled = true;
        }

        if (messageText != null)
            messageText.text = message;
    }

    private void HideBlackScreen()
    {
        if (blackScreenPanel != null)
        {
            Collider col = blackScreenPanel.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            blackScreenPanel.SetActive(false);
        }
    }
}