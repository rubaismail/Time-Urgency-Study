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

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Countdown - Minute Warnings")]
    public AudioClip fiveMinutesClip;
    public AudioClip fourMinutesClip;
    public AudioClip threeMinutesClip;
    public AudioClip twoMinutesClip;
    public AudioClip oneMinuteClip;

    [Header("Audio Countdown - Final Warnings")]
    public AudioClip thirtySecondsClip;

    [Header("Audio Countdown - Final 10 Seconds")]
    public AudioClip tenClip;
    public AudioClip nineClip;
    public AudioClip eightClip;
    public AudioClip sevenClip;
    public AudioClip sixClip;
    public AudioClip fiveClip;
    public AudioClip fourClip;
    public AudioClip threeClip;
    public AudioClip twoClip;
    public AudioClip oneClip;

    private bool playedFiveMinutes = false;
    private bool playedFourMinutes = false;
    private bool playedThreeMinutes = false;
    private bool playedTwoMinutes = false;
    private bool playedOneMinute = false;
    private bool playedThirtySeconds = false;

    private bool playedTen = false;
    private bool playedNine = false;
    private bool playedEight = false;
    private bool playedSeven = false;
    private bool playedSix = false;
    private bool playedFive = false;
    private bool playedFour = false;
    private bool playedThree = false;
    private bool playedTwo = false;
    private bool playedOne = false;

    public void SetMode(Mode newMode)
    {
        mode = newMode;
        ResetToIdle();
    }

    public void StartPressure(Mode newMode, float timeLimit)
    {
        mode = newMode;

        ResetAudioFlags();

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
            visualTimerRoot.SetActive(false);
        }

        if (timerText != null)
        {
            timerText.text = "";
        }

        if (npcTextRoot != null)
        {
            npcTextRoot.SetActive(false);
        }
    }

    public void ResetToIdle()
    {
        ResetAudioFlags();

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

    private void ResetAudioFlags()
    {
        playedFiveMinutes = false;
        playedFourMinutes = false;
        playedThreeMinutes = false;
        playedTwoMinutes = false;
        playedOneMinute = false;
        playedThirtySeconds = false;

        playedTen = false;
        playedNine = false;
        playedEight = false;
        playedSeven = false;
        playedSix = false;
        playedFive = false;
        playedFour = false;
        playedThree = false;
        playedTwo = false;
        playedOne = false;
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
            int totalSeconds = Mathf.CeilToInt(timeRemaining);

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timerText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
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

        TryPlayMinuteCue(timeRemaining, timeLimit);
        TryPlayFinalCue(timeRemaining);
    }

    private void TryPlayMinuteCue(float timeRemaining, float timeLimit)
    {
        if (!playedFiveMinutes && timeLimit >= 300f && timeRemaining <= 300f)
        {
            playedFiveMinutes = true;
            PlayClip(fiveMinutesClip);
        }

        if (!playedFourMinutes && timeLimit >= 240f && timeRemaining <= 240f)
        {
            playedFourMinutes = true;
            PlayClip(fourMinutesClip);
        }

        if (!playedThreeMinutes && timeLimit >= 180f && timeRemaining <= 180f)
        {
            playedThreeMinutes = true;
            PlayClip(threeMinutesClip);
        }

        if (!playedTwoMinutes && timeLimit >= 120f && timeRemaining <= 120f)
        {
            playedTwoMinutes = true;
            PlayClip(twoMinutesClip);
        }

        if (!playedOneMinute && timeLimit >= 60f && timeRemaining <= 60f)
        {
            playedOneMinute = true;
            PlayClip(oneMinuteClip);
        }

        if (!playedThirtySeconds && timeLimit >= 30f && timeRemaining <= 30f)
        {
            playedThirtySeconds = true;
            PlayClip(thirtySecondsClip);
        }
    }

    private void TryPlayFinalCue(float timeRemaining)
    {
        if (!playedTen && timeRemaining <= 10f)
        {
            playedTen = true;
            PlayClip(tenClip);
        }

        if (!playedNine && timeRemaining <= 9f)
        {
            playedNine = true;
            PlayClip(nineClip);
        }

        if (!playedEight && timeRemaining <= 8f)
        {
            playedEight = true;
            PlayClip(eightClip);
        }

        if (!playedSeven && timeRemaining <= 7f)
        {
            playedSeven = true;
            PlayClip(sevenClip);
        }

        if (!playedSix && timeRemaining <= 6f)
        {
            playedSix = true;
            PlayClip(sixClip);
        }

        if (!playedFive && timeRemaining <= 5f)
        {
            playedFive = true;
            PlayClip(fiveClip);
        }

        if (!playedFour && timeRemaining <= 4f)
        {
            playedFour = true;
            PlayClip(fourClip);
        }

        if (!playedThree && timeRemaining <= 3f)
        {
            playedThree = true;
            PlayClip(threeClip);
        }

        if (!playedTwo && timeRemaining <= 2f)
        {
            playedTwo = true;
            PlayClip(twoClip);
        }

        if (!playedOne && timeRemaining <= 1f)
        {
            playedOne = true;
            PlayClip(oneClip);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
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