using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;


[DefaultExecutionOrder(-999)]
public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public AudioMixerGroup MusicGroup;
    public AudioMixerGroup GasGroup;
    public AudioMixerGroup PopGroup;
    public AudioMixerGroup GamePlayMusicGroup;
    public AudioMixerGroup GameOverMusicGroup;
    public AudioMixerGroup BoilingWaterGroup;
    
    public AudioMixerSnapshot mainMenuMixer;
    public AudioMixerSnapshot gameplayMixer;
    public AudioMixerSnapshot endGameplayMixer;

    
    private static AudioManager  s_Instance;
    public static AudioManager Instance
    {
        get
        {
            return s_Instance;
        }

        private set => s_Instance = value;
    }
    
    
    private void Awake()
    {
        if (s_Instance == this)
        {
            return;
        }

        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }

    public void TrasitionToState(string state, float transitionDelay)
    {
        switch (state)
        {
            case "MainMenu":
                mainMenuMixer.TransitionTo(transitionDelay);
                break;
            case "Gameplay":
                gameplayMixer.TransitionTo(transitionDelay);
                break;
            case "EndGame":
                endGameplayMixer.TransitionTo(transitionDelay);
                break;
        }
    }

    public void PlaySFXSource(AudioSource source)
    {
        source.outputAudioMixerGroup = PopGroup;
        source.Play();
    }

    public void PlayMusicSource(AudioSource source)
    {
        source.outputAudioMixerGroup = MusicGroup;   
        source.Play();
    }

    public void PlayGasSource(AudioSource source)
    {
        source.outputAudioMixerGroup = GasGroup;
        source.loop = true;
        source.Play();
    }

    public void PlayGamePlaySource(AudioSource source)
    {
        gameplayMixer.TransitionTo(1.0f);
        source.outputAudioMixerGroup = GamePlayMusicGroup;
        source.loop = true;
        source.Play();
    }

    public void PlayGameOverSource(AudioSource source)
    {
        source.outputAudioMixerGroup = GameOverMusicGroup;
        source.loop = true;
        source.Play();
        endGameplayMixer.TransitionTo(1.0f);
    }
    
    public void PlayBoilingWaterSource(AudioSource source)
    {
        source.outputAudioMixerGroup = BoilingWaterGroup;
        source.loop = true;
        source.Play();
    }
}
