using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    public GameObject menuButton;
    public GameObject Producer;
    public GameObject Introduction;
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void OpenProducer()
    {
        menuButton.SetActive(false);
        Producer.SetActive(true);
    }

    public void CloseProducer()
    {
        menuButton.SetActive(true);
        Producer.SetActive(false);
    }

    public void OpenIntroduction()
    {
        menuButton.SetActive(false);
        Introduction.SetActive(true);
    }

    public void CloseIntroduction()

    {
        menuButton.SetActive(true);
        Introduction.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
