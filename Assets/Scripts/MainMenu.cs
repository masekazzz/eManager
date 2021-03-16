using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void QuitGame() => Application.Quit();

    void Update()
    {
        if (!(File.Exists("Save/Save.ser")))
            GameObject.Find("LoadGameButton").GetComponent<Button>().interactable = false;
        else
            GameObject.Find("LoadGameButton").GetComponent<Button>().interactable = true;
    }

}
