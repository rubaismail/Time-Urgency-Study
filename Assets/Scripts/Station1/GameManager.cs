using UnityEngine;

namespace Station1
{
    public class GameManager : MonoBehaviour
    {
        public bool gameStarted = false;

        [Header("References")]
        public MonoBehaviour movementScript;
        public HanoiManager hanoiManager;
        public Disk[] allDisks;

        private void Start()
        {
            DisableAllDiskGrabs();
        }

        public void StartGame()
        {
            if (gameStarted)
                return;

            Debug.Log("START BUTTON PRESSED");

            gameStarted = true;

            if (movementScript != null)
                movementScript.enabled = false;

            if (hanoiManager != null)
            {
                hanoiManager.enabled = true;
                hanoiManager.InitializePuzzle();
                hanoiManager.RefreshAllGrabStates();
            }

            Debug.Log("Task 1 Started!");
        }

        private void DisableAllDiskGrabs()
        {
            if (allDisks == null)
                return;

            foreach (Disk disk in allDisks)
            {
                if (disk != null)
                    disk.SetGrabbable(false);
            }
        }
    }
}