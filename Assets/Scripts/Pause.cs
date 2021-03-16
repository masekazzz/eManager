using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class Pause : MonoBehaviour
{
    private bool paused = false;
    public GameObject PauseMenu;
    public GameObject mailButton;
    public bool firstTime = true;
    public Toggle FullScreen;
    public TMP_Dropdown ScreenResolution;
    public AudioMixer audioMixer;
    public Manager manager;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameObject.Find("Game") == true)
        {
            if (!paused)
            {
                paused = true;
                PauseMenu.SetActive(true);
            }
            else
            {
                paused = false;
                PauseMenu.SetActive(false);
            }
        }
    }

    public void ToMainMenu()
    {
        paused = false;
        firstTime = true;
        Destroy(GameObject.Find("MailButton(Clone)"));
        GameObject.Find("RawImage").GetComponent<RawImage>().transform.position = new Vector3(0.1685725F, -1.173354F, 100);
    }

    public void HideMailButton()
    {
        if (firstTime)
        {
            mailButton = GameObject.Find("MailButton(Clone)");
            firstTime = false;
        }
        mailButton.SetActive(false);
    }

    public void RevealMailButton() => mailButton.SetActive(true);

    public void FullScreenToggler()
    {
        if (!FullScreen.isOn)
        {
            Screen.fullScreen = false;
            ScreenResolution.gameObject.SetActive(true);

        }
        else
        {
            Screen.fullScreen = true;
            ScreenResolution.gameObject.SetActive(false);
            Screen.SetResolution(1920, 1080, true);
        }
    }

    public void ScreenResolutionChange()
    {
        switch (ScreenResolution.value)
        {
            case (0):
                Screen.SetResolution(1920, 1080, true);
                Screen.fullScreen = false;
                break;
            case (1):
                Screen.SetResolution(1600, 900, true);
                Screen.fullScreen = false;
                break;
            case (2):
                Screen.SetResolution(1280, 720, true);
                Screen.fullScreen = false;
                break;
            case (3):
                Screen.SetResolution(960, 540, true);
                Screen.fullScreen = false;
                break;
            case (4):
                Screen.SetResolution(640, 360, true);
                Screen.fullScreen = false;
                break;
            case (5):
                Screen.SetResolution(321, 180, true);
                Screen.fullScreen = false;
                break;
        }
    }

    public void SetQuality(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);

    public void SetVolume(float volume) => audioMixer.SetFloat("Volume", volume);

    public bool firstTimeButtons = true;

    public void SetButtons()
    {
        if (firstTimeButtons)
        {
            for (int i = 0; i <= 7; i++)
            {
                int temp = i + 1;
                var smth = GameObject.Find("Icons").transform.Find($"Button ({i})");
                smth.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        GameObject.Find("RawImage").GetComponent<RawImage>().texture = smth.GetComponent<RawImage>().texture;
                        GameObject.Find("Icons").SetActive(false);
                        manager.iconPath = $"Icons/{temp}";
                    }
                );
            }
            firstTimeButtons = false;
        }
    }
}
