using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject VN;
    public GameObject MainMenu;
    public GameObject settingsMenu;
    public AudioSource mainMenuMusic; 
    public AudioSource music;
    public DialogueSystem dialogueSystem;
    public GameObject settingsButton;

    public void StartGame()
    {
        mainMenuMusic.enabled = false;
        MainMenu.SetActive(false);
        VN.SetActive(true);
        dialogueSystem.Start();
        foreach (GameObject panel in dialogueSystem.endingPanels)
        {
            panel.SetActive(false);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Q");
    }

    public void OpenSettings()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void Settings()
    {
        bool enabled = settingsMenu.activeSelf;
        settingsMenu.SetActive(!enabled);

        if(enabled)
        {
            music.UnPause();
        }
        else
        {
            music.Pause();
        }
    }

    
}

