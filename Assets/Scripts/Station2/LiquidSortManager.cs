using UnityEngine;

namespace Station2
{
    public class LiquidSortManager : MonoBehaviour
    {
        [Header("All Tubes")]
        public LiquidSortTube[] tubes;

        private LiquidSortTube selectedTube;

        private int pourCount;
        private int illegalPourCount;

        public void SelectTube(LiquidSortTube tube)
        {
            if (tube == null)
            {
                return;
            }

            if (selectedTube == null)
            {
                selectedTube = tube;
                Debug.Log("Selected source tube: " + tube.name);
                return;
            }

            if (selectedTube == tube)
            {
                Debug.Log("Deselected tube: " + tube.name);
                selectedTube = null;
                return;
            }

            TryPour(selectedTube, tube);
            selectedTube = null;
        }

        private void TryPour(LiquidSortTube sourceTube, LiquidSortTube destinationTube)
        {
            if (sourceTube.CanPourInto(destinationTube))
            {
                int amountPoured = sourceTube.PourInto(destinationTube);
                pourCount++;

                Debug.Log("Legal pour: " + sourceTube.name + " → " + destinationTube.name +
                          " | Sections poured: " + amountPoured +
                          " | Total pours: " + pourCount);

                CheckWin();
            }
            else
            {
                illegalPourCount++;

                Debug.Log("Illegal pour: " + sourceTube.name + " → " + destinationTube.name +
                          " | Illegal count: " + illegalPourCount);
            }
        }

        private void CheckWin()
        {
            foreach (LiquidSortTube tube in tubes)
            {
                if (!tube.IsSolved())
                {
                    return;
                }
            }

            Debug.Log("Liquid Sort solved!");
        }

        public int GetPourCount()
        {
            return pourCount;
        }

        public int GetIllegalPourCount()
        {
            return illegalPourCount;
        }

        public void ResetCounts()
        {
            pourCount = 0;
            illegalPourCount = 0;
            selectedTube = null;
        }
    }
}