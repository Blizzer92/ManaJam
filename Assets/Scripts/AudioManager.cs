using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    AudioSource sfxAudioSource;
    AudioSource musicAudioSource;

    public SoundKeyValue[] sfx;        
    public SoundKeyValue[] music;

    // set up the singleton
    private void Awake() 
    {
        if (instance == null)
        {   
            Init();         
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }        
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.S))
        {
            PlaySFX("TestSound");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlaySFX("Cricket");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopMusic();
        }*/
    }

    private void Init()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource = gameObject.AddComponent<AudioSource>();        
        
        EventManager.StartListening("PlayMusic", OnPlayMusic);
        EventManager.StartListening("PlaySFX", OnPlaySFX);
    }

    public void PlayMusic(string musicName)
    {
        SoundKeyValue musicSource = Array.Find(music, x => x.SoundName == musicName);
        if (musicSource != null)
        {
            if (musicAudioSource.isPlaying && musicAudioSource.clip == musicSource.AudioClip)
            {
                //musicAudioSource.time = 0f;
                return;
            }
            musicAudioSource.clip = musicSource.AudioClip;
            musicAudioSource.Play();
        } else
        {
            Debug.LogError("Music Source " + musicName + " not found!");
        }
    }

    public void StopMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    public void PlaySFX(string sfxName)
    {
        SoundKeyValue sfxSource = Array.Find(sfx, x => x.SoundName == sfxName);
        if (sfxSource != null)
        {
            sfxAudioSource.clip = sfxSource.AudioClip;
            sfxAudioSource.Play();
        } else
        {
            Debug.LogError("SFX Source " + sfxName + " not found!");
        }
    }        

    public void StopSFX()
    {
        if (sfxAudioSource.isPlaying)
        {
            sfxAudioSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicAudioSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxAudioSource.volume = volume;
    }

    public void SetAllVolume(float volume)
    {
        SetMusicVolume(volume);
        SetSFXVolume(volume);
    }

    private void OnPlayMusic(Dictionary<string, object> message)
    {
        object musicName;
        if (message.TryGetValue("MusicName", out musicName))
        {
            if (musicName is string)
            {
                PlayMusic((string)musicName);
            } else
            {
                Debug.LogError("OnPlayMusic: musicName='" + musicName + "' not a string!");
            }
        } else
        {
            Debug.LogError("OnPlayMusic: parameter 'MusicName' not found!");
        }
    }

    private void OnPlaySFX(Dictionary<string, object> message)
    {
        object sfxName;
        if (message.TryGetValue("SFXName", out sfxName))
        {
            if (sfxName is string)
            {
                PlaySFX((string)sfxName);
            }
            else
            {
                Debug.LogError("OnPlaySFX: sfxName='" + sfxName + "' not a string!");
            }
        }
        else
        {
            Debug.LogError("OnPlaySFX: parameter 'SFXName' not found!");
        }
    }

}

[System.Serializable]
public class SoundKeyValue
{
    public string SoundName;
    public AudioClip AudioClip;
}