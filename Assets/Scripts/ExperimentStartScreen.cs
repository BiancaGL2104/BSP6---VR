using UnityEngine;

public class ExperimentStartScreen : MonoBehaviour
{
    public KeyCode startKey = KeyCode.Space;

    public GameObject blackScreenPanel;
    public SessionManager sessionManager;

    private bool experimentStarted = false;

    void Start()
    {
        experimentStarted = false;

        if (blackScreenPanel != null)
            blackScreenPanel.SetActive(true);
    }

    void Update()
    {
        if (!experimentStarted && Input.GetKeyDown(startKey))
        {
            StartExperiment();
        }
    }

    void StartExperiment()
    {
        experimentStarted = true;

        if (blackScreenPanel != null)
        {
            Collider col = blackScreenPanel.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            blackScreenPanel.SetActive(false);
        }

        if (sessionManager != null)
            sessionManager.StartSession();
        else
            Debug.LogWarning("SessionManager is not assigned on ExperimentStartScreen.");
    }
}