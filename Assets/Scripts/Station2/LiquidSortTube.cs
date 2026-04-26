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

        private readonly List<LiquidColor> liquidSections = new List<LiquidColor>();
        private readonly List<GameObject> spawnedVisuals = new List<GameObject>();

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

                GameObject newSection = Instantiate(liquidSectionPrefab, slots[i].position, Quaternion.identity, slots[i]);
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
    }
}