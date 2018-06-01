using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{   //Script used to rotate the enemy healthbar to the camera
	// Update is called once per frame
	void LateUpdate ()
    {
        transform.rotation = Camera.main.transform.rotation;
	}
}
