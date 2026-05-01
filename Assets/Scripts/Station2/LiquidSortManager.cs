using UnityEngine;
using TMPro;

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

        [Header("UI")]
        public GameObject timerUIRoot;
        public TextMeshPro timerText;

        [Header("References")]
        public LiquidSortTube[] tubes;
        public LiquidSortTubeSelector[] tubeSelectors;
        public GameObject startButtonObject;

        [Header("Counts")]
        public int pourCount = 0;
        public int illegalPourCount = 0;

        private LiquidSortTube selectedTube;

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

            UpdateTimerVisual();
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

            UpdateTimerVisual();
            UpdateStartButtonVisual();

            Debug.Log("LIQUID SORT SUCCEEDED");
            Debug.Log("Pours: " + pourCount);
            Debug.Log("Illegal Pours: " + illegalPourCount);
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

            EnableTubeInteraction(false);
            ClearSelectedTube();

            UpdateTimerVisual();
            UpdateStartButtonVisual();

            Debug.Log("LIQUID SORT FAILED: TIME RAN OUT");
            Debug.Log("Pours: " + pourCount);
            Debug.Log("Illegal Pours: " + illegalPourCount);
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

            pourCount = 0;
            illegalPourCount = 0;
            selectedTube = null;

            InitializeAllTubes();
            EnableTubeInteraction(false);

            UpdateTimerVisual();
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

        private void UpdateStartButtonVisual()
        {
            if (startButtonObject == null)
                return;

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