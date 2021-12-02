using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Sound;


    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource MusicAudioSource, SFXAudioSource;
    [SerializeField] private AudioClip playerAttackAudio, playerHurtAudio, playerDefenceAudio;
    [SerializeField] private AudioClip bossAttackAudio, bossHurtAudio, bossAudio;
    [SerializeField]private AudioClip BackgroundMusic;
    void Awake()
    {
        Sound = this;
    }

    //特效音频播放
    public void AudioPlay(string objectName, string operation)
    {
        SFXAudioSource.clip = AudioName(objectName,operation);
        SFXAudioSource.Play();
    }
    
    //objectName分为player和boss（都小写）
    //operation分为hurt,attack,defence
    //输入对象名和操作行为输出对应音频
    public AudioClip AudioName(string objectName,string operation)
    {
        if(objectName == "player")
        {
            if (operation == "attack") return playerAttackAudio;
            else if (operation == "hurt") return playerHurtAudio;
            else if (operation =="defence")return playerDefenceAudio;
        }
        else if(objectName == "boss")
        {
            if (operation == "attack") return bossAttackAudio;
            else if (operation == "hurt") return bossHurtAudio;
            else if (operation == "audio") return bossAudio;
        }
        return null;
    }
    
    //背景音乐播放
    public void MusicPlay()
    {
        MusicAudioSource.clip = BackgroundMusic;
        MusicAudioSource.Play();
    }

}
