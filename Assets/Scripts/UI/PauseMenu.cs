using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject Insrtuction,PausePanel,QuitConfirm;
    public AudioMixer AudioMixer;


    public void OpenInsrtuction()
    {
        PausePanel.SetActive(false);
        Insrtuction.SetActive(true);
    }

    public void CloseInsrtuction()

    {
        PausePanel.SetActive(true);
        Insrtuction.SetActive(false);
    }

    public void OpenQuitConfirm()
    {
        QuitConfirm.SetActive(true);
    }

    public void CloseQuitConfirm()

    {
        QuitConfirm.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioMixer.SetFloat("Pitch", 0f);
    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1.0f;
        AudioMixer.SetFloat("Pitch", 1f);
    }
    public void SetMainVolume(float value)
    {
        AudioMixer.SetFloat("Main", value);
    }

    public void SetMusicVolume(float value)
    {
        AudioMixer.SetFloat("Music", value);
    }

    public void SetSFXVolume(float value)
    {
        AudioMixer.SetFloat("SFX", value);
    }
}
