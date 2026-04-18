using UnityEngine;
using TMPro;

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

        [Header("UI")]
        public GameObject timerUIRoot;
        public TextMeshPro timerText;

        [Header("References")]
        public HanoiManager hanoiManager;
        public Disk[] allDisks;
        public GameObject startButtonObject;

        private void Start()
        {
            ResetTaskToIdleState();
            UpdateTimerVisual();
            UpdateStartButtonVisual();
        }

        private void Update()
        {
            if (!taskRunning)
                return;

            timeRemaining -= Time.deltaTime;

            if (timeRemaining < 0f)
                timeRemaining = 0f;

            UpdateTimerVisual();

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

            UpdateTimerVisual();
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
            UpdateTimerVisual();
            UpdateStartButtonVisual();

            Debug.Log("TASK SUCCEEDED");
            Debug.Log("Success: true");
            Debug.Log("Moves: " + hanoiManager.moveCount);
            Debug.Log("Illegal Moves: " + hanoiManager.illegalMoveCount);
            Debug.Log("Invalid Drops: " + hanoiManager.invalidDropCount);
            Debug.Log("Time To Completion: " + timeToCompletion.ToString("F2"));
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
            UpdateTimerVisual();
            UpdateStartButtonVisual();

            Debug.Log("TASK FAILED: TIME RAN OUT");
            Debug.Log("Success: false");
            Debug.Log("Moves: " + hanoiManager.moveCount);
            Debug.Log("Illegal Moves: " + hanoiManager.illegalMoveCount);
            Debug.Log("Invalid Drops: " + hanoiManager.invalidDropCount);
            Debug.Log("Time To Completion: " + timeToCompletion.ToString("F2"));
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

            UpdateTimerVisual();
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

            UpdateTimerVisual();
            UpdateStartButtonVisual();
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

            // Show button only when task is idle
            bool showButton = !taskRunning;
            startButtonObject.SetActive(showButton);
        }

        private void UpdateTimerVisual()
        {
            if (timerUIRoot != null)
            {
                bool showVisualTimer = (timePressureMode == TimePressureMode.VisualCountdown);
                timerUIRoot.SetActive(showVisualTimer);
            }

            if (timerText == null)
                return;

            if (timePressureMode != TimePressureMode.VisualCountdown)
                return;

            if (!taskRunning && !taskEnded)
            {
                timerText.text = "Press Button To Start";
            }
            else if (taskRunning)
            {
                timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
            }
            else if (taskEnded && taskSucceeded)
            {
                timerText.text = "Success!";
            }
            else if (taskEnded && !taskSucceeded)
            {
                timerText.text = "Time Is Up!";
            }
        }
    }
}