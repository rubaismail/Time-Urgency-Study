using UnityEngine;
using UnityEngine.UI;

public class TaskInstructionManager : MonoBehaviour
{
    public enum TaskToBegin
    {
        Hanoi,
        LiquidSort
    }

    [Header("Instruction UI")]
    public GameObject instructionCanvasRoot;
    public Button beginTaskButton;

    [Header("Which Task This Canvas Starts")]
    public TaskToBegin taskToBegin = TaskToBegin.Hanoi;

    [Header("Task Manager References")]
    public Station1.GameManager hanoiGameManager;
    public Station2.LiquidSortManager liquidSortManager;

    [Header("Session Manager")]
    public StudySessionManager studySessionManager;

    public bool IsInstructionPanelOpen
    {
        get
        {
            return instructionCanvasRoot != null && instructionCanvasRoot.activeSelf;
        }
    }

    private void Awake()
    {
        if (beginTaskButton != null)
        {
            beginTaskButton.onClick.AddListener(BeginTask);
        }

        HideInstructions();
    }

    private void OnDestroy()
    {
        if (beginTaskButton != null)
        {
            beginTaskButton.onClick.RemoveListener(BeginTask);
        }
    }

    public void ShowInstructions()
    {
        if (studySessionManager == null)
        {
            Debug.LogWarning(name + ": Missing StudySessionManager reference.");
            return;
        }

        if (!studySessionManager.CanStartTask())
        {
            return;
        }

        if (instructionCanvasRoot != null)
        {
            instructionCanvasRoot.SetActive(true);
        }

        studySessionManager.RefreshTaskStartButtons();

        Debug.Log("Showing instructions for: " + taskToBegin);
    }

    public void BeginTask()
    {
        if (studySessionManager != null && !studySessionManager.CanStartTask())
        {
            Debug.Log("Cannot begin task because session state does not allow it.");
            return;
        }

        HideInstructions();

        switch (taskToBegin)
        {
            case TaskToBegin.Hanoi:
                if (hanoiGameManager != null)
                {
                    hanoiGameManager.StartGame();
                }
                else
                {
                    Debug.LogWarning(name + ": Missing Hanoi GameManager reference.");
                }
                break;

            case TaskToBegin.LiquidSort:
                if (liquidSortManager != null)
                {
                    liquidSortManager.StartGame();
                }
                else
                {
                    Debug.LogWarning(name + ": Missing LiquidSortManager reference.");
                }
                break;
        }

        if (studySessionManager != null)
        {
            studySessionManager.RefreshTaskStartButtons();
        }
    }

    public void HideInstructions()
    {
        if (instructionCanvasRoot != null)
        {
            instructionCanvasRoot.SetActive(false);
        }
    }
}