using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class mainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField IF;
    [SerializeField] Transform scrollviewC;
    [SerializeField] GameObject MMcharSlab;
    [SerializeField] TMP_Text count;
    
    void Start()
    {
        IF.text = gameManager.totalPlayers.ToString();
        
        foreach(string s in gameManager.customChars){
            addCustomChar(s);
        }
    }

    public void getNumberOfChars(){
        IF.text = (Mathf.Min(804, float.Parse(IF.text))).ToString();
        gameManager.totalPlayers = (int)Mathf.Min(804, float.Parse(IF.text));
    }
    public void addCustomChar(string _name){
        count.text = (int.Parse(count.text) + 1).ToString();
        GameObject customChar  = Instantiate(MMcharSlab, scrollviewC.transform, false);
        customChar.transform.GetChild(0).GetComponent<TMP_InputField>().text = _name;
        customChar.transform.SetSiblingIndex(0);
    }
    public void LaunchGame(){
        gameManager.customChars = new List<string>();
        for(int i = 0; i < int.Parse(count.text); i++){
            gameManager.customChars.Add(scrollviewC.GetChild(i).GetChild(0).GetComponent<TMP_InputField>().text);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
