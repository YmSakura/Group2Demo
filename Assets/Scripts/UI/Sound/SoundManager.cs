using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Sound;
    //public AudioMixer audioMixer;


    [SerializeField] private AudioSource MusicAudioSource, PlayerSFXAudioSource, BossSFXAudioSource;
    [SerializeField] private AudioClip playerAttackAudio, playerHurtAudio, playerDefenceAudio;
    [SerializeField] private AudioClip bossAttackAudio, bossHurtAudio, bossAudio;
    [SerializeField]private AudioClip BackgroundMusic;
    void Awake()
    {
        Sound = this;
    }

    //玩家特效音频播放
    public void PlayerAudioPlay(string operation)
    {
        PlayerSFXAudioSource.clip = AudioName("player", operation);
        PlayerSFXAudioSource.Play();
    }

    //Boss特效音频播放
    public void BossAudioPlay(string operation)
    {
        BossSFXAudioSource.clip = AudioName("boss", operation);
        BossSFXAudioSource.Play();
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

    public void StopMusic()
    {
        MusicAudioSource.Stop();
       
    }

}
