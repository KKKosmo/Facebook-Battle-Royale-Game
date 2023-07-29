using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sightCollider : MonoBehaviour
{
    [SerializeField] private CharacterController CC;
    [SerializeField] private Collider selfTarget;

    void OnTriggerEnter(Collider collision)
    {
        if(CC.target == null && collision.gameObject.name == "target" && collision != selfTarget){
            CC.target = collision.transform;
            CC.target.transform.parent.transform.parent.transform.parent.GetComponent<CharacterController>().targetedBy.Add(CC);
            CC.targetInSightRange = true;
        }       
    }
}