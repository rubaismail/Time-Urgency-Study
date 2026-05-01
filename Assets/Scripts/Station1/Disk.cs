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

        [HideInInspector] public Peg currentPeg;

        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private Peg previousPeg;
        private Peg hoveredPeg;

        private XRGrabInteractable grabInteractable;
        private Rigidbody rb;
        private Renderer diskRenderer;
        private Color originalColor;

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
            SetPhysicsLocked(false);
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            previousPeg = currentPeg;
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();

            if (gameManager == null || !gameManager.taskRunning)
            {
                ReturnToPreviousSpot();
                return;
            }

            if (previousPeg == null)
            {
                CountIllegalMove();
                ReturnToPreviousSpot();
                return;
            }

            if (!previousPeg.IsTopDisk(this))
            {
                CountIllegalMove();
                ReturnToPreviousSpot();
                return;
            }

            if (hoveredPeg == null)
            {
                CountInvalidDrop();
                ReturnToPreviousSpot();
                return;
            }

            if (!hoveredPeg.CanPlaceDisk(this))
            {
                CountIllegalMove();
                ReturnToPreviousSpot();
                return;
            }

            previousPeg.RemoveDisk(this);
            hoveredPeg.SnapDiskToPeg(this);

            CountLegalMove();
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

        private void RefreshGrabStates()
        {
            HanoiManager manager = FindFirstObjectByType<HanoiManager>();
            if (manager != null)
            {
                manager.RefreshAllGrabStates();
            }
        }
        
        public void SetPhysicsLocked(bool locked)
        {
            if (rb == null)
                return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = locked;
        }

        private void CountLegalMove()
        {
            HanoiManager manager = FindFirstObjectByType<HanoiManager>();
            if (manager != null)
            {
                manager.moveCount++;
                Debug.Log("Moves: " + manager.moveCount);
            }
        }

        private void CountIllegalMove()
        {
            HanoiManager manager = FindFirstObjectByType<HanoiManager>();
            if (manager != null)
            {
                manager.illegalMoveCount++;
                Debug.Log("Illegal Moves: " + manager.illegalMoveCount);
            }
        }
        
        private void CountInvalidDrop()
        {
            HanoiManager manager = FindFirstObjectByType<HanoiManager>();
            if (manager != null)
            {
                manager.invalidDropCount++;
                Debug.Log("Invalid Drops: " + manager.invalidDropCount);
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
    }
}