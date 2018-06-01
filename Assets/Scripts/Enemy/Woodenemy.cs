using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Woodenemy : MonoBehaviour
    {   
        static Animator anim;
        public NavMeshAgent agent;// This is a navmesh agent
        public StatsSettings settings;

        //Defines the states the character can take
        public enum State
        {
            IDLE,
            CHASE,
            ATTACK,
			DYING
        }

        public State state;
		private bool alive;

		private AudioSource woodThingIdleSound;
		private int count=0;

        //Variable for chasing
        public float chaseSpeed = 3f;
        public GameObject target;

        // Use this for initialization
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
			woodThingIdleSound = GetComponent<AudioSource> ();

            agent.updatePosition = true;
            agent.updateRotation = false;
           
            state = Woodenemy.State.IDLE;//The state the charcter starts
            alive = true;
            StartCoroutine("FSM");//Starts the finite state machine
        }

        //Finite state machine, used to switch between the states the enemy can take
        IEnumerator FSM()
        {
            yield return null;
            while (alive)
            {
                switch (state)
                {   
                    //Defines the states and how they work
                    case State.IDLE:
                        Idle();
                        break;
                    case State.CHASE:
                        Chase();
                        break;
                    case State.ATTACK:
                        Attack();
                        break;
					case State.DYING:
						Dying ();
						break;
                }
                yield return null;
            }
		}

        void Update()
        {
            //Checks is the enemy is dead and switches to that
            if (EnemyScript.currentHealthPoints < 1)
            {
                state = Woodenemy.State.DYING;
            }
        }

        void Idle()
        { 
            //The charcter is not attacking, just sits around
            anim.SetBool("isAttacking", false);  
        }

        //Defines the function of attacking from that state
        void Attack()
        {
			if (alive) {
				anim.SetBool("isAttacking", true);//switches to the attacking animation

				if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))// if the enemy animation is set on attack it sets the setting to attack
				{
					settings.enemyAttacking = true;
				}
                //Turns towards the player
                Vector3 direction = target.transform.position - this.transform.position;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            }   
        }

        //Function for the Chase state where the enemy chases the player
        void Chase() 
		{	
			if (EnemyScript.currentHealthPoints >= 1)
            {
                //Plays the enemy sound
				if (count==0)
                {
					FindObjectOfType<SFXManager>().Play("WoodThing");
				}

				count = 1;
                
				anim.SetBool("isAttacking", false);//The animation for attack is set to false
				agent.speed = chaseSpeed;
				agent.SetDestination(target.transform.position);//goes towards the target
                //Turns to target
				Vector3 direction = target.transform.position - this.transform.position;
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
			}
        }

        //Function for the dying state where the enemy dies
        void Dying()
        {   
            //checks again if the enemy is dead
			if (EnemyScript.currentHealthPoints < 1)
            {
				anim.SetBool ("isAttacking", false);//stops animations for attack
				anim.SetBool ("isDead", true);// plays the animation for dying
				alive = false;
				FindObjectOfType<SFXManager> ().Stop ("WoodThing");
			}

		}

        void OnTriggerEnter(Collider other)
        {   
            //Checks if the player is through a trigger collider, if the player is close it starts chasing
            if (other.tag == "player")
            {
                state = Woodenemy.State.CHASE;
                target = other.gameObject;
            }
            else if(other.tag == "Player")//if it collides with the body after chasing it starts attacking
            {
                target = other.gameObject;
                state = Woodenemy.State.ATTACK;
            }
        }

        void OnTriggerExit(Collider other)//if the player tries to run away it still chaes
        {
            if (other.tag == "player")
            {
                target = other.gameObject;
                state = Woodenemy.State.CHASE;
            }
        }
    }
}
