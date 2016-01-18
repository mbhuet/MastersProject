using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	int playerNum = 0;
	ProgramManager programManager;


	void Awake(){
		programManager = this.GetComponent<ProgramManager>();
		if(isLocalPlayer){
			Debug.Log("Local Player here");
		}

	}

	public override void OnStartLocalPlayer(){
		Debug.Log("OnStartLocalPlayer");
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[playerNum]);
		ProgramUI progUI = GameObject.FindObjectOfType<ProgramUI>();
		progUI.BuildUIFromBlueprint(GameManager.Instance.programProfiles[playerNum]);
	}


	void Update(){

	}


	void SubmitCommands(){
	}

	void RecieveCommands(){
	}


}
