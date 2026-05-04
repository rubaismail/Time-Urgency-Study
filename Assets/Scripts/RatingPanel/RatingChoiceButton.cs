using UnityEngine;
using UnityEngine.UI;

namespace RatingPanel
{
    public class RatingChoiceButton : MonoBehaviour
    {
        [Header("Rating Setup")]
        public RatingPanelManager manager;
        public RatingPanelManager.RatingQuestion question;
        public int value = 1;

        [Header("Selected Visual")]
        public float selectedScaleMultiplier = 1.18f;

        private Button button;
        private Vector3 originalScale;

        private void Awake()
        {
            button = GetComponent<Button>();
            originalScale = transform.localScale;

            if (button != null)
            {
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            if (manager == null)
            {
                Debug.LogWarning(name + ": Missing RatingPanelManager reference.");
                return;
            }

            manager.SelectRating(question, value);
        }

        public void SetSelectedVisual(bool selected)
        {
            transform.localScale = selected ? originalScale * selectedScaleMultiplier : originalScale;
        }
    }
}