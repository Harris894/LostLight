using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainCharControls : MonoBehaviour
{
    static Animator anim;
	public StatsSettings settings;
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float smoother = 50.0f;
    public Collider bladeCollider;
    public ParticleSystem emberEffect;
	public ParticleSystem swordFlames;
	public ParticleSystem swordEmbers;
    public float targetTime;
    public static int buff=0;

	[HideInInspector]
    public bool isCurrentlyAttacking = false;
	[HideInInspector]
	public bool isCurrentlyLightAttacking = false;
	[HideInInspector]
	public bool isCurrentlyHeavyAttacking = false;
	[HideInInspector]
	public bool isCurrentlySprintng=false;
	[HideInInspector]
	public bool isCurrentlyHitLeft=false;
	[HideInInspector]
	public bool isCurrentlyHit=false;
	[HideInInspector]
	public bool isCurrentlyHitRight=false;
	[HideInInspector]
	public bool isCurrentlyDodging=false;
	[HideInInspector]
	public bool isBuffed=false;
    
	private float translation;
	private ParticleSystem.EmissionModule emberEmission;
	private ParticleSystem buffParticles;
	private bool isDead;
   
    private void Awake()
    {
        emberEmission = emberEffect.emission;
    }

    void Start ()
    {
        anim = GetComponent<Animator>();
        Collider bladeCollider = GetComponent(typeof(CapsuleCollider)) as Collider;
        emberEffect = GetComponent<ParticleSystem>();
		buffParticles = this.GetComponent<ParticleSystem> ();
		settings.hpRegen = 10;
		isDead = false;
		settings.enemyHitFromLeft = false;
		settings.enemyHitFromRight = false; 
	}
	
	void Update ()
    {
		settings.charAttacking = isCurrentlyAttacking;
		//Character moves only if he is not currently attacking, or getting attacked or dodging.
		if (!isCurrentlyAttacking||!isCurrentlyHit||!isCurrentlyDodging)
        {
            translation = Input.GetAxis("Vertical") * speed;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

			//Calculations to rotate the character towards the mouse position.
            Vector3 lookDir = Camera.main.transform.forward;
            lookDir.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

			//Where translating and rotating is actually executed.
			if (translation != 0 || rotation != 0)
            {
				transform.Translate (0, 0, translation);
				transform.Rotate (0, rotation, 0);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, lookRotation, Time.deltaTime * smoother);
			}
        }
        
		//Stops the jumbing animation.
        anim.SetBool("isJumping", false);

		//if the stamina reaches 0 the sprinting stops
		if (settings.currentStamina<1)
        {
			anim.SetBool ("isSprinting",false);
			isCurrentlySprintng = false;
		}

		//if he character dies, set regen to 0, stop particle system for torch,
		//after 0.05seconds, set the animator to false so that it doesn't keep updating to true.
		if (settings.currentHP<1&&isDead==false)
        {
			anim.SetBool ("isDead", true);
			settings.hpRegen = 0;
			isDead = true;
			emberEffect.Stop ();
			StartCoroutine (DodgeTurnOff ());
		}

		//if he is hit from left, plays the appropriate animation
		if (settings.enemyHitFromLeft)
        {
			anim.SetBool ("GettingHitFromLeft", true);
			isCurrentlyHitLeft = true;
			isCurrentlyHit = true;
		}
		else//otherwise it's set to false
        {
			anim.SetBool ("GettingHitFromLeft", false);
			isCurrentlyHitLeft = false;
		}

		//if he is hit from left, plays the appropriate animation
		if (settings.enemyHitFromRight)
        {
			anim.SetBool ("GettingHitFromRight", true);
			isCurrentlyHitRight = true;
			isCurrentlyHit = true;
		}
        else//otherwise set to false.
        {
			anim.SetBool ("GettingHitFromRight", false);
			isCurrentlyHitRight = false;
		}

		//update the connecting scripts.
		if (!isCurrentlyHitLeft || !isCurrentlyHitRight)
        {
			isCurrentlyHit = false;
			settings.enemyHitFromLeft = false;
			settings.enemyHitFromRight = false;
		}

		//If F gets pressed, player gets a buff.
		//play the animation, sound, starts the particle systems and start coroutines so they don't keep playing constantly
		if ((Input.GetKeyDown(KeyCode.F))&&(buff==0))
        {
			anim.SetBool ("isBuffing", true);
			isBuffed = true;
			StartCoroutine (StartSparks ());
			StartCoroutine (DodgeTurnOff ());
			StartCoroutine (BuffOut ());
            buff = 1;
			FindObjectOfType<SFXManager> ().Play ("BuffExplosion");
			FindObjectOfType<SFXManager> ().Play ("BuffFire");
		}

		//Turn off particle systems
		if (!isBuffed)
        {
			swordFlames.Stop ();
			swordEmbers.Stop ();
		}

		//Do these if the player is not standing still.
        if (anim.GetFloat("Vertical") != 0)
        {
			//With space jump, and set the emission for the particles higher.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("isJumping",true);
                emberEmission.rate = 80f;
            }

			//With leftShift if stamina is more thatn 0,start sprinting, set the emission for the particles higher, 
			if (Input.GetKeyDown(KeyCode.LeftShift) && translation>0 && settings.currentStamina >0)
            {
				anim.SetBool ("isSprinting", true);
				emberEmission.rate = 80f;
            }

			//When leftShift is unpressed, stop sprinting.
            if (Input.GetKeyUp(KeyCode.LeftShift)||translation==0 )
            {
                anim.SetBool("isSprinting", false);      
            }
        }

		//This runs only if the player is not attacking at the moment.(so that the actions don't duplicate)
		if (!isCurrentlyAttacking)
        {	//Left click is lightAttack
			if (Input.GetMouseButtonDown(0))
			{
				anim.SetBool("isLightAttacking", true);
			}
			//Right click is heavyAttack
			if (Input.GetMouseButtonDown(1))
			{
				anim.SetBool("isHeavyAttacking", true);
			}
			//When Q is pressed, play the dodging animation, deduct stamina, and turn animation bool off.
			if (Input.GetKeyDown(KeyCode.Q)&& isCurrentlyDodging==false)
            {
				anim.SetBool ("isDodging", true);
				settings.currentStamina -= settings.dodgeStaminaCost;
				StartCoroutine (DodgeTurnOff ());
			}
		}
        
		//When left mouse button comes up, turn off light attack animation
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isLightAttacking", false);
        }
		//When right mouse button comes up, turn off heavy attack animation
        if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("isHeavyAttacking", false);
        }

		//While the light attack animation is playing-enable the sword's collider
		//increase emission rate
		//update connecting scripts
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Light_Attack"))
        {

			isCurrentlyAttacking = true;
            bladeCollider.enabled = true;
            emberEmission.rate = 80f;
			isCurrentlyLightAttacking = true;
        }

		//While the heavy attack animation is playing-enable the sword's collider
		//increase emission rate
		//update connecting scripts
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy_Attack")) 
		{
			isCurrentlyAttacking = true;
			bladeCollider.enabled = true;
			emberEmission.rate = 80f;
			isCurrentlyHeavyAttacking = true;
		}

		//While the walking animation is playing, set emission rate lower, set sprinting to false.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            emberEmission.rate = .02f;
			isCurrentlySprintng = false;
        }

		//While the sprinting animation is playing deduct stamina constantly and set sprinting to true.
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting")) 
		{
			isCurrentlySprintng = true;
			settings.currentStamina -= settings.sprintingStaminaCost * Time.deltaTime;
		}

		//While dodging, set the bools accordingly.
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Dodge_Back"))
        {
			isCurrentlyDodging = true;
		}
        else
        {
			isCurrentlyDodging = false;
		}

		//Specific checks for the stamina deduction 
		if (Input.GetMouseButtonDown(1)&&anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy_Attack")==false)
        {
			settings.currentStamina -= settings.heavyAttackCost;
		}

		//Specific checks for the stamina deduction 
		if (Input.GetMouseButtonDown(0)&&anim.GetCurrentAnimatorStateInfo(0).IsName("Light_Attack")==false)
        {
			settings.currentStamina -= settings.lightAttackCost;
		}

		//If none of the attacking animations are running, set the bools accordingly.
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Light_Attack") == false && anim.GetCurrentAnimatorStateInfo (0).IsName ("Heavy_Attack") == false)
        {
            isCurrentlyAttacking = false;
			isCurrentlyLightAttacking = false;
			isCurrentlyHeavyAttacking = false;
		}

		//Backspace to restart the scene(lack of pause menu)
		if (Input.GetKeyDown(KeyCode.Backspace))
        {
			restartScene ();
		}
    }

	//After 0.05 seconds, set animation bools to false to avoid animations constantly trying to run creating glitches.
	IEnumerator DodgeTurnOff()
	{
		yield return new WaitForSeconds (0.05f);
		anim.SetBool ("isDodging", false);
		anim.SetBool ("isBuffing", false);
		anim.SetBool ("isDead", false);
	}

	//Buff burn out duration.
	IEnumerator BuffOut()
	{
		yield return new WaitForSeconds (20);
		isBuffed = false;
		swordEmbers.Stop ();
	}

	//Adds a delay to the start of particle effects for the buffed state.
	IEnumerator StartSparks()
	{
		yield return new WaitForSeconds (0.3f);
		buffParticles.Play ();
		swordFlames.Play ();
		swordEmbers.Play ();
	}

	void restartScene()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}    
}
