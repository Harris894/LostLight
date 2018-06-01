using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucyAttacking : MonoBehaviour
{
	public StatsSettings settings;
	public LucyTheBoarBeast lucy;

	private CapsuleCollider m_Collider;
	private int counts=0;

	// Use this for initialization
	void Start ()
    {
		m_Collider = this.GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update ()
	{	//If Lucy is dead, destroy the collider
		if (settings.lucyCurHP<1)
        {
			Destroy (m_Collider);
		}
        else
        {
			OnTriggerEnter (m_Collider);
			OnTriggerExit (m_Collider);
		}
	}

	//If the collider touches the Player, deduct the amount of attackdamage from his HP.
	//Set that he was hit from left so the appropriate animation is played.
	//After 1sec the counts is set back to 0 so that the player can get hit again so it doesn't constantly apply damage.
	//Play the appropriate sound.
	void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Player" && counts == 0)
        {
			settings.currentHP -= settings.lucyDMG;
			StartCoroutine (LateCall ());
			counts = 1;
			FindObjectOfType<SFXManager> ().Play ("LucyAttack");
		}
	}

	//If the collider exits and counts is still 1, set it to 0 and stop playing the attacking sound.
	void OnTriggerExit(Collider other)
    {
		if (gameObject==null)
        {
			if (other.gameObject.tag=="Player"&&counts==1)
            {
				StartCoroutine (LateCall ());
				FindObjectOfType<SFXManager> ().Stop ("LucyAttack");
			}
		}
	}

	IEnumerator LateCall()
	{
		yield return new WaitForSeconds(1);
		counts=0;
	}
}