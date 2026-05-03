using UnityEngine;
using TMPro;

public class TimePressureController : MonoBehaviour
{
    public enum Mode
    {
        VisualCountdown,
        AudioCountdown,
        NpcUrging
    }

    [Header("Mode")]
    public Mode mode = Mode.VisualCountdown;

    [Header("Start Instruction")]
    public GameObject startInstructionRoot;
    public TextMeshPro startInstructionText;
    public string startInstructionMessage = "Press Button To Start";

    [Header("Visual Countdown")]
    public GameObject visualTimerRoot;
    public TextMeshPro timerText;

    [Header("NPC Urging")]
    public GameObject npcTextRoot;
    public TextMeshPro npcText;

    [Header("Audio Countdown")]
    public AudioSource audioSource;
    public AudioClip warningClip;
    public AudioClip finalCountdownClip;

    private bool hasPlayedHalfwayWarning = false;
    private bool hasPlayedFinalWarning = false;

    public void SetMode(Mode newMode)
    {
        mode = newMode;
        ResetToIdle();
    }

    public void StartPressure(Mode newMode, float timeLimit)
    {
        mode = newMode;

        hasPlayedHalfwayWarning = false;
        hasPlayedFinalWarning = false;

        ShowStartInstruction(false);
        UpdatePressure(timeLimit, timeLimit, true, false);
    }

    public void UpdatePressure(float timeRemaining, float timeLimit, bool taskRunning, bool taskEnded)
    {
        ShowStartInstruction(!taskRunning && !taskEnded);

        UpdateVisualCountdown(timeRemaining, taskRunning, taskEnded);
        UpdateAudioCountdown(timeRemaining, timeLimit, taskRunning, taskEnded);
        UpdateNpcUrging(timeRemaining, timeLimit, taskRunning, taskEnded);
    }

    public void EndPressure(bool success)
    {
        ShowStartInstruction(false);

        if (visualTimerRoot != null)
        {
            visualTimerRoot.SetActive(mode == Mode.VisualCountdown);
        }

        if (timerText != null && mode == Mode.VisualCountdown)
        {
            timerText.text = success ? "Success!" : "Time Is Up!";
        }

        if (npcTextRoot != null)
        {
            npcTextRoot.SetActive(false);
        }
    }

    public void ResetToIdle()
    {
        hasPlayedHalfwayWarning = false;
        hasPlayedFinalWarning = false;

        ShowStartInstruction(true);

        if (visualTimerRoot != null)
        {
            visualTimerRoot.SetActive(mode == Mode.VisualCountdown);
        }

        if (timerText != null && mode == Mode.VisualCountdown)
        {
            timerText.text = "";
        }

        if (npcTextRoot != null)
        {
            npcTextRoot.SetActive(false);
        }
    }

    private void ShowStartInstruction(bool show)
    {
        if (startInstructionRoot != null)
        {
            startInstructionRoot.SetActive(show);
        }

        if (startInstructionText != null)
        {
            startInstructionText.text = startInstructionMessage;
        }
    }

    private void UpdateVisualCountdown(float timeRemaining, bool taskRunning, bool taskEnded)
    {
        if (visualTimerRoot != null)
        {
            visualTimerRoot.SetActive(mode == Mode.VisualCountdown && taskRunning && !taskEnded);
        }

        if (timerText == null)
            return;

        if (mode != Mode.VisualCountdown)
            return;

        if (taskRunning)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
        }
    }

    private void UpdateAudioCountdown(float timeRemaining, float timeLimit, bool taskRunning, bool taskEnded)
    {
        if (mode != Mode.AudioCountdown)
            return;

        if (!taskRunning || taskEnded)
            return;

        if (audioSource == null)
            return;

        if (!hasPlayedHalfwayWarning && timeRemaining <= timeLimit * 0.5f)
        {
            hasPlayedHalfwayWarning = true;

            if (warningClip != null)
            {
                audioSource.PlayOneShot(warningClip);
            }
        }

        if (!hasPlayedFinalWarning && timeRemaining <= 10f)
        {
            hasPlayedFinalWarning = true;

            if (finalCountdownClip != null)
            {
                audioSource.PlayOneShot(finalCountdownClip);
            }
        }
    }

    private void UpdateNpcUrging(float timeRemaining, float timeLimit, bool taskRunning, bool taskEnded)
    {
        if (npcTextRoot != null)
        {
            npcTextRoot.SetActive(mode == Mode.NpcUrging && taskRunning && !taskEnded);
        }

        if (mode != Mode.NpcUrging)
            return;

        if (!taskRunning || taskEnded)
            return;

        if (npcText == null)
            return;

        if (timeRemaining <= 10f)
        {
            npcText.text = "Hurry! You are almost out of time!";
        }
        else if (timeRemaining <= timeLimit * 0.5f)
        {
            npcText.text = "You need to move faster!";
        }
        else
        {
            npcText.text = "Keep going!";
        }
    }
}