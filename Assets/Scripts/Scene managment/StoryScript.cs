using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryScript : MonoBehaviour {
    //Script used to play the story
    public Text story;
    public Image storyBox;
    private Color loadToColor = Color.black;

    public Image[] imageList;
    public string[] textList;

    int sceneCounter = 0;
    
    //Plays the text for the story
	void Start ()
    {
        story.text = textList[sceneCounter];
        imageList[sceneCounter].gameObject.SetActive(true);
	}

    public void NextArc()
    {
        imageList[sceneCounter].gameObject.SetActive(false);
        sceneCounter = sceneCounter + 1;

        if (sceneCounter == 28)
        {
            LastArc();
        }

        else
        {
            story.text = textList[sceneCounter];
            imageList[sceneCounter].gameObject.SetActive(true);
        }
        
    }

    public void PevArc ()
    {
        imageList[sceneCounter].gameObject.SetActive(false);
        sceneCounter = sceneCounter - 1;
        story.text = textList[sceneCounter];
        imageList[sceneCounter].gameObject.SetActive(true);
    }
    
    //After the story is finished changes scenes and fades to black
    public void LastArc ()
    {   
        Initiate.Fade("firstOne", loadToColor, 1f);
    }

}
