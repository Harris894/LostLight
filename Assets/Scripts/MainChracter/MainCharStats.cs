using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCharStats : MonoBehaviour
{
	public StatsSettings settings;
	public MainCharControls m_Control;
	public static float healthPoints;
	public static float staminapoints;
	public Image healthBar;
	public Image staminaBar;
	public Canvas hudCanvas;

	private CapsuleCollider m_Collider;

	void Awake()
	{
		settings.currentHP = settings.maxHp;
		settings.currentStamina = settings.maxStamina;
	}
		
	void Start ()
    {
		m_Collider = GetComponent<CapsuleCollider> ();
		m_Control = GetComponent<MainCharControls> ();
	}
		
	void Update ()
    {	//As long as you are not running, the stamina is regenerating. 
		if (!m_Control.isCurrentlySprintng) 
		{
			settings.currentStamina += settings.staminaRechargeRate * Time.deltaTime;
		}
		//As long as you are not attacking or getting attacked,the HP is regenerating
		if (!m_Control.isCurrentlyAttacking && !m_Control.isCurrentlyHit)
        {
			settings.currentHP += settings.hpRegen * Time.deltaTime;
		}
		//Sets a minimum and maximum value for stamina and HP.
		settings.currentHP= Mathf.Clamp (settings.currentHP, 0, 100);
		settings.currentStamina= Mathf.Clamp (settings.currentStamina, 0, 100);

		//Health and stamina bar fills constantly updating according to the values.
		healthBar.fillAmount = settings.currentHP / settings.maxHp;
		staminaBar.fillAmount = settings.currentStamina / settings.maxStamina;
	}

	//When going close to a Checkpoint, refill health and stamina.
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            settings.currentHP = settings.maxHp;
            settings.currentStamina = settings.maxStamina;
        }
    }
}