using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    //Audio Sources
    public AudioSource sEffectSource;
    public AudioSource musicSource;
    
    // Player Audio Clips
    public AudioClip[] pFootsteps;
    public AudioClip[] pAtk1;
    public AudioClip[] pAtk2;
    public AudioClip[] pJump;
    public AudioClip[] pLand;
    public AudioClip[] drinkPotion;
    public AudioClip pHurt;
    public AudioClip pDeath;

    // Enemy Audio Clips
    public AudioClip flyerAttack;
    public AudioClip flyerFlap;
    public AudioClip flyerHurt;
    public AudioClip flyerDeath;
    public AudioClip skeleAttack;
    public AudioClip skeleWalk;
    public AudioClip skeleHurt;
    public AudioClip skeleDeath;
    public AudioClip sporeAttack;
    public AudioClip sporeWalk;
    public AudioClip sporeHurt;
    public AudioClip sporeDeath;
    public AudioClip goblinAttack;
    public AudioClip goblinWalk;
    public AudioClip goblinHurt;
    public AudioClip goblinDeath;

    // Music Clips
    public AudioClip titleMusic;
    public AudioClip[] arenaMusic;
    public AudioClip gameOverMusic;
    public int songCounter = 0;

    // Misc Clips
    public AudioClip[] waveImpactSounds;
    public AudioClip gameOverLaugh;
    public AudioClip specialClick;

    // Singleton Variable
    public static SoundController sC;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject); // Persist between scenes
        // Assign singleton - destroy all duplicates in existence
        if (sC == null) sC = this;
        else Destroy(gameObject);
    }

    public void PlayMusic(AudioClip clip, float volume) // Plays music on music audiosource
    {
        musicSource.volume = volume;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void Play(AudioSource audioSource, AudioClip[] audioClips, float volume) // Plays random clip from array on specified audiosource
    {
        AudioClip clip = GetRandomClip(audioClips);
        audioSource.PlayOneShot(clip, volume);
    }

    public void Play(AudioSource audioSource, AudioClip audioClip, float volume) // Plays one shot clip on specified audiosource
    {
        audioSource.PlayOneShot(audioClip, volume);
    }

    public void Play(AudioSource audioSource, Vector3 position, AudioClip[] audioClips, float volume) // Plays random clip from array on specified audiosource at specified position
    {
        transform.position = position;
        AudioClip clip = GetRandomClip(audioClips);
        audioSource.PlayOneShot(clip, volume);
    }

    public void Play(AudioSource audioSource, Vector3 position, AudioClip audioClip, float volume) // Plays clip on specified audiosource at specified position
    {
        transform.position = position;
        audioSource.PlayOneShot(audioClip, volume);
    }

    public AudioClip GetRandomClip(AudioClip[] audioClips) // Gets a random audioclip from an audioclip array
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
