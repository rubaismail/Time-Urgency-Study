using UnityEngine;

namespace Station2
{
    public class LiquidSortManager : MonoBehaviour
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
        public float timeLimit = 120f;
        public float timeRemaining;
        public float timeToCompletion = 0f;

        [Header("References")]
        public LiquidSortTube[] tubes;
        public LiquidSortTubeSelector[] tubeSelectors;
        public GameObject startButtonObject;
        public DataLogger dataLogger;
        public TimePressureController timePressureController;
        public StudySessionManager studySessionManager;

        [Header("Error Sound")]
        public AudioSource errorAudioSource;
        public AudioClip illegalPourClip;
        public float errorSoundVolume = 1f;

        [Header("Counts")]
        public int pourCount = 0;
        public int illegalPourCount = 0;

        [Header("Latest Result")]
        public TaskResult lastResult;

        private LiquidSortTube selectedTube;

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

            if (CheckWin())
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
            Debug.Log("Liquid Sort START BUTTON PRESSED");

            ResetTaskForNewRun();

            taskRunning = true;
            taskEnded = false;
            taskSucceeded = false;
            timeToCompletion = 0f;

            EnableTubeInteraction(true);

            if (timePressureController != null)
            {
                timePressureController.StartPressure(ConvertToPressureMode(), timeLimit);
            }

            UpdateStartButtonVisual();

            Debug.Log("Liquid Sort Started!");
        }

        public void EndTaskSuccess()
        {
            if (taskEnded)
                return;

            taskRunning = false;
            taskEnded = true;
            taskSucceeded = true;

            timeToCompletion = timeLimit - timeRemaining;

            EnableTubeInteraction(false);
            ClearSelectedTube();

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

            EnableTubeInteraction(false);
            ClearSelectedTube();

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
        }

        public void PlayErrorSound()
        {
            if (errorAudioSource == null)
            {
                Debug.LogWarning("LiquidSortManager is missing Error Audio Source.");
                return;
            }

            if (illegalPourClip == null)
            {
                Debug.LogWarning("LiquidSortManager is missing Illegal Pour Clip.");
                return;
            }

            errorAudioSource.PlayOneShot(illegalPourClip, errorSoundVolume);
        }

        private void NotifyStudySessionTaskFinished()
        {
            if (studySessionManager != null)
            {
                studySessionManager.OnTaskFinished("LiquidSort");
            }
            else
            {
                Debug.LogWarning("LiquidSortManager is missing StudySessionManager reference.");
            }
        }

        private void ResetTaskForNewRun()
        {
            taskRunning = false;
            taskEnded = false;
            taskSucceeded = false;

            timeRemaining = timeLimit;
            timeToCompletion = 0f;

            pourCount = 0;
            illegalPourCount = 0;
            selectedTube = null;

            InitializeAllTubes();
            EnableTubeInteraction(false);

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

            pourCount = 0;
            illegalPourCount = 0;
            selectedTube = null;

            InitializeAllTubes();
            EnableTubeInteraction(false);

            if (timePressureController != null)
            {
                timePressureController.mode = ConvertToPressureMode();
                timePressureController.ResetToIdle();
            }

            UpdateStartButtonVisual();
        }

        private void InitializeAllTubes()
        {
            if (tubes == null)
                return;

            foreach (LiquidSortTube tube in tubes)
            {
                if (tube != null)
                {
                    tube.InitializeTube();
                    tube.ReturnToOriginalPose();
                    tube.SetSelectedHighlight(false);
                    tube.SetHoverHighlight(false);
                }
            }
        }

        public void SelectTube(LiquidSortTube tube)
        {
            if (!taskRunning)
            {
                Debug.Log("Liquid Sort has not started yet.");
                return;
            }

            if (tube == null)
                return;

            if (selectedTube == null)
            {
                selectedTube = tube;
                selectedTube.SetSelectedHighlight(true);
                selectedTube.LiftAsSelected();

                Debug.Log("Selected source tube: " + tube.name);
                return;
            }

            if (selectedTube == tube)
            {
                selectedTube.SetSelectedHighlight(false);
                selectedTube.ReturnToOriginalPose();
                selectedTube = null;

                Debug.Log("Deselected tube: " + tube.name);
                return;
            }

            TryPour(selectedTube, tube);
        }

        private void TryPour(LiquidSortTube sourceTube, LiquidSortTube destinationTube)
        {
            if (sourceTube.CanPourInto(destinationTube))
            {
                StartCoroutine(sourceTube.PlayPourAnimationToward(destinationTube, () =>
                {
                    int amountPoured = sourceTube.PourInto(destinationTube);
                    pourCount++;

                    Debug.Log("Legal pour: " + sourceTube.name + " → " + destinationTube.name +
                              " | Sections poured: " + amountPoured +
                              " | Total pours: " + pourCount);

                    sourceTube.SetSelectedHighlight(false);
                    selectedTube = null;
                }));
            }
            else
            {
                illegalPourCount++;
                PlayErrorSound();

                Debug.Log("Illegal pour: " + sourceTube.name + " → " + destinationTube.name +
                          " | Illegal count: " + illegalPourCount);

                sourceTube.SetSelectedHighlight(false);
                sourceTube.ReturnToOriginalPose();
                selectedTube = null;
            }
        }

        private bool CheckWin()
        {
            if (tubes == null || tubes.Length == 0)
                return false;

            foreach (LiquidSortTube tube in tubes)
            {
                if (tube != null && !tube.IsSolved())
                {
                    return false;
                }
            }

            return true;
        }

        private void EnableTubeInteraction(bool canInteract)
        {
            if (tubeSelectors == null)
                return;

            foreach (LiquidSortTubeSelector selector in tubeSelectors)
            {
                if (selector != null)
                {
                    selector.SetInteractionEnabled(canInteract);
                }
            }
        }

        private void ClearSelectedTube()
        {
            if (selectedTube != null)
            {
                selectedTube.SetSelectedHighlight(false);
                selectedTube.ReturnToOriginalPose();
                selectedTube = null;
            }
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

        private void UpdateStartButtonVisual()
        {
            if (startButtonObject == null)
                return;

            bool showButton = !taskRunning;
            startButtonObject.SetActive(showButton);
        }

        private void BuildTaskResult(bool successValue)
        {
            lastResult = new TaskResult
            {
                taskName = "LiquidSort",
                timePressureMode = timePressureMode.ToString(),
                success = successValue,
                moveCount = pourCount,
                illegalMoveCount = illegalPourCount,
                invalidDropCount = 0,
                timeToCompletion = timeToCompletion,
                timeLimit = timeLimit
            };
        }

        private void PrintTaskResult()
        {
            if (lastResult == null)
                return;

            Debug.Log("----- LIQUID SORT RESULT -----");
            Debug.Log("Task Name: " + lastResult.taskName);
            Debug.Log("Mode: " + lastResult.timePressureMode);
            Debug.Log("Success: " + lastResult.success);
            Debug.Log("Pours: " + lastResult.moveCount);
            Debug.Log("Illegal Pours: " + lastResult.illegalMoveCount);
            Debug.Log("Time: " + lastResult.timeToCompletion.ToString("F2"));
            Debug.Log("------------------------------");
        }
    }
}