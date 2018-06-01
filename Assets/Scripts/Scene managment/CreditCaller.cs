using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditCaller : MonoBehaviour {
    
    //Script used for the credits 
    private BoxCollider myCollider;
    private Color loadToColor = Color.black;

    private void Start()
    {
        myCollider = this.GetComponent<BoxCollider>();
    }

    //Checks through a collider trigger if the player reached the end spot in the map then switches to credits
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Initiate.Fade("Credits", loadToColor, 1f);
        }
    }
}
