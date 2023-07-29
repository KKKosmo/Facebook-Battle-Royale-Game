using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cinemachine;
public class score : MonoBehaviour
{
    public Transform spectatePoint;
    [SerializeField] Button btn;

    [SerializeField] CinemachineVirtualCamera cam;
	public int kills = 0;
	public bool alive = true;
	void Start () {
		cam = GameObject.Find("VirtualCamera1").GetComponent<CinemachineVirtualCamera>();
		btn.onClick.AddListener(spectate);
	}

	public void spectate(){
		cam.Follow = spectatePoint;
		cam.LookAt = spectatePoint;
		// Debug.Log ("You have clicked the button!");
	}
}
