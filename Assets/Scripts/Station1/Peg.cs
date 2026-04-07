using System.Collections.Generic;
using UnityEngine;

namespace Station1
{
    public class Peg : MonoBehaviour
    {
        [Header("References")]
        public Transform snapAnchor;

        [Header("Stack Settings")]
        public float diskHeightSpacing = 0.03f;

        private List<Disk> disksOnPeg = new List<Disk>();

        public int DiskCount
        {
            get { return disksOnPeg.Count; }
        }

        public Disk GetTopDisk()
        {
            if (disksOnPeg.Count == 0)
                return null;

            return disksOnPeg[disksOnPeg.Count - 1];
        }

        public bool IsTopDisk(Disk disk)
        {
            return GetTopDisk() == disk;
        }

        public bool CanPlaceDisk(Disk disk)
        {
            Disk topDisk = GetTopDisk();

            if (topDisk == null)
                return true;

            return disk.sizeRank < topDisk.sizeRank;
        }

        public Vector3 GetSnapPosition()
        {
            return snapAnchor.position + Vector3.up * (disksOnPeg.Count * diskHeightSpacing);
        }

        public void AddDisk(Disk disk)
        {
            if (!disksOnPeg.Contains(disk))
            {
                disksOnPeg.Add(disk);
                disk.currentPeg = this;
            }
        }

        public void RemoveDisk(Disk disk)
        {
            if (disksOnPeg.Contains(disk))
            {
                disksOnPeg.Remove(disk);
            }
        }

        public void SnapDiskToPeg(Disk disk)
        {
            Vector3 targetPosition = GetSnapPosition();

            disk.transform.position = targetPosition;
            disk.transform.rotation = Quaternion.identity;

            Rigidbody rb = disk.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            AddDisk(disk);
        }
        
        public void UpdateGrabState()
        {
            for (int i = 0; i < disksOnPeg.Count; i++)
            {
                Disk disk = disksOnPeg[i];
                bool isTop = (i == disksOnPeg.Count - 1);
                disk.SetGrabbable(isTop);
            }
        }
    }
}