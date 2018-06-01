using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class EnemyScript : MonoBehaviour
{
    static Animator anim;
	public StatsSettings settings;
    public Image healthBar;
    public Canvas hpCanvas;
    public Light leftEye;
    public Light rightEye;
	public MainCharControls mainChar;
    public float maxHealthPoints;
    public static float currentHealthPoints;
    public float sec=1f;
	public float secHeavy=2f;
	public bool dead=false;

	private CapsuleCollider my_Collider;
	private AudioSource woodThingIdleSound;
	private int counts = 0;
    
	//Set values according to StatsSettings script.
	void Awake()
    {
		maxHealthPoints = settings.woodSpiritHP;
		currentHealthPoints = maxHealthPoints;
	}

	// Use this for initialization
	void Start ()
    {
		anim = GetComponent<Animator>();
		my_Collider = GetComponent<CapsuleCollider> ();
		woodThingIdleSound = GetComponent<AudioSource> ();      
    }

	void Update ()
    {
		settings.woodSpiritCurHP = currentHealthPoints;
		//if the WoodSprite is dead,play death animation, disable HPCanvas, turn off lights in the eyes
		//10 seconds after death, destroy the object.
        if (currentHealthPoints<1)
        {
            anim.SetBool("isDead", true);
            hpCanvas.enabled = false;
            leftEye.enabled = false;
            rightEye.enabled = false;
			settings.enemyAttacking = false;
			StartCoroutine (DestroyOnDeath ());
			dead = true;
        }
		//Between 26 and 50 hp the healtBar's color is set to yellow.
        if (currentHealthPoints>26 && currentHealthPoints<50)
        {
            healthBar.color = Color.yellow;
        }
		//Between 1 and 25 hp the healtBar's color is set to red.
		if (currentHealthPoints < 25)
        {
			healthBar.color = Color.red;
		}
	}


    private void OnTriggerEnter(Collider other)
    {	//If the Player is currently attacking and the sword has collided with the enemy, if he used a light attack or heavy attack deduct the appropriate amount from his HP
		//update the fill of the healthBar.
		//play the appropriate sound.
		if (mainChar.isCurrentlyAttacking&& counts==0)
        {
			if (other.gameObject.tag=="weapon" && mainChar.isCurrentlyLightAttacking==true)
			{
				currentHealthPoints = currentHealthPoints - settings.lightAttackDMG;
				healthBar.fillAmount = currentHealthPoints / maxHealthPoints;
				counts = 1;
				FindObjectOfType<SFXManager> ().Play ("LightAttacking");
			}

			if (other.gameObject.tag=="weapon" && mainChar.isCurrentlyHeavyAttacking==true)
			{
				currentHealthPoints = currentHealthPoints - settings.heavyAttackDMG;
				healthBar.fillAmount = currentHealthPoints / maxHealthPoints;
				counts = 2;
				FindObjectOfType<SFXManager> ().Play ("HeavyAttacking");
			}
		}       
    }

	private void OnTriggerExit(Collider other)
    {	//Upon exiting, according to the variable counts, we choose between 2 coroutines with different waiting times
		//since each attack runs for different duration.
		//and also stops the hitting sound.
		if (other.gameObject.tag=="weapon")
        {
			if (counts==1 && mainChar.isCurrentlyAttacking)
            {
				StartCoroutine (LateCall ());
				FindObjectOfType<SFXManager> ().Stop ("LightAttacking");
			}
			if (counts==2 && mainChar.isCurrentlyAttacking)
            {
				StartCoroutine (LateCall2 ());
				FindObjectOfType<SFXManager> ().Stop ("HeavyAttacking");
			}
		}
	}


    IEnumerator LateCall()
    {
        yield return new WaitForSeconds(sec);
		counts=0;
    }

	IEnumerator LateCall2()
	{
		yield return new WaitForSeconds(secHeavy);
		counts=0;
	}

	IEnumerator DestroyOnDeath()
    {
		yield return new WaitForSeconds(10);
		Destroy (gameObject);
	}
}
