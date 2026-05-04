using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonRayTarget : MonoBehaviour
{
    [Header("Session Manager")]
    public StudySessionManager studySessionManager;

    [Header("Instruction Manager")]
    public TaskInstructionManager taskInstructionManager;

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

        if (taskInstructionManager == null)
        {
            Debug.LogWarning(name + ": Missing TaskInstructionManager reference.");
            return;
        }

        if (!studySessionManager.CanStartTask())
        {
            return;
        }

        Debug.Log("Start Button Selected. Showing instructions.");

        taskInstructionManager.ShowInstructions();

        studySessionManager.RefreshTaskStartButtons();
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