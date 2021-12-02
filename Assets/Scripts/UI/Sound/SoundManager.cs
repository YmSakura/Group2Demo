using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Sound;


    [SerializeField] private AudioSource MusicAudioSource, PlayerSFXAudioSource, BossSFXAudioSource;
    [SerializeField] private AudioClip playerAttackAudio, playerHurtAudio, playerDefenceAudio;
    [SerializeField] private AudioClip bossAttackAudio, bossHurtAudio, bossAudio;
    [SerializeField]private AudioClip BackgroundMusic;
    void Awake()
    {
        Sound = this;
    }

    //�����Ч��Ƶ����
    public void PlayerAudioPlay(string operation)
    {
        PlayerSFXAudioSource.clip = AudioName("player", operation);
        PlayerSFXAudioSource.Play();
    }

    //Boss��Ч��Ƶ����
    public void BossAudioPlay(string operation)
    {
        BossSFXAudioSource.clip = AudioName("boss", operation);
        BossSFXAudioSource.Play();
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
