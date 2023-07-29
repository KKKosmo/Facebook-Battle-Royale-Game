using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public CharacterController owner;
    public float range;
    public float damage;

    void Start(){
        Destroy(gameObject, range);
    }
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.layer == 8){
            // Debug.Log(collision.gameObject.name);
            GameObject character = collision.transform.parent.gameObject;
            // Debug.Log(character.name);
            while(character.name != "ActiveRagdoll"){
                character = character.transform.parent.gameObject;
                // Debug.Log(character.name);
            }
                // Debug.Log("");
            CharacterController CC = character.GetComponent<CharacterController>();
            CC.TakeDamage(UnityEngine.Random.Range(0, damage), owner);
            
        }
    }
}
