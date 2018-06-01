using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainCharControls : MonoBehaviour {

    static Animator anim;
	public StatsSettings settings;
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float smoother = 50.0f;
    public Collider bladeCollider;
    public ParticleSystem emberEffect;
	public ParticleSystem swordFlames;
    public float targetTime;
    public static int buff=0;

	//[HideInInspector]
    public bool isCurrentlyAttacking = false;
	//[HideInInspector]
	public bool isCurrentlyLightAttacking = false;
	//[HideInInspector]
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
   

    private void Awake()
    {
        emberEmission = emberEffect.emission;
    }

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        Collider bladeCollider = GetComponent(typeof(CapsuleCollider)) as Collider;
        emberEffect = GetComponent<ParticleSystem>();
		swordFlames = GetComponent<ParticleSystem> ();
		buffParticles = this.GetComponent<ParticleSystem> ();
        
	}
	
	// Update is called once per frame
	void Update () {

		if (!isCurrentlyAttacking||!isCurrentlyHit||!isCurrentlyDodging)
        {
            translation = Input.GetAxis("Vertical") * speed;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

            Vector3 lookDir = Camera.main.transform.forward;
            lookDir.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

            if (translation != 0 || rotation!=0)
            {
                transform.Translate(0, 0, translation);
                transform.Rotate(0, rotation, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * smoother);
            }
        }
        
        
        
        anim.SetBool("isJumping", false);

		if (settings.currentStamina<1) {
			anim.SetBool ("isSprinting",false);
			isCurrentlySprintng = false;
		}

		if (settings.enemyHitFromLeft) {
			anim.SetBool ("GettingHitFromLeft", true);
			isCurrentlyHitLeft = true;
			isCurrentlyHit = true;
		} else {
			anim.SetBool ("GettingHitFromLeft", false);
			isCurrentlyHitLeft = false;
		}

		if (settings.enemyHitFromRight) {
			anim.SetBool ("GettingHitFromRight", true);
			isCurrentlyHitRight = true;
			isCurrentlyHit = true;
		} else {
			anim.SetBool ("GettingHitFromRight", false);
			isCurrentlyHitRight = false;
		}

		if (!isCurrentlyHitLeft || !isCurrentlyHitRight) {
			isCurrentlyHit = false;
			settings.enemyHitFromLeft = false;
			settings.enemyHitFromRight = false;
		}

		if ((Input.GetKeyDown(KeyCode.F))&&(buff==0)) {
			anim.SetBool ("isBuffing", true);
			isBuffed = true;
			StartCoroutine (StartSparks ());
			StartCoroutine (DodgeTurnOff ());
			StartCoroutine (BuffOut ());
            buff = 1;
		}

		if (!isBuffed) {
			swordFlames.Stop ();
		}
        
      


        if (anim.GetFloat("Vertical") != 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("isJumping",true);
                emberEmission.rate = 80f;
            }

			if (Input.GetKeyDown(KeyCode.LeftShift) && translation>0 && settings.currentStamina >0)
            {
				
				anim.SetBool ("isSprinting", true);
				emberEmission.rate = 80f;
		
            }

            if (Input.GetKeyUp(KeyCode.LeftShift)||translation==0 )
            {
                anim.SetBool("isSprinting", false);      
            }



        }

		if (!isCurrentlyAttacking) {
			if (Input.GetMouseButtonDown(0))
			{

				anim.SetBool("isLightAttacking", true);

			}

			if (Input.GetMouseButtonDown(1))
			{
				anim.SetBool("isHeavyAttacking", true);


			}

			if (Input.GetKeyDown(KeyCode.Q)&& isCurrentlyDodging==false) {
				anim.SetBool ("isDodging", true);
				settings.currentStamina -= settings.dodgeStaminaCost;
				StartCoroutine (DodgeTurnOff ());
			}

		}
        

        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isLightAttacking", false);

        }

       
        if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("isHeavyAttacking", false);

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Light_Attack"))
        {
            
			StartCoroutine(LateCall());
			isCurrentlyAttacking = true;
            bladeCollider.enabled = true;
            emberEmission.rate = 80f;
			isCurrentlyLightAttacking = true;
        }

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy_Attack")) 
		{
			StartCoroutine(LateCall());
			isCurrentlyAttacking = true;
			bladeCollider.enabled = true;
			emberEmission.rate = 80f;
			isCurrentlyHeavyAttacking = true;
		}

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            
            emberEmission.rate = .02f;
            //isCurrentlyAttacking = false;
			isCurrentlySprintng = false;

        }

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting")) 
		{
			isCurrentlySprintng = true;
			settings.currentStamina -= settings.sprintingStaminaCost * Time.deltaTime;
		}

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Dodge_Back")) {
			isCurrentlyDodging = true;
		} else {
			isCurrentlyDodging = false;
		}

		if (Input.GetMouseButtonDown(1)&&anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy_Attack")==false) {
			settings.currentStamina -= settings.heavyAttackCost;
		}
		if (Input.GetMouseButtonDown(0)&&anim.GetCurrentAnimatorStateInfo(0).IsName("Light_Attack")==false) {
			settings.currentStamina -= settings.lightAttackCost;
		}
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Light_Attack") == false && anim.GetCurrentAnimatorStateInfo (0).IsName ("Heavy_Attack") == false) {

			isCurrentlyAttacking = false;
			isCurrentlyLightAttacking = false;
			isCurrentlyHeavyAttacking = false;
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			restartScene ();
		}
    }

	IEnumerator LateCall()
	{
		if (isCurrentlyAttacking) 
		{
			yield return new WaitForSeconds(targetTime);
			//isCurrentlyAttacking = false;
		}

	}

	IEnumerator DodgeTurnOff()
	{

		yield return new WaitForSeconds (0.05f);
		anim.SetBool ("isDodging", false);
		anim.SetBool ("isBuffing", false);
	}

	IEnumerator BuffOut()
	{
		yield return new WaitForSeconds (20);
		isBuffed = false;
	}

	IEnumerator StartSparks()
	{
		yield return new WaitForSeconds (0.3f);
		buffParticles.Play ();
		swordFlames.Play ();

	}

	void restartScene()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}



    
}
