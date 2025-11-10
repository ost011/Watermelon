using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }

            return instance;
        }
    }

    public AudioSource bgmSource;
    public AudioSource fxSource;
    public AudioSource fxClickSource;

    [Space]
    public AudioClip[] clipBGMs; // 0 tutorial, 1 lobby, 2 slm

    [Space]
    public AudioClip clipClick;
    [Tooltip("0 : 놓을 때, 1 : 부딪힐 때, 2 : 합쳐질 때")]
    public AudioClip[] clipsFx;

    public void Init()
    {
        //var sfxIndex = PlayerPrefs.GetInt(Constants.KEY_STR_SFX_ON_OFF, 1);
        //var bgmIndex = PlayerPrefs.GetInt(Constants.KEY_STR_BGM_ON_OFF, 1);

        //SetSFXOnOffState(sfxIndex);
        //SetBGMOnOffState(bgmIndex);
        
        //CustomDebug.Log($"sfxIndex : {sfxIndex}, bgmIndex : {bgmIndex}");
        
        PlayBGM(0);
    }

    public void PlayBGM(int bgmIndex)
    {
        StopBGM();

        var clip = this.clipBGMs[bgmIndex];

        bgmSource.clip = clip;

        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();

            bgmSource.clip = null;
        }
    }

    public void PlaySoundClip(EnumSets.InGameSoundClipType soundType)
    {
        var soundClipIndex = (int)soundType;

        fxSource.PlayOneShot(clipsFx[soundClipIndex]);
    }

    public void PlayClickSound()
    {
        fxClickSource.PlayOneShot(this.clipClick);
    }

    public void ModerateSFXToggle(Toggle toggle)
    {
        var index = 0;

        if (toggle.isOn)
        {
            index = 1;
        }

        SetSFXOnOffState(index);
        SaveSFXEnabledState(index);
    }

    public void ModerateBGMToggle(Toggle toggle)
    {
        var index = 0;

        if (toggle.isOn)
        {
            index = 1;
        }

        SetBGMOnOffState(index);
        SaveBGMEnabledState(index);
    }

    public void SetSFXOnOffState(int index)
    {
        var isEnabled = false;
        
        if(index == 1)
        {
            isEnabled = true;
        }

        this.fxSource.mute = !isEnabled;
    }

    public void SetBGMOnOffState(int index)
    {
        var isEnabled = false;

        if (index == 1)
        {
            isEnabled = true;
        }

        this.bgmSource.mute = !isEnabled;
    }

    public void SaveSFXEnabledState(int index)
    {
        PlayerPrefs.SetInt(Constants.KEY_STR_SFX_ON_OFF, index);
        PlayerPrefs.Save();
    }

    public void SaveBGMEnabledState(int index)
    {
        PlayerPrefs.SetInt(Constants.KEY_STR_BGM_ON_OFF, index);
        PlayerPrefs.Save();
    }
}
