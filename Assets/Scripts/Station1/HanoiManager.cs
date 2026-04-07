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

        private void Start()
        {
            InitializePuzzle();
            RefreshAllGrabStates();
        }

        public void InitializePuzzle()
        {
            // Clear any old peg references
            foreach (Disk disk in startingDisks)
            {
                if (disk.currentPeg != null)
                {
                    disk.currentPeg.RemoveDisk(disk);
                }
            }

            // Put all starting disks on Peg1
            for (int i = 0; i < startingDisks.Length; i++)
            {
                peg1.SnapDiskToPeg(startingDisks[i]);
            }
        }
        
        public void RefreshAllGrabStates()
        {
            peg1.UpdateGrabState();
            peg2.UpdateGrabState();
            peg3.UpdateGrabState();
        }
    }
}