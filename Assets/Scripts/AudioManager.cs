using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Source")]
    public AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip normalMusic;
    public AudioClip bossMusic;

    [Header("Boss Settings")]
    public int bossLevel = 5;

    private AudioClip currentClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMusicForLevel(int currentLevel)
    {
        AudioClip targetClip = currentLevel >= bossLevel ? bossMusic : normalMusic;

        if (targetClip != null && currentClip != targetClip)
        {
            currentClip = targetClip;
            musicSource.clip = currentClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void MuteMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = true;
        }
    }

    public void UnmuteMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = false;
        }
    }
}