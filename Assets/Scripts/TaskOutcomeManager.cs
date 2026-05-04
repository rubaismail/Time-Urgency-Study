using UnityEngine;
using TMPro;

public class TaskOutcomeMessageManager : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject messageRoot;

    [Header("Text")]
    public TextMeshProUGUI outcomeText;

    [Header("Messages")]
    public string successMessage = "Task Success!";
    public string failureMessage = "Time is up! Task Failed";

    private void Awake()
    {
        HideMessage();
    }

    public void ShowSuccessMessage()
    {
        ShowMessage(successMessage);
    }

    public void ShowFailureMessage()
    {
        ShowMessage(failureMessage);
    }

    public void ShowMessage(string message)
    {
        if (outcomeText != null)
        {
            outcomeText.text = message;
        }

        if (messageRoot != null)
        {
            messageRoot.SetActive(true);
        }
    }

    public void HideMessage()
    {
        if (messageRoot != null)
        {
            messageRoot.SetActive(false);
        }
    }
}