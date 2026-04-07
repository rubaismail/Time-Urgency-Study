using UnityEngine;

namespace Station1
{
    public class GameManager : MonoBehaviour
    {
        public bool gameStarted = false;

        public MonoBehaviour movementScript; // assign your movement component
        public HanoiManager hanoiManager;

        public void StartGame()
        {
            Debug.Log("START BUTTON PRESSED");

            gameStarted = true;

            // Disable movement
            if (movementScript != null)
                movementScript.enabled = false;

            // Enable puzzle interaction
            hanoiManager.enabled = true;
            
            hanoiManager.InitializePuzzle();
            hanoiManager.RefreshAllGrabStates();

            Debug.Log("Task 1 Started!");
        }
    }
}