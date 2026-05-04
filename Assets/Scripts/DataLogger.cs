using System;
using System.IO;
using DataLogging;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    [Header("File Settings")]
    public string taskResultsFileName = "study_results.csv";
    public string ratingResultsFileName = "rating_results.csv";

    [Header("Auto-Generated Session Info")]
    public string participantID;
    public string sessionID;

    private string taskResultsFilePath;
    private string ratingResultsFilePath;

    private void Awake()
    {
        GenerateSessionIdentifiers();

        taskResultsFilePath = Path.Combine(Application.persistentDataPath, taskResultsFileName);
        ratingResultsFilePath = Path.Combine(Application.persistentDataPath, ratingResultsFileName);

        EnsureTaskResultsFileExists();
        EnsureRatingResultsFileExists();

        Debug.Log("Participant ID: " + participantID);
        Debug.Log("Session ID: " + sessionID);
        Debug.Log("Task CSV Path: " + taskResultsFilePath);
        Debug.Log("Rating CSV Path: " + ratingResultsFilePath);
    }

    private void GenerateSessionIdentifiers()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        participantID = "AUTO";
        sessionID = "SESSION_" + timestamp;
    }

    private void EnsureTaskResultsFileExists()
    {
        if (!File.Exists(taskResultsFilePath))
        {
            string header = "participant_id,session_id,task_name,time_pressure_mode,success,moves,illegal_moves,invalid_drops,time_to_completion,time_limit\n";
            File.WriteAllText(taskResultsFilePath, header);
        }
    }

    private void EnsureRatingResultsFileExists()
    {
        if (!File.Exists(ratingResultsFilePath))
        {
            string header = "participant_id,session_id,rating_stage,task_name,time_pressure_mode,stress_rating,calmness_rating,mood_rating,pressure_rating,difficulty_rating,timestamp\n";
            File.WriteAllText(ratingResultsFilePath, header);
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

        File.AppendAllText(taskResultsFilePath, row);

        Debug.Log("Task result written to CSV.");
        Debug.Log("Task CSV Path: " + taskResultsFilePath);
    }

    public void LogRatingResult(RatingResult result)
    {
        if (result == null)
        {
            Debug.LogWarning("Tried to log a null RatingResult.");
            return;
        }

        string row =
            participantID + "," +
            sessionID + "," +
            result.ratingStage + "," +
            result.taskName + "," +
            result.timePressureMode + "," +
            result.stressRating + "," +
            result.calmnessRating + "," +
            result.moodRating + "," +
            result.pressureRating + "," +
            result.difficultyRating + "," +
            result.timestamp + "\n";

        File.AppendAllText(ratingResultsFilePath, row);

        Debug.Log("Rating result written to CSV.");
        Debug.Log("Rating CSV Path: " + ratingResultsFilePath);
    }
}