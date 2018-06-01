using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
     //checks if the Player is at the checkpoint
     void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //refreshes the buff
            MainCharControls.buff = 0;
        }
    }
}
