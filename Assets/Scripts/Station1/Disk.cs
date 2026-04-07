using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Station1
{
    public class Disk : MonoBehaviour
    {
        [Header("Disk Info")]
        public int sizeRank;
        
        [Header("Highlight")]
        public Color highlightColor = Color.yellow;

        private Renderer diskRenderer;
        private Color originalColor;

        [HideInInspector] public Peg currentPeg;

        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private Peg previousPeg;

        private Peg hoveredPeg;

        private XRGrabInteractable grabInteractable;
        private Rigidbody rb;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            
            diskRenderer = GetComponentInChildren<Renderer>();

            if (diskRenderer != null && diskRenderer.material.HasProperty("_Color"))
            {
                originalColor = diskRenderer.material.color;
            }
        }
        
        private void Start()
        {
            SetGrabbable(false);
        }

        private void OnEnable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrab);
                grabInteractable.selectExited.AddListener(OnRelease);
                grabInteractable.hoverEntered.AddListener(OnHoverEnter);
                grabInteractable.hoverExited.AddListener(OnHoverExit);
            }
        }

        private void OnDisable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrab);
                grabInteractable.selectExited.RemoveListener(OnRelease);
                grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
                grabInteractable.hoverExited.RemoveListener(OnHoverExit);
            }
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            previousPeg = currentPeg;
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            if (previousPeg == null)
            {
                ReturnToPreviousSpot();
                return;
            }

            if (!previousPeg.IsTopDisk(this))
            {
                ReturnToPreviousSpot();
                return;
            }

            if (hoveredPeg == null)
            {
                ReturnToPreviousSpot();
                return;
            }

            if (!hoveredPeg.CanPlaceDisk(this))
            {
                ReturnToPreviousSpot();
                return;
            }

            previousPeg.RemoveDisk(this);
            hoveredPeg.SnapDiskToPeg(this);
            RefreshGrabStates();
        }

        private void ReturnToPreviousSpot()
        {
            transform.position = previousPosition;
            transform.rotation = previousRotation;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            if (previousPeg != null)
            {
                currentPeg = previousPeg;
            }

            RefreshGrabStates();
        }

        private void OnTriggerEnter(Collider other)
        {
            PegZone pegZone = other.GetComponent<PegZone>();
            if (pegZone != null)
            {
                hoveredPeg = pegZone.parentPeg;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PegZone pegZone = other.GetComponent<PegZone>();
            if (pegZone != null && hoveredPeg == pegZone.parentPeg)
            {
                hoveredPeg = null;
            }
        }
        
        public void SetGrabbable(bool canGrab)
        {
            if (grabInteractable != null)
            {
                grabInteractable.enabled = canGrab;
            }
        }
        
        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            SetHighlight(true);
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            SetHighlight(false);
        }

        private void SetHighlight(bool isHighlighted)
        {
            if (diskRenderer != null && diskRenderer.material.HasProperty("_Color"))
            {
                diskRenderer.material.color = isHighlighted ? highlightColor : originalColor;
            }
        }
        
        private void RefreshGrabStates()
        {
            HanoiManager manager = FindFirstObjectByType<HanoiManager>();
            if (manager != null)
            {
                manager.RefreshAllGrabStates();
            }
        }
    }
}