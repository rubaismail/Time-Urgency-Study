using UnityEngine;

namespace Station1
{
    public class GameManager : MonoBehaviour
    {
        [Header("Task State")]
        public bool taskRunning = false;
        public bool taskEnded = false;
        public bool taskSucceeded = false;

        [Header("Timer")]
        public float timeLimit = 60f;
        public float timeRemaining;

        [Header("Results")]
        public float timeToCompletion = 0f;

        [Header("References")]
        public HanoiManager hanoiManager;
        public Disk[] allDisks;

        private void Start()
        {
            ResetTaskToIdleState();
        }

        private void Update()
        {
            if (!taskRunning)
                return;

            timeRemaining -= Time.deltaTime;

            Debug.Log("Time Remaining: " + timeRemaining.ToString("F2"));

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
    }
}