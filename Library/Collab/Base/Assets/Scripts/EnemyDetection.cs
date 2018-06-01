using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour {

    private CapsuleCollider myCapsuleCollider;

    public float sec;

    private void Start()
    {
        myCapsuleCollider = this.GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="enemy")
        {
            StartCoroutine(LateCall());
        }
    }

    IEnumerator LateCall()
    {
        yield return new WaitForSeconds(sec);
        myCapsuleCollider.enabled = false;
    }
}
