using Station1;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonRayTarget : MonoBehaviour
{
    public GameManager gameManager;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
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
        Debug.Log("Hovering Start Button");
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Start Button Selected");

        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }
}