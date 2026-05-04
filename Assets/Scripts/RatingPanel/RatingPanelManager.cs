using System;
using DataLogging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RatingPanel
{
    public class RatingPanelManager : MonoBehaviour
    {
        public enum RatingStage
        {
            PreTask,
            PostTask
        }

        public enum RatingQuestion
        {
            Stress,
            Calmness,
            Mood,
            Pressure,
            Difficulty
        }

        [Header("Canvas Root")]
        public GameObject ratingCanvasRoot;

        [Header("Pre-Task Placement")]
        public Transform preTaskPanelLocation;
        public bool useOriginalScenePositionForPreTask = true;

        [Header("Post-Task Face User Placement")]
        public Transform cameraTransform;
        public float distanceFromCamera = 1.6f;
        public float verticalOffset = -0.05f;
        public bool placePostTaskInFrontOfCamera = true;

        [Header("Titles")]
        public GameObject preTaskTitleObject;
        public GameObject postTaskTitleObject;

        [Header("Question Rows")]
        public GameObject stressRow;
        public GameObject calmnessRow;
        public GameObject moodRow;
        public GameObject pressureRow;
        public GameObject difficultyRow;

        [Header("Buttons")]
        public RatingChoiceButton[] ratingButtons;
        public Button submitButton;

        [Header("Feedback Text")]
        public TextMeshProUGUI warningText;

        [Header("References")]
        public DataLogger dataLogger;
        public StudySessionManager studySessionManager;

        private RatingStage currentStage;
        private string currentTaskName = "None";
        private string currentTimePressureMode = "None";

        private int stressRating = -1;
        private int calmnessRating = -1;
        private int moodRating = -1;
        private int pressureRating = -1;
        private int difficultyRating = -1;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;

        public bool IsPanelOpen
        {
            get
            {
                return ratingCanvasRoot != null && ratingCanvasRoot.activeSelf;
            }
        }

        private void Awake()
        {
            if (ratingCanvasRoot != null)
            {
                originalPosition = ratingCanvasRoot.transform.position;
                originalRotation = ratingCanvasRoot.transform.rotation;
                originalScale = ratingCanvasRoot.transform.localScale;
            }

            if (submitButton != null)
            {
                submitButton.onClick.AddListener(SubmitRatings);
            }

            HidePanel();
        }

        private void OnDestroy()
        {
            if (submitButton != null)
            {
                submitButton.onClick.RemoveListener(SubmitRatings);
            }
        }

        public void ShowPreTaskRating(string timePressureMode)
        {
            currentStage = RatingStage.PreTask;
            currentTaskName = "None";
            currentTimePressureMode = timePressureMode;

            ResetRatings();
            SetPreTaskLayout();
            ShowPanelForPreTask();

            Debug.Log("Showing pre-task rating panel.");
        }

        public void ShowPostTaskRating(string taskName, string timePressureMode)
        {
            currentStage = RatingStage.PostTask;
            currentTaskName = taskName;
            currentTimePressureMode = timePressureMode;

            ResetRatings();
            SetPostTaskLayout();
            ShowPanelForPostTask();

            Debug.Log("Showing post-task rating panel for: " + taskName);
        }

        private void ShowPanelForPreTask()
        {
            PlacePanelAtPreTaskLocation();

            if (ratingCanvasRoot != null)
            {
                ratingCanvasRoot.SetActive(true);
            }
        }

        private void ShowPanelForPostTask()
        {
            PlacePanelInFrontOfCamera();

            if (ratingCanvasRoot != null)
            {
                ratingCanvasRoot.SetActive(true);
            }
        }

        private void PlacePanelAtPreTaskLocation()
        {
            if (ratingCanvasRoot == null)
                return;

            if (preTaskPanelLocation != null)
            {
                ratingCanvasRoot.transform.position = preTaskPanelLocation.position;
                ratingCanvasRoot.transform.rotation = preTaskPanelLocation.rotation;
                ratingCanvasRoot.transform.localScale = preTaskPanelLocation.localScale;
                return;
            }

            if (useOriginalScenePositionForPreTask)
            {
                ratingCanvasRoot.transform.position = originalPosition;
                ratingCanvasRoot.transform.rotation = originalRotation;
                ratingCanvasRoot.transform.localScale = originalScale;
            }
        }

        private void PlacePanelInFrontOfCamera()
        {
            if (!placePostTaskInFrontOfCamera)
            {
                return;
            }

            if (ratingCanvasRoot == null)
            {
                return;
            }

            if (cameraTransform == null)
            {
                Camera mainCamera = Camera.main;

                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }
            }

            if (cameraTransform == null)
            {
                Debug.LogWarning("RatingPanelManager: Missing cameraTransform. Cannot place panel in front of user.");
                return;
            }

            Vector3 forwardFlat = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;

            if (forwardFlat == Vector3.zero)
            {
                forwardFlat = cameraTransform.forward;
            }

            Vector3 targetPosition =
                cameraTransform.position +
                forwardFlat * distanceFromCamera +
                Vector3.up * verticalOffset;

            ratingCanvasRoot.transform.position = targetPosition;

            Vector3 directionToPanel = ratingCanvasRoot.transform.position - cameraTransform.position;
            directionToPanel.y = 0f;

            if (directionToPanel != Vector3.zero)
            {
                ratingCanvasRoot.transform.rotation = Quaternion.LookRotation(directionToPanel);
            }

            ratingCanvasRoot.transform.localScale = originalScale;
        }

        public void SelectRating(RatingQuestion question, int value)
        {
            switch (question)
            {
                case RatingQuestion.Stress:
                    stressRating = value;
                    break;

                case RatingQuestion.Calmness:
                    calmnessRating = value;
                    break;

                case RatingQuestion.Mood:
                    moodRating = value;
                    break;

                case RatingQuestion.Pressure:
                    pressureRating = value;
                    break;

                case RatingQuestion.Difficulty:
                    difficultyRating = value;
                    break;
            }

            RefreshButtonVisuals();

            if (warningText != null)
            {
                warningText.text = "";
            }

            Debug.Log("Selected " + question + ": " + value);
        }

        private void SubmitRatings()
        {
            if (!HasRequiredRatings())
            {
                if (warningText != null)
                {
                    warningText.text = "Please answer all visible questions before submitting.";
                }

                Debug.LogWarning("Rating submission blocked. Missing required ratings.");
                return;
            }

            RatingResult result = new RatingResult
            {
                ratingStage = currentStage.ToString(),
                taskName = currentTaskName,
                timePressureMode = currentTimePressureMode,
                stressRating = stressRating,
                calmnessRating = calmnessRating,
                moodRating = moodRating,
                pressureRating = pressureRating,
                difficultyRating = difficultyRating,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            if (dataLogger != null)
            {
                dataLogger.LogRatingResult(result);
            }
            else
            {
                Debug.LogWarning("RatingPanelManager is missing DataLogger reference.");
            }

            HidePanel();

            if (studySessionManager != null)
            {
                studySessionManager.OnRatingPanelSubmitted(currentStage);
            }

            Debug.Log("Submitted rating panel: " + currentStage);
        }

        private bool HasRequiredRatings()
        {
            if (currentStage == RatingStage.PreTask)
            {
                return stressRating > 0 &&
                       calmnessRating > 0 &&
                       moodRating > 0;
            }

            if (currentStage == RatingStage.PostTask)
            {
                return stressRating > 0 &&
                       pressureRating > 0 &&
                       difficultyRating > 0;
            }

            return false;
        }

        private void SetPreTaskLayout()
        {
            SetObjectActive(preTaskTitleObject, true);
            SetObjectActive(postTaskTitleObject, false);

            SetObjectActive(stressRow, true);
            SetObjectActive(calmnessRow, true);
            SetObjectActive(moodRow, true);

            SetObjectActive(pressureRow, false);
            SetObjectActive(difficultyRow, false);

            if (warningText != null)
            {
                warningText.text = "";
            }
        }

        private void SetPostTaskLayout()
        {
            SetObjectActive(preTaskTitleObject, false);
            SetObjectActive(postTaskTitleObject, true);

            SetObjectActive(stressRow, true);
            SetObjectActive(calmnessRow, false);
            SetObjectActive(moodRow, false);

            SetObjectActive(pressureRow, true);
            SetObjectActive(difficultyRow, true);

            if (warningText != null)
            {
                warningText.text = "";
            }
        }

        private void ResetRatings()
        {
            stressRating = -1;
            calmnessRating = -1;
            moodRating = -1;
            pressureRating = -1;
            difficultyRating = -1;

            RefreshButtonVisuals();
        }

        private void RefreshButtonVisuals()
        {
            if (ratingButtons == null)
            {
                return;
            }

            foreach (RatingChoiceButton ratingButton in ratingButtons)
            {
                if (ratingButton == null)
                {
                    continue;
                }

                bool selected = false;

                switch (ratingButton.question)
                {
                    case RatingQuestion.Stress:
                        selected = ratingButton.value == stressRating;
                        break;

                    case RatingQuestion.Calmness:
                        selected = ratingButton.value == calmnessRating;
                        break;

                    case RatingQuestion.Mood:
                        selected = ratingButton.value == moodRating;
                        break;

                    case RatingQuestion.Pressure:
                        selected = ratingButton.value == pressureRating;
                        break;

                    case RatingQuestion.Difficulty:
                        selected = ratingButton.value == difficultyRating;
                        break;
                }

                ratingButton.SetSelectedVisual(selected);
            }
        }

        private void HidePanel()
        {
            if (ratingCanvasRoot != null)
            {
                ratingCanvasRoot.SetActive(false);
            }
        }

        private void SetObjectActive(GameObject obj, bool active)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }
    }
}