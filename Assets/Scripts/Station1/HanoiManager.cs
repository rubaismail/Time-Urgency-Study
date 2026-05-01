using UnityEngine;

namespace Station1
{
    public class HanoiManager : MonoBehaviour
    {
        [Header("Peg References")]
        public Peg peg1;
        public Peg peg2;
        public Peg peg3;

        [Header("Starting Disks (largest to smallest)")]
        public Disk[] startingDisks;

        public int moveCount = 0;
        public int illegalMoveCount = 0;
        public int invalidDropCount = 0;

        public void InitializePuzzle()
        {
            ClearAllPegs();

            for (int i = 0; i < startingDisks.Length; i++)
            {
                peg1.SnapDiskToPeg(startingDisks[i]);
            }

            moveCount = 0;
            illegalMoveCount = 0;
            invalidDropCount = 0;
        }

        public void ClearAllPegs()
        {
            foreach (Disk disk in startingDisks)
            {
                if (disk.currentPeg != null)
                {
                    disk.currentPeg.RemoveDisk(disk);
                    disk.currentPeg = null;
                }
            }
        }

        public bool CheckWin()
        {
            return peg3.diskCount == startingDisks.Length;
        }

        public void RefreshAllGrabStates()
        {
            peg1.UpdateGrabState();
            peg2.UpdateGrabState();
            peg3.UpdateGrabState();
        }
    }
}