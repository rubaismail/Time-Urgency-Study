using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Station2
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class LiquidSortTubeSelector : MonoBehaviour
    {
        public LiquidSortManager manager;
        public LiquidSortTube tube;

        private XRSimpleInteractable _simpleInteractable;

        private void Awake()
        {
            _simpleInteractable = GetComponent<XRSimpleInteractable>();
        }

        private void OnEnable()
        {
            _simpleInteractable.selectEntered.AddListener(OnSelected);
        }

        private void OnDisable()
        {
            _simpleInteractable.selectEntered.RemoveListener(OnSelected);
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
}