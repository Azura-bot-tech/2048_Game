using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource sfxSource;

    public AudioClip ingameMusic;
    public AudioClip mergeSound;

    public AudioClip gameOverSound;

    private void Start()
    {
        backgroundMusic.clip = ingameMusic;
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }

    public void PlaySfx(AudioClip sfxClip)
    {
        sfxSource.clip = sfxClip;
        sfxSource.PlayOneShot(sfxClip);
    }

    public void StopPlayingSfx(AudioClip sfxClip)
    {
        sfxSource.clip = sfxClip;
        sfxSource.Stop();
    }

    public void StopInGameMusic()
    {
        backgroundMusic.Stop();
    }

    public void PlayInGameMusic()
    {
        backgroundMusic.clip = ingameMusic;
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }
}
