using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	int playerNum;
	ProgramManager programManager;


	void Start(){
		programManager = this.GetComponent<ProgramManager>();
		int num = NetworkServer.connections.Count;
		Init(num);
	}


	void Init(int playerNum){
		this.playerNum = playerNum;
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[playerNum]);
	}


	void SubmitCommands(){
	}

	void RecieveCommands(){
	}

}
