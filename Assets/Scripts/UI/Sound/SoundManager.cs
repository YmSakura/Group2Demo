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

    //��Ч��Ƶ����
    public void AudioPlay(string objectName, string operation)
    {
        SFXAudioSource.clip = AudioName(objectName,operation);
        SFXAudioSource.Play();
    }
    
    //objectName��Ϊplayer��boss����Сд��
    //operation��Ϊhurt,attack,defence
    //����������Ͳ�����Ϊ�����Ӧ��Ƶ
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
    
    //�������ֲ���
    public void MusicPlay()
    {
        MusicAudioSource.clip = BackgroundMusic;
        MusicAudioSource.Play();
    }

}
