using System.IO;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    private string filePath;
    private bool isInitialized = false;

    void Start()
    {
        InitializeLogger();
    }

    private void InitializeLogger()
    {
        if (isInitialized) return;

        string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");

        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        filePath = Path.Combine(logDirectory, "event_log.csv");

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "timestamp,event_type,round_index,condition_id,object_id,extra_1,extra_2\n");
        }

        isInitialized = true;
        Debug.Log("Event log path: " + filePath);
    }

    public void LogEvent(string eventType, int roundIndex, string conditionId, string objectId, string extra1 = "NA", string extra2 = "NA")
    {
        if (!isInitialized)
        {
            InitializeLogger();
        }

        float timestamp = Time.realtimeSinceStartup;

        string line = timestamp + "," +
                      eventType + "," +
                      roundIndex + "," +
                      conditionId + "," +
                      objectId + "," +
                      extra1 + "," +
                      extra2 + "\n";

        File.AppendAllText(filePath, line);
        Debug.Log("LOGGED: " + line);
    }
}