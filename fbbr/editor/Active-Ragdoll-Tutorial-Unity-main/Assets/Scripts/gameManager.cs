using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using Cinemachine;


public class gameManager : MonoBehaviour
{
    [SerializeField] public HashSet<string> friends;
    [SerializeField] TextAsset names;
    public static List<string> customChars = new List<string>();
    [SerializeField] TMP_Text charactersLeftText;
    List<CharacterController> characters = new List<CharacterController>();
    public static int totalPlayers;
    [SerializeField] GameObject character;

    [SerializeField] GameObject battlefield;
    [SerializeField] GameObject scoreboard;
    [SerializeField] GameObject scoreslab;
    [SerializeField] GameObject winGUI;
    [SerializeField] TMP_Text winText;

    [SerializeField] CinemachineVirtualCamera cam;

    private static readonly System.Random random  = new System.Random();
    
    
    private void OnEnable(){
        CharacterController.onCharacterKilled += handleCharacterDefeated;
    }
    private void OnDisable(){
        
        CharacterController.onCharacterKilled -= handleCharacterDefeated;
    }
    void Awake()
    {
		cam = GameObject.Find("VirtualCamera1").GetComponent<CinemachineVirtualCamera>();
        friends = new HashSet<string>(names.text.Split("\r\n"));


        totalPlayers += customChars.Count;
        Vector3 scale = battlefield.transform.localScale;
        scale.Set(Math.Max(6, totalPlayers / 1.5f), Math.Max(6, totalPlayers / 1.5f), Math.Max(6, totalPlayers / 1.5f));
        battlefield.transform.localScale = scale;
        // Debug.Log($"intial scale is {scale}");
        // Debug.Log($"intial scale is {((battlefield.transform.localScale.x-2)/(-2)) * 10} , {((battlefield.transform.localScale.x-2)/2) * 10}");
        
        for(int i = 0; i < totalPlayers; i++){
            string __name = friends.ElementAt(random.Next(friends.Count));
            CreateCustomChar(__name);
        }
        foreach(string s in customChars){
            CreateCustomChar(s);
        }
        UpdateEnemiesLeftText();
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            ExitGame();
        }
    }

    void handleCharacterDefeated(CharacterController character){
        if(characters.Remove(character)){
            UpdateEnemiesLeftText();
        }
    }
    
    void UpdateEnemiesLeftText(){
        charactersLeftText.text = $"Characters left: {characters.Count}";
        if(characters.Count == 1){
            winGUI.SetActive(true);
            foreach(CharacterController CC in characters){
                winText.text = string.Format($"{CC._name} wins with {CC.kills} kills!");
            }
        }
        else if(characters.Count == 0){
            winGUI.SetActive(true);
            foreach(CharacterController CC in characters){
                winText.text = string.Format("everyone died LMAO");
            }
        }
        
    }
    void CreateCustomChar(string Name){

            float x = UnityEngine.Random.Range(((battlefield.transform.localScale.x-3)/(-2)) * 10, ((battlefield.transform.localScale.x-3)/2) * 10);
            float z = UnityEngine.Random.Range(((battlefield.transform.localScale.z-3)/(-2)) * 10, ((battlefield.transform.localScale.z-3)/2) * 10);
            Vector3 spawn;
            spawn = new Vector3(x,1,z);
            // Debug.Log(spawn);
            
            GameObject _character  = Instantiate(character, spawn, Quaternion.identity);
            CharacterController _characterCC = _character.transform.Find("ActiveRagdoll").GetComponent<CharacterController>();
            GameObject characterScore  = Instantiate(scoreslab, scoreboard.transform, false);
            _characterCC._name = Name;
            _characterCC.scoreSlab = characterScore;
            _characterCC.nametag.text = Name;
            _characterCC.scoretag = characterScore.transform.GetChild(0).GetComponent<TMP_Text>();
            _characterCC.scoretag.text = (string.Format($"{Name}: 0"));

            characterScore.GetComponent<score>().spectatePoint = _characterCC.spectatePoint.transform;

            //to avoid duplicates
            friends.Remove(Name);

            characters.Add(_characterCC);
    }
    public void ExitGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
