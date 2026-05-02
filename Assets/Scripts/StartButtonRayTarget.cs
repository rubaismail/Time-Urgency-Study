using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonRayTarget : MonoBehaviour
{
    private static bool anyTaskRunning = false;

    [Header("Task 1 (Hanoi)")]
    public Station1.GameManager hanoiGameManager;

    [Header("Task 2 (Liquid Sort)")]
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

    private void Update()
    {
        anyTaskRunning =
            (hanoiGameManager != null && hanoiGameManager.taskRunning) ||
            (liquidSortManager != null && liquidSortManager.taskRunning);
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
        if (anyTaskRunning)
        {
            Debug.Log("Cannot start another task while one task is already running.");
            return;
        }

        Debug.Log("Start Button Selected");

        if (hanoiGameManager != null)
        {
            hanoiGameManager.StartGame();
            anyTaskRunning = true;
            return;
        }

        if (liquidSortManager != null)
        {
            liquidSortManager.StartGame();
            anyTaskRunning = true;
            return;
        }
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