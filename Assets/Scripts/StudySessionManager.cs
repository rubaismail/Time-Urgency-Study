using System.Collections;
using RatingPanel;
using UnityEngine;

public class StudySessionManager : MonoBehaviour
{
    public enum SharedTimePressureMode
    {
        VisualCountdown,
        AudioCountdown,
        NpcUrging
    }

    [Header("Mode Selection")]
    public bool randomizeModeOnStart = false;
    public SharedTimePressureMode selectedMode = SharedTimePressureMode.VisualCountdown;
    public bool hasChosenMode = false;

    [Header("Startup UI")]
    public GameObject modalitySelectionUI;

    [Header("Rating UI")]
    public RatingPanelManager ratingPanelManager;
    public bool preTaskRatingCompleted = false;
    
    [Header("Instruction UI")]
    public TaskInstructionManager taskInstructionManager;

    [Header("Task Outcome Message")]
    public TaskOutcomeMessageManager taskOutcomeMessageManager;

    [Header("Post-Task Rating Delay")]
    public float postTaskPanelDelay = 2f;
    public bool postTaskRatingPending = false;

    [Header("Movement Lock")]
    public Behaviour[] movementComponentsToDisable;
    public GameObject[] movementObjectsToDisable;

    [Header("Task Managers")]
    public Station1.GameManager hanoiGameManager;
    public Station2.LiquidSortManager liquidSortManager;

    [Header("Time Pressure Controllers")]
    public TimePressureController hanoiPressureController;
    public TimePressureController liquidSortPressureController;

    private bool movementShouldBeEnabled = false;
    private Coroutine postTaskRatingCoroutine;

    private void Awake()
    {
        hasChosenMode = false;
        preTaskRatingCompleted = false;
        postTaskRatingPending = false;
        movementShouldBeEnabled = false;

        if (taskOutcomeMessageManager != null)
        {
            taskOutcomeMessageManager.HideMessage();
        }

        ForceMovementState();

        if (randomizeModeOnStart)
        {
            RandomizeMode();
            ConfirmSelectedMode();
        }
        else
        {
            ShowModalitySelection();
        }
    }

    private void Start()
    {
        ForceMovementState();
        RefreshTaskStartButtons();
    }

    private void Update()
    {
        RefreshTaskStartButtons();

        if (!hasChosenMode)
        {
            movementShouldBeEnabled = false;
            ForceMovementState();
        }
    }

    private void LateUpdate()
    {
        if (!hasChosenMode)
        {
            movementShouldBeEnabled = false;
            ForceMovementState();
        }
    }

    private void ShowModalitySelection()
    {
        if (modalitySelectionUI != null)
        {
            modalitySelectionUI.SetActive(true);
        }

        if (taskOutcomeMessageManager != null)
        {
            taskOutcomeMessageManager.HideMessage();
        }

        preTaskRatingCompleted = false;
        postTaskRatingPending = false;
        movementShouldBeEnabled = false;
        ForceMovementState();

        RefreshTaskStartButtons();
    }

    public void ChooseVisualCountdown()
    {
        selectedMode = SharedTimePressureMode.VisualCountdown;
        ConfirmSelectedMode();
    }

    public void ChooseAudioCountdown()
    {
        selectedMode = SharedTimePressureMode.AudioCountdown;
        ConfirmSelectedMode();
    }

    public void ChooseNpcUrging()
    {
        selectedMode = SharedTimePressureMode.NpcUrging;
        ConfirmSelectedMode();
    }

    private void ConfirmSelectedMode()
    {
        hasChosenMode = true;
        preTaskRatingCompleted = false;
        postTaskRatingPending = false;

        ApplyModeToAllTasks();

        if (modalitySelectionUI != null)
        {
            modalitySelectionUI.SetActive(false);
        }

        movementShouldBeEnabled = true;
        ForceMovementState();

        if (ratingPanelManager != null)
        {
            ratingPanelManager.ShowPreTaskRating(selectedMode.ToString());
        }
        else
        {
            Debug.LogWarning("StudySessionManager is missing RatingPanelManager. Skipping pre-task rating.");
            preTaskRatingCompleted = true;
        }

        RefreshTaskStartButtons();

        Debug.Log("Confirmed study mode: " + selectedMode);
    }

    public void OnRatingPanelSubmitted(RatingPanelManager.RatingStage ratingStage)
    {
        if (ratingStage == RatingPanelManager.RatingStage.PreTask)
        {
            preTaskRatingCompleted = true;
            Debug.Log("Pre-task rating completed.");
        }

        if (ratingStage == RatingPanelManager.RatingStage.PostTask)
        {
            Debug.Log("Post-task rating completed.");
        }

        RefreshTaskStartButtons();
    }

    public void OnTaskFinished(string taskName, bool success)
    {
        postTaskRatingPending = true;
        RefreshTaskStartButtons();

        if (taskOutcomeMessageManager != null)
        {
            if (success)
            {
                taskOutcomeMessageManager.ShowSuccessMessage();
            }
            else
            {
                taskOutcomeMessageManager.ShowFailureMessage();
            }
        }

        if (postTaskRatingCoroutine != null)
        {
            StopCoroutine(postTaskRatingCoroutine);
        }

        postTaskRatingCoroutine = StartCoroutine(ShowPostTaskRatingAfterDelay(taskName));
    }

    private IEnumerator ShowPostTaskRatingAfterDelay(string taskName)
    {
        Debug.Log("Task finished. Showing outcome message before post-task rating panel: " + taskName);

        yield return new WaitForSeconds(postTaskPanelDelay);

        if (taskOutcomeMessageManager != null)
        {
            taskOutcomeMessageManager.HideMessage();
        }

        postTaskRatingPending = false;

        if (ratingPanelManager != null)
        {
            ratingPanelManager.ShowPostTaskRating(taskName, selectedMode.ToString());
        }
        else
        {
            Debug.LogWarning("StudySessionManager is missing RatingPanelManager. Cannot show post-task rating.");
        }

        RefreshTaskStartButtons();

        postTaskRatingCoroutine = null;
    }

    public void RandomizeMode()
    {
        int randomIndex = Random.Range(0, 3);
        selectedMode = (SharedTimePressureMode)randomIndex;

        Debug.Log("Randomized Study Mode: " + selectedMode);
    }

    public bool IsAnyTaskRunning()
    {
        bool hanoiRunning = hanoiGameManager != null && hanoiGameManager.taskRunning;
        bool liquidSortRunning = liquidSortManager != null && liquidSortManager.taskRunning;

        return hanoiRunning || liquidSortRunning;
    }

    public bool IsRatingPanelOpen()
    {
        return ratingPanelManager != null && ratingPanelManager.IsPanelOpen;
    }

    public bool CanStartTask()
    {
        if (!hasChosenMode)
        {
            Debug.Log("Cannot start task yet. No time pressure mode has been selected.");
            return false;
        }

        if (!preTaskRatingCompleted)
        {
            Debug.Log("Cannot start task yet. Pre-task rating has not been completed.");
            return false;
        }

        if (postTaskRatingPending)
        {
            Debug.Log("Cannot start task while post-task rating is pending.");
            return false;
        }

        if (IsRatingPanelOpen())
        {
            Debug.Log("Cannot start task while rating panel is open.");
            return false;
        }

        if (taskInstructionManager != null && taskInstructionManager.IsInstructionPanelOpen)
        {
            Debug.Log("Cannot start task while instruction panel is open.");
            return false;
        }

        if (IsAnyTaskRunning())
        {
            Debug.Log("Cannot start task. Another task is already running.");
            return false;
        }

        return true;
    }

    public void RefreshTaskStartButtons()
    {
        bool instructionPanelOpen =
            taskInstructionManager != null &&
            taskInstructionManager.IsInstructionPanelOpen;

        bool canShowTaskButtons =
            hasChosenMode &&
            preTaskRatingCompleted &&
            !postTaskRatingPending &&
            !IsAnyTaskRunning() &&
            !IsRatingPanelOpen() &&
            !instructionPanelOpen;

        if (hanoiGameManager != null && hanoiGameManager.startButtonObject != null)
        {
            hanoiGameManager.startButtonObject.SetActive(canShowTaskButtons);
        }

        if (liquidSortManager != null && liquidSortManager.startButtonObject != null)
        {
            liquidSortManager.startButtonObject.SetActive(canShowTaskButtons);
        }
    }

    public void ApplyModeToAllTasks()
    {
        if (hanoiGameManager != null)
        {
            hanoiGameManager.timePressureMode = ConvertToHanoiMode(selectedMode);
        }

        if (liquidSortManager != null)
        {
            liquidSortManager.timePressureMode = ConvertToLiquidSortMode(selectedMode);
        }

        if (hanoiPressureController != null)
        {
            hanoiPressureController.SetMode(ConvertToControllerMode(selectedMode));
        }

        if (liquidSortPressureController != null)
        {
            liquidSortPressureController.SetMode(ConvertToControllerMode(selectedMode));
        }

        Debug.Log("Applied study mode to all tasks: " + selectedMode);
    }

    private void ForceMovementState()
    {
        if (movementComponentsToDisable != null)
        {
            foreach (Behaviour component in movementComponentsToDisable)
            {
                if (component != null)
                {
                    component.enabled = movementShouldBeEnabled;
                }
            }
        }

        if (movementObjectsToDisable != null)
        {
            foreach (GameObject obj in movementObjectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(movementShouldBeEnabled);
                }
            }
        }
    }

    private Station1.GameManager.TimePressureMode ConvertToHanoiMode(SharedTimePressureMode mode)
    {
        switch (mode)
        {
            case SharedTimePressureMode.AudioCountdown:
                return Station1.GameManager.TimePressureMode.AudioCountdown;

            case SharedTimePressureMode.NpcUrging:
                return Station1.GameManager.TimePressureMode.NpcUrging;

            default:
                return Station1.GameManager.TimePressureMode.VisualCountdown;
        }
    }

    private Station2.LiquidSortManager.TimePressureMode ConvertToLiquidSortMode(SharedTimePressureMode mode)
    {
        switch (mode)
        {
            case SharedTimePressureMode.AudioCountdown:
                return Station2.LiquidSortManager.TimePressureMode.AudioCountdown;

            case SharedTimePressureMode.NpcUrging:
                return Station2.LiquidSortManager.TimePressureMode.NpcUrging;

            default:
                return Station2.LiquidSortManager.TimePressureMode.VisualCountdown;
        }
    }

    private TimePressureController.Mode ConvertToControllerMode(SharedTimePressureMode mode)
    {
        switch (mode)
        {
            case SharedTimePressureMode.AudioCountdown:
                return TimePressureController.Mode.AudioCountdown;

            case SharedTimePressureMode.NpcUrging:
                return TimePressureController.Mode.NpcUrging;

            default:
                return TimePressureController.Mode.VisualCountdown;
        }
    }
}