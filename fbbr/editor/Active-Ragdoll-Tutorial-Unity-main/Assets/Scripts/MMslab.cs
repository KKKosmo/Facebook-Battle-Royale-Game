using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MMslab : MonoBehaviour
{

    public void removeCustomChar(GameObject parent){
        GameObject.Find("count").GetComponent<TMP_Text>().text = (int.Parse(GameObject.Find("count").GetComponent<TMP_Text>().text) - 1).ToString();
        Destroy(parent);
    }
}
