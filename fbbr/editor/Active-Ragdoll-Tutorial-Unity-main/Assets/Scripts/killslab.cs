using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killslab : MonoBehaviour
{
    void Start(){
        Invoke(nameof(CleanKillfeed), 5);
    }
    private void CleanKillfeed(){
        Destroy(gameObject);
    }
}
