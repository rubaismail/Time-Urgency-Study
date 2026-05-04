using UnityEngine;

namespace Station1
{
    public class GameManager : MonoBehaviour
    {
        public enum TimePressureMode
        {
            VisualCountdown,
            AudioCountdown,
            NpcUrging
        }

        [Header("Session Condition")]
        public TimePressureMode timePressureMode = TimePressureMode.VisualCountdown;

        [Header("Task State")]
        public bool taskRunning = false;
        public bool taskEnded = false;
        public bool taskSucceeded = false;

        [Header("Timer")]
        public float timeLimit = 60f;
        public float timeRemaining;
        public float timeToCompletion = 0f;

        [Header("References")]
        public HanoiManager hanoiManager;
        public Disk[] allDisks;
        public GameObject startButtonObject;
        public DataLogger dataLogger;
        public TimePressureController timePressureController;
        public StudySessionManager studySessionManager;

        [Header("Error Sound")]
        public AudioSource errorAudioSource;
        public AudioClip illegalMoveClip;
        public float errorSoundVolume = 1f;

        [Header("Latest Result")]
        public TaskResult lastResult;

        private void Start()
        {
            ResetTaskToIdleState();
            UpdateStartButtonVisual();
        }

        private void Update()
        {
            if (!taskRunning)
                return;

            timeRemaining -= Time.deltaTime;

            if (timeRemaining < 0f)
                timeRemaining = 0f;

            UpdateTimePressure();

            if (hanoiManager != null && hanoiManager.CheckWin())
            {
                EndTaskSuccess();
                return;
            }

            if (timeRemaining <= 0f)
            {
                EndTaskFailure();
            }
        }

        public void StartGame()
        {
            Debug.Log("START BUTTON PRESSED");

            ResetTaskForNewRun();

            taskRunning = true;
            taskEnded = false;
            taskSucceeded = false;
            timeToCompletion = 0f;

            if (hanoiManager != null)
            {
                hanoiManager.InitializePuzzle();
                hanoiManager.RefreshAllGrabStates();
            }

            if (timePressureController != null)
            {
                timePressureController.StartPressure(ConvertToPressureMode(), timeLimit);
            }

            UpdateStartButtonVisual();

            Debug.Log("Task 1 Started!");
        }

        public void EndTaskSuccess()
        {
            if (taskEnded)
                return;

            taskRunning = false;
            taskEnded = true;
            taskSucceeded = true;

            timeToCompletion = timeLimit - timeRemaining;

            DisableAllDiskGrabs();

            BuildTaskResult(true);
            PrintTaskResult();

            if (dataLogger != null)
            {
                dataLogger.LogTaskResult(lastResult);
            }

            if (timePressureController != null)
            {
                timePressureController.EndPressure(true);
            }

            UpdateStartButtonVisual();
            NotifyStudySessionTaskFinished();

            Debug.Log("TASK SUCCEEDED");
        }

        public void EndTaskFailure()
        {
            if (taskEnded)
                return;

            taskRunning = false;
            taskEnded = true;
            taskSucceeded = false;

            timeRemaining = 0f;
            timeToCompletion = timeLimit;

            DisableAllDiskGrabs();

            BuildTaskResult(false);
            PrintTaskResult();

            if (dataLogger != null)
            {
                dataLogger.LogTaskResult(lastResult);
            }

            if (timePressureController != null)
            {
                timePressureController.EndPressure(false);
            }

            UpdateStartButtonVisual();
            NotifyStudySessionTaskFinished();

            Debug.Log("TASK FAILED: TIME RAN OUT");
        }

        public void PlayErrorSound()
        {
            if (errorAudioSource == null)
            {
                Debug.LogWarning("Hanoi GameManager is missing Error Audio Source.");
                return;
            }

            if (illegalMoveClip == null)
            {
                Debug.LogWarning("Hanoi GameManager is missing Illegal Move Clip.");
                return;
            }

            errorAudioSource.PlayOneShot(illegalMoveClip, errorSoundVolume);
        }

        private void NotifyStudySessionTaskFinished()
        {
            if (studySessionManager != null)
            {
                studySessionManager.OnTaskFinished("TowersOfHanoi");
            }
            else
            {
                Debug.LogWarning("Hanoi GameManager is missing StudySessionManager reference.");
            }
        }

        private void ResetTaskForNewRun()
        {
            taskRunning = false;
            taskEnded = false;
            taskSucceeded = false;
            timeRemaining = timeLimit;
            timeToCompletion = 0f;

            DisableAllDiskGrabs();

            if (hanoiManager != null)
            {
                hanoiManager.ClearAllPegs();
            }

            if (timePressureController != null)
            {
                timePressureController.mode = ConvertToPressureMode();
                timePressureController.ResetToIdle();
            }

            UpdateStartButtonVisual();
        }

        private void ResetTaskToIdleState()
        {
            taskRunning = false;
            taskEnded = false;
            taskSucceeded = false;
            timeRemaining = timeLimit;
            timeToCompletion = 0f;

            DisableAllDiskGrabs();

            if (hanoiManager != null)
            {
                hanoiManager.ClearAllPegs();
            }

            if (timePressureController != null)
            {
                timePressureController.mode = ConvertToPressureMode();
                timePressureController.ResetToIdle();
            }

            UpdateStartButtonVisual();
        }

        private void UpdateTimePressure()
        {
            if (timePressureController != null)
            {
                timePressureController.UpdatePressure(timeRemaining, timeLimit, taskRunning, taskEnded);
            }
        }

        private TimePressureController.Mode ConvertToPressureMode()
        {
            switch (timePressureMode)
            {
                case TimePressureMode.AudioCountdown:
                    return TimePressureController.Mode.AudioCountdown;

                case TimePressureMode.NpcUrging:
                    return TimePressureController.Mode.NpcUrging;

                default:
                    return TimePressureController.Mode.VisualCountdown;
            }
        }

        private void BuildTaskResult(bool successValue)
        {
            if (hanoiManager == null)
                return;

            lastResult = new TaskResult
            {
                taskName = "TowersOfHanoi",
                timePressureMode = timePressureMode.ToString(),
                success = successValue,
                moveCount = hanoiManager.moveCount,
                illegalMoveCount = hanoiManager.illegalMoveCount,
                invalidDropCount = hanoiManager.invalidDropCount,
                timeToCompletion = timeToCompletion,
                timeLimit = timeLimit
            };
        }

        private void PrintTaskResult()
        {
            if (lastResult == null)
                return;

            Debug.Log("----- TASK RESULT -----");
            Debug.Log("Task Name: " + lastResult.taskName);
            Debug.Log("Time Pressure Mode: " + lastResult.timePressureMode);
            Debug.Log("Success: " + lastResult.success);
            Debug.Log("Moves: " + lastResult.moveCount);
            Debug.Log("Illegal Moves: " + lastResult.illegalMoveCount);
            Debug.Log("Invalid Drops: " + lastResult.invalidDropCount);
            Debug.Log("Time To Completion: " + lastResult.timeToCompletion.ToString("F2"));
            Debug.Log("Time Limit: " + lastResult.timeLimit.ToString("F2"));
            Debug.Log("-----------------------");
        }

        private void DisableAllDiskGrabs()
        {
            if (allDisks == null)
                return;

            foreach (Disk disk in allDisks)
            {
                if (disk != null)
                    disk.SetGrabbable(false);
            }
        }

        private void UpdateStartButtonVisual()
        {
            if (startButtonObject == null)
                return;

            bool showButton = !taskRunning;
            startButtonObject.SetActive(showButton);
        }
    }
}