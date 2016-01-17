using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	int playerNum;
	ProgramManager programManager;


	void Awake(){
		programManager = this.GetComponent<ProgramManager>();
	}

	public void Init(int playerNum){
		this.playerNum = playerNum;
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[playerNum]);

		ProgramUI progUI = GameObject.FindObjectOfType<ProgramUI>();
		progUI.BuildUIFromBlueprint(GameManager.Instance.programProfiles[playerNum]);
	}


	void SubmitCommands(){
	}

	void RecieveCommands(){
	}


}
