using Station2;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class LiquidSortTubeSelector : MonoBehaviour
{
    public LiquidSortManager manager;
    public LiquidSortTube tube;

    private XRSimpleInteractable simpleInteractable;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
        simpleInteractable.hoverExited.AddListener(OnHoverExited);
        simpleInteractable.selectEntered.AddListener(OnSelected);
    }

    private void OnDisable()
    {
        simpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
        simpleInteractable.hoverExited.RemoveListener(OnHoverExited);
        simpleInteractable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (tube != null)
        {
            tube.SetHoverHighlight(true);
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (tube != null)
        {
            tube.SetHoverHighlight(false);
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (manager == null || tube == null)
        {
            Debug.LogWarning(name + ": Missing manager or tube reference.");
            return;
        }

        manager.SelectTube(tube);
    }
}