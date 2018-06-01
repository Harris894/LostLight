using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    // Script used to randomize wolf howl sound in game
	public AudioSource howling;
	public AudioClip wolf;
	private int num;

	// Use this for initialization
	void Start ()
    {   //selects a random number
		num = Random.Range (25, 50);
		howling.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
		StartCoroutine (Howl ());
	}

	IEnumerator Howl()
    {   
        //plays random wolf howl after random number of seconds
		yield return new WaitForSeconds (num);
		howling.PlayOneShot (wolf);
	}
}
