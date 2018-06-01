using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    //Script uded for the Menu. Holds the button functions which is to load to the story scene or quit the game
    public void NewGame()
    {
        SceneManager.LoadScene("Story scene");
    }

    public void  QuitGame ()
    {
        Application.Quit();
    }
}
