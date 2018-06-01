using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class BoarBeast : MonoBehaviour {

        public NavMeshAgent agent;// This is a navmesh agent
        static Animator anim;
        public StatsSettings settings;

        //Defines the states the character can take
        public enum State
        {
            PATROL,
            CHASE,
            ATTACK,
            DYING
        }

        public State state;
        private bool alive;

        //Variables for patrol
        public GameObject[] waypoints;
        private int waypointInd = 0;
        public float patrolSpeed = 2f;

        //Variable for chasing
        public float chaseSpeed = 3f;
        public GameObject target;
        private Vector3 direction;

        // Use this for initialization
        void Start () {
            agent = GetComponent<NavMeshAgent>();
			anim = GetComponent<Animator> ();

            agent.updatePosition = true;
            agent.updateRotation = false;
            state = BoarBeast.State.PATROL;
            alive = true;
            StartCoroutine("FSM");
	    }

        //Finite state machine, used to switch between the states the enemy can take       
        IEnumerator FSM()
        {
            yield return null;
            while (alive)
            {
                //Defines the states and how they work
                switch (state)
                {
                    case State.PATROL:
                        Patrol();
                        break;
                    case State.CHASE:
                        Chase();
                        break;
                    case State.ATTACK:
                        Attack();
                        break;
                    case State.DYING:
                        Dying();
                        break;

                }
                yield return null;
            }

        }

        //Defines the function for the state of the patrol
        void Patrol()
        {
            agent.speed = patrolSpeed;//sets the speed for patroling
            //used to go from a waypoint to another
            if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) >= 5)
            {
                agent.SetDestination(waypoints[waypointInd].transform.position);
                Vector3 direction = waypoints[waypointInd].transform.position - this.transform.position;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);

            }
            else if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) <= 5)
            {
                //resets the array for the waypoints
                waypointInd += 1;
                if (waypointInd >= waypoints.Length)
                {
                    waypointInd = 0;
                }
            }
        }

        //Defines the function of attacking from that state. Same as for the wood enemy
        void Attack()
        {
            if (alive)
            {
                anim.SetBool("isAttacking", true);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
                {
                    settings.enemyAttacking = true;
                }
                Vector3 direction = target.transform.position - this.transform.position;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            }
        }

        //Defines the function of dying from that state. Same as for the wood enemy
        void Dying()
        {
            if (EnemyScript.currentHealthPoints < 1)
            {
                anim.SetBool("isAttacking", false);
                anim.SetBool("isDead", true);
                alive = false;
                //stops the sounds for Lucy
				FindObjectOfType<SFXManager> ().Stop ("LucyMoving");
				FindObjectOfType<SFXManager> ().Stop ("LucyAttacking");
            }
        }

        //Function for the Chase state where the enemy chases the player, same as the wood enemy 
        void Chase()
        {
			anim.SetBool ("isAttacking", false);//stops the attack animation
            agent.speed = chaseSpeed;

            agent.SetDestination(target.transform.position);//sets the target as destination
            //turns to him
            Vector3 direction = target.transform.position - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            //Because of the size of the boar sometimes it doesn't reach the collider so we added a fail safe 
			if (direction.magnitude < 3)
            {
				state = BoarBeast.State.ATTACK;
			}
            else
            {
				state = BoarBeast.State.CHASE;
			}
        }

        void OnTriggerEnter(Collider other)
        {   //same as for the wood enemy checks for collision then starts chasing tha player
            if (other.tag == "player")
            {
                state = BoarBeast.State.CHASE;
                target = other.gameObject;
                //plays the chasing sound
				FindObjectOfType<SFXManager> ().Play ("LucyMoving");
               
            }
            else if (other.tag == "Player")
            {
                target = other.gameObject;
                state = BoarBeast.State.ATTACK;
            }
        }

        //Starts chasing the player again if he tries to run
		void OnTriggerExit(Collider other)
		{
			if (other.tag=="player")
            {
				target = other.gameObject;
				state = BoarBeast.State.CHASE;
				FindObjectOfType<SFXManager> ().Stop ("LucyMoving");
			}
		}
    }
}
