using UnityEngine;

public class NpcUrgingController : MonoBehaviour
{
    public enum AnimationTrigger
    {
        Talk,
        UrgentTalk,
        Yell,
        Encourage
    }

    [System.Serializable]
    public class NpcUrgingCue
    {
        [Header("Cue Info")]
        public string cueName;

        [Header("When To Play")]
        public bool playAtTaskStart = false;
        public float remainingTimeThreshold = 60f;

        [Header("Audio")]
        public AudioClip voiceClip;

        [Header("Animation")]
        public AnimationTrigger animationTrigger = AnimationTrigger.Talk;

        [HideInInspector] public bool hasPlayed = false;
    }

    [Header("NPC Root")]
    public GameObject npcRoot;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Animation")]
    public Animator animator;

    [Header("Animator Trigger Names")]
    public string talkTriggerName = "Talk";
    public string urgentTalkTriggerName = "UrgentTalk";
    public string yellTriggerName = "Yell";
    public string encourageTriggerName = "Encourage";

    [Header("Urging Cues")]
    public NpcUrgingCue[] cues;

    private bool isRunning = false;

    private void Awake()
    {
        HideNpc();
        ResetCues();
    }

    public void StartUrging(float timeLimit)
    {
        Debug.Log("START URGING CALLED on: " + gameObject.name);

        isRunning = true;
        ResetCues();
        ShowNpc();

        Debug.Log("NPC Root Active After ShowNpc: " + npcRoot.activeSelf);

        PlayStartCues();

        Debug.Log("NPC urging started.");
    }

    public void UpdateUrging(float timeRemaining)
    {
        if (!isRunning)
            return;

        if (cues == null)
            return;

        foreach (NpcUrgingCue cue in cues)
        {
            if (cue == null)
                continue;

            if (cue.hasPlayed)
                continue;

            if (cue.playAtTaskStart)
                continue;

            if (timeRemaining <= cue.remainingTimeThreshold)
            {
                PlayCue(cue);
            }
        }
    }

    public void StopUrging()
    {
        isRunning = false;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        HideNpc();

        Debug.Log("NPC urging stopped.");
    }

    public void ResetToIdle()
    {
        isRunning = false;
        ResetCues();

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        HideNpc();
    }

    private void PlayStartCues()
    {
        if (cues == null)
            return;

        foreach (NpcUrgingCue cue in cues)
        {
            if (cue == null)
                continue;

            if (cue.hasPlayed)
                continue;

            if (cue.playAtTaskStart)
            {
                PlayCue(cue);
            }
        }
    }

    private void PlayCue(NpcUrgingCue cue)
    {
        if (cue == null)
            return;

        cue.hasPlayed = true;

        PlayAnimation(cue.animationTrigger);
        PlayAudio(cue.voiceClip);

        Debug.Log("NPC cue played: " + cue.cueName);
    }

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("NpcUrgingController is missing AudioSource.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("NpcUrgingController cue is missing an AudioClip.");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    private void PlayAnimation(AnimationTrigger trigger)
    {
        if (animator == null)
        {
            Debug.LogWarning("NpcUrgingController is missing Animator.");
            return;
        }

        switch (trigger)
        {
            case AnimationTrigger.Talk:
                animator.SetTrigger(talkTriggerName);
                break;

            case AnimationTrigger.UrgentTalk:
                animator.SetTrigger(urgentTalkTriggerName);
                break;

            case AnimationTrigger.Yell:
                animator.SetTrigger(yellTriggerName);
                break;

            case AnimationTrigger.Encourage:
                animator.SetTrigger(encourageTriggerName);
                break;
        }
    }

    private void ShowNpc()
    {
        if (npcRoot != null)
        {
            npcRoot.SetActive(true);
        }
    }

    private void HideNpc()
    {
        if (npcRoot != null)
        {
            npcRoot.SetActive(false);
        }
    }

    private void ResetCues()
    {
        if (cues == null)
            return;

        foreach (NpcUrgingCue cue in cues)
        {
            if (cue != null)
            {
                cue.hasPlayed = false;
            }
        }
    }
}