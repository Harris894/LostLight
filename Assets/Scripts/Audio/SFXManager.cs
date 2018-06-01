using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class SFXManager : MonoBehaviour
{   //
	public static SFXManager instance;
	public Sound[] sounds;
	// Use this for initialization
	void Awake ()
    {	//Checks if there is no instance of SFXManager, then sets this one as it
		//Otherwise destroy this. 
		//This makes sure that when changing from scene to scene, the sfx manager works without stopping
		//and without creating multiple instances of it.
		if (instance == null)
        {
			instance = this;
		}
        else
        {
			Destroy (gameObject);
			return;
		}

		//Goes through the Sounds array and sets the attributes given in the Sound Class to the appropriate AudioSource attributes.

		foreach (Sound s in sounds) 
		{
			s.source=gameObject.AddComponent<AudioSource> ();
			s.source.clip = s.clip;

			s.source.outputAudioMixerGroup = s.sfx;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.spatialBlend = s.spatialBlend;
			s.source.loop = s.loop;
			s.source.playOnAwake = s.onAwake;

			//According to the type of sound typed, it's being output to the according audioMixer.
			if (s.type=="sfx")
            {
				s.source.outputAudioMixerGroup = s.sfx;
			}
            else if (s.type=="background")
            {
				s.source.outputAudioMixerGroup = s.background;
			}
            else if (s.type=="ambient")
            {
				s.source.outputAudioMixerGroup = s.ambient;
			}
		}
	}

	//Function that is being called when we want to play a sound.
	//Searches for the sound name given in the parameter and plays it.
	public void Play(string name)
    {
		Sound s=Array.Find (sounds, sound => sound.name == name);
		if (s == null)
        {
			Debug.Log ("Sounds: " + name + "not found!");
			return;
		}
		s.source.Play ();
	}

	//Function that is being called when we want to stop a sound.
	//Searches for the sound name given in the parameter and stops it.
	public void Stop(string name)
    {
		Sound s = Array.Find (sounds, sound => sound.name == name);
		if (s == null)
        {
			Debug.Log ("Sounds: " + name + "not found!");
			return;
		}
		s.source.Stop ();
	}
}