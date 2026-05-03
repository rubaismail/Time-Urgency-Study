using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonRayTarget : MonoBehaviour
{
    public enum TaskToStart
    {
        Hanoi,
        LiquidSort
    }

    [Header("Session Manager")]
    public StudySessionManager studySessionManager;

    [Header("Which Task Does This Button Start?")]
    public TaskToStart taskToStart = TaskToStart.Hanoi;

    [Header("Task Managers")]
    public Station1.GameManager hanoiGameManager;
    public Station2.LiquidSortManager liquidSortManager;

    [Header("Hover Color")]
    public Renderer[] highlightRenderers;
    public Color normalColor = Color.darkRed;
    public Color hoverColor = Color.yellow;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        SetHoverColor(normalColor);
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelectEnter);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnSelectEnter);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        SetHoverColor(hoverColor);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        SetHoverColor(normalColor);
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
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

        Debug.Log("Start Button Selected: " + taskToStart);

        switch (taskToStart)
        {
            case TaskToStart.Hanoi:
                StartHanoi();
                break;

            case TaskToStart.LiquidSort:
                StartLiquidSort();
                break;
        }

        studySessionManager.RefreshTaskStartButtons();
    }

    private void StartHanoi()
    {
        if (hanoiGameManager == null)
        {
            Debug.LogWarning(name + ": Missing Hanoi GameManager reference.");
            return;
        }

        hanoiGameManager.StartGame();
    }

    private void StartLiquidSort()
    {
        if (liquidSortManager == null)
        {
            Debug.LogWarning(name + ": Missing LiquidSortManager reference.");
            return;
        }

        liquidSortManager.StartGame();
    }

    private void SetHoverColor(Color color)
    {
        if (highlightRenderers == null)
            return;

        foreach (Renderer rend in highlightRenderers)
        {
            if (rend != null)
            {
                rend.material.color = color;
            }
        }
    }
}