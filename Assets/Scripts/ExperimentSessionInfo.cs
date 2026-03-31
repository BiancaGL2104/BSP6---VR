using UnityEngine;
using System;

public class ExperimentSessionInfo : MonoBehaviour
{
    public static ExperimentSessionInfo Instance { get; private set; }

    [Header("Session Metadata")]
    public string SessionId { get; private set; }
    public string ParticipantId = "PILOT";
    public string Notes = "NA";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SessionId = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }
}