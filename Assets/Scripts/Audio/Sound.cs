using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{	//Attributes for the array of Sounds used in the SFXManager.
	//They are later related to the attributes of Component:AudioSource.
	public string name;
	public string type;
	public AudioClip clip;

	[Range(0f,1f)]
	public float volume=1;
	[Range(0.1f,3f)]
	public float pitch=1;
	public bool onAwake = false;
	public bool loop = false;
	public float spatialBlend;
	public AudioMixerGroup sfx;
	public AudioMixerGroup background;
	public AudioMixerGroup ambient;

	[HideInInspector]
	public AudioSource source;
}
