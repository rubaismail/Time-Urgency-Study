using System;
using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    [Header("File Settings")]
    public string fileName = "study_results.csv";

    [Header("Auto-Generated Session Info")]
    public string participantID;
    public string sessionID;

    private string filePath;

    private void Awake()
    {
        GenerateSessionIdentifiers();

        filePath = Path.Combine(Application.persistentDataPath, fileName);
        EnsureFileExists();

        Debug.Log("Participant ID: " + participantID);
        Debug.Log("Session ID: " + sessionID);
        Debug.Log("CSV Path: " + filePath);
    }

    private void GenerateSessionIdentifiers()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        participantID = "AUTO";
        sessionID = "SESSION_" + timestamp;
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(filePath))
        {
            string header = "participant_id,session_id,task_name,time_pressure_mode,success,moves,illegal_moves,invalid_drops,time_to_completion,time_limit\n";
            File.WriteAllText(filePath, header);
        }
    }

    public void LogTaskResult(TaskResult result)
    {
        if (result == null)
        {
            Debug.LogWarning("Tried to log a null TaskResult.");
            return;
        }

        string row =
            participantID + "," +
            sessionID + "," +
            result.taskName + "," +
            result.timePressureMode + "," +
            result.success + "," +
            result.moveCount + "," +
            result.illegalMoveCount + "," +
            result.invalidDropCount + "," +
            result.timeToCompletion.ToString("F2") + "," +
            result.timeLimit.ToString("F2") + "\n";

        File.AppendAllText(filePath, row);

        Debug.Log("Task result written to CSV.");
        Debug.Log("CSV Path: " + filePath);
        
    }
}