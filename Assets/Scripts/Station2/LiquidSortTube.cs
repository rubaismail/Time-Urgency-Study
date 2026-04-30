using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station2
{
    public enum LiquidColor
    {
        Empty,
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Orange
    }

    public class LiquidSortTube : MonoBehaviour
    {
        [Header("Tube Settings")]
        public int capacity = 4;

        [Header("Slot Transforms - Bottom to Top")]
        public Transform[] slots;

        [Header("Liquid Visual Prefab")]
        public GameObject liquidSectionPrefab;

        [Header("Starting Liquid - Bottom to Top")]
        public LiquidColor[] startingColors;

        [Header("Visual Feedback")]
        public Renderer[] highlightRenderers;
        public Color normalHighlightColor = Color.white;
        public Color hoverHighlightColor = Color.yellow;
        public Color selectedHighlightColor = Color.cyan;

        [Header("Selection / Pour Animation")]
        public float selectedLiftHeight = 0.18f;
        public float pourHoverHeight = 0.25f;
        public float animationSpeed = 6f;
        public float pourTiltAngle = -55f;

        private Vector3 originalLocalPosition;
        private Quaternion originalLocalRotation;

        private readonly List<LiquidColor> liquidSections = new List<LiquidColor>();
        private readonly List<GameObject> spawnedVisuals = new List<GameObject>();

        private void Awake()
        {
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
        }

        private void Start()
        {
            InitializeTube();
        }

        public void InitializeTube()
        {
            liquidSections.Clear();

            for (int i = 0; i < startingColors.Length && i < capacity; i++)
            {
                if (startingColors[i] != LiquidColor.Empty)
                {
                    liquidSections.Add(startingColors[i]);
                }
            }

            RefreshVisuals();
        }

        public bool IsEmpty()
        {
            return liquidSections.Count == 0;
        }

        public bool IsFull()
        {
            return liquidSections.Count >= capacity;
        }

        public LiquidColor GetTopColor()
        {
            if (IsEmpty())
            {
                return LiquidColor.Empty;
            }

            return liquidSections[liquidSections.Count - 1];
        }

        public int GetFreeSpace()
        {
            return capacity - liquidSections.Count;
        }

        public int CountTopSameColorGroup()
        {
            if (IsEmpty())
            {
                return 0;
            }

            LiquidColor topColor = GetTopColor();
            int count = 0;

            for (int i = liquidSections.Count - 1; i >= 0; i--)
            {
                if (liquidSections[i] == topColor)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public bool CanPourInto(LiquidSortTube destinationTube)
        {
            if (destinationTube == null)
            {
                return false;
            }

            if (destinationTube == this)
            {
                return false;
            }

            if (IsEmpty())
            {
                return false;
            }

            if (destinationTube.IsFull())
            {
                return false;
            }

            LiquidColor sourceTopColor = GetTopColor();
            LiquidColor destinationTopColor = destinationTube.GetTopColor();

            return destinationTopColor == LiquidColor.Empty || destinationTopColor == sourceTopColor;
        }

        public int PourInto(LiquidSortTube destinationTube)
        {
            if (!CanPourInto(destinationTube))
            {
                return 0;
            }

            LiquidColor colorToPour = GetTopColor();

            int sameColorCount = CountTopSameColorGroup();
            int freeSpace = destinationTube.GetFreeSpace();
            int amountToPour = Mathf.Min(sameColorCount, freeSpace);

            for (int i = 0; i < amountToPour; i++)
            {
                liquidSections.RemoveAt(liquidSections.Count - 1);
                destinationTube.liquidSections.Add(colorToPour);
            }

            RefreshVisuals();
            destinationTube.RefreshVisuals();

            return amountToPour;
        }

        public bool IsSolved()
        {
            if (IsEmpty())
            {
                return true;
            }

            if (liquidSections.Count != capacity)
            {
                return false;
            }

            LiquidColor firstColor = liquidSections[0];

            for (int i = 1; i < liquidSections.Count; i++)
            {
                if (liquidSections[i] != firstColor)
                {
                    return false;
                }
            }

            return true;
        }

        public void RefreshVisuals()
        {
            ClearVisuals();

            for (int i = 0; i < liquidSections.Count; i++)
            {
                if (i >= slots.Length)
                {
                    Debug.LogWarning(name + ": Not enough slot transforms assigned.");
                    return;
                }

                GameObject newSection = Instantiate(liquidSectionPrefab, slots[i]);

                newSection.transform.localPosition = Vector3.zero;
                newSection.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                newSection.transform.localScale = liquidSectionPrefab.transform.localScale;

                newSection.name = "Liquid_" + liquidSections[i];

                Renderer sectionRenderer = newSection.GetComponent<Renderer>();

                if (sectionRenderer != null)
                {
                    sectionRenderer.material.color = GetColor(liquidSections[i]);
                }

                spawnedVisuals.Add(newSection);
            }
        }

        private void ClearVisuals()
        {
            for (int i = spawnedVisuals.Count - 1; i >= 0; i--)
            {
                if (spawnedVisuals[i] != null)
                {
                    Destroy(spawnedVisuals[i]);
                }
            }

            spawnedVisuals.Clear();
        }

        private Color GetColor(LiquidColor liquidColor)
        {
            switch (liquidColor)
            {
                case LiquidColor.Red:
                    return Color.red;
                case LiquidColor.Blue:
                    return Color.blue;
                case LiquidColor.Green:
                    return Color.green;
                case LiquidColor.Yellow:
                    return Color.yellow;
                case LiquidColor.Purple:
                    return new Color(0.5f, 0f, 1f);
                case LiquidColor.Orange:
                    return new Color(1f, 0.5f, 0f);
                default:
                    return Color.clear;
            }
        }

        public void SetHoverHighlight(bool isHovering)
        {
            if (highlightRenderers == null)
            {
                return;
            }

            Color targetColor = isHovering ? hoverHighlightColor : normalHighlightColor;

            foreach (Renderer rend in highlightRenderers)
            {
                if (rend != null)
                {
                    rend.material.color = targetColor;
                }
            }
        }

        public void SetSelectedHighlight(bool isSelected)
        {
            if (highlightRenderers == null)
            {
                return;
            }

            Color targetColor = isSelected ? selectedHighlightColor : normalHighlightColor;

            foreach (Renderer rend in highlightRenderers)
            {
                if (rend != null)
                {
                    rend.material.color = targetColor;
                }
            }
        }

        public void LiftAsSelected()
        {
            StopAllCoroutines();
            StartCoroutine(MoveToLocalPosition(originalLocalPosition + Vector3.up * selectedLiftHeight));
        }

        public void ReturnToOriginalPose()
        {
            StopAllCoroutines();
            StartCoroutine(ReturnToOriginalPoseRoutine());
        }

        public IEnumerator PlayPourAnimationToward(LiquidSortTube destinationTube, System.Action onPourMoment)
        {
            if (destinationTube == null)
            {
                yield break;
            }

            StopAllCoroutines();

            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;

            Vector3 targetPosition = destinationTube.transform.position + Vector3.up * pourHoverHeight;
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, pourTiltAngle);

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * animationSpeed;

                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                yield return null;
            }

            onPourMoment?.Invoke();

            yield return new WaitForSeconds(0.25f);

            yield return ReturnToOriginalPoseRoutine();

            // Final safety refresh after tube is upright again.
            RefreshVisuals();
        }

        private IEnumerator MoveToLocalPosition(Vector3 targetLocalPosition)
        {
            Vector3 startPosition = transform.localPosition;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * animationSpeed;
                transform.localPosition = Vector3.Lerp(startPosition, targetLocalPosition, t);
                yield return null;
            }

            transform.localPosition = targetLocalPosition;
        }

        private IEnumerator ReturnToOriginalPoseRoutine()
        {
            Vector3 startPosition = transform.localPosition;
            Quaternion startRotation = transform.localRotation;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * animationSpeed;

                transform.localPosition = Vector3.Lerp(startPosition, originalLocalPosition, t);
                transform.localRotation = Quaternion.Slerp(startRotation, originalLocalRotation, t);

                yield return null;
            }

            transform.localPosition = originalLocalPosition;
            transform.localRotation = originalLocalRotation;
        }
    }
}