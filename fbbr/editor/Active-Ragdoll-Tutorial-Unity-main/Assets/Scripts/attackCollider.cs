using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackCollider : MonoBehaviour
{
    [SerializeField] private CharacterController CC;
    [SerializeField] private Collider selfTarget;

    void OnTriggerEnter(Collider collision)
    {
        if (CC.target != null && collision == CC.target.gameObject.GetComponent<Collider>())
        {
            CC.targetInAttackRange = true;
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if(CC.target != null){
            if (collision == CC.target.gameObject.GetComponent<Collider>())
            {
                CC.targetInAttackRange = false;
            }
        }
    }
}
