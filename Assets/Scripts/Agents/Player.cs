using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	ProgramManager programManager;
	NetworkPlayer netPlayer;

	[SyncVar]
	bool isReady = false;

	[SyncVar]
	public int playerNum;


	void Awake(){
		programManager = this.GetComponent<ProgramManager>();
	}

	public override void OnStartLocalPlayer(){
		netPlayer = Network.player;
	}

	[ClientRpc]
	public void RpcRegister(int num){
		//Debug.Log("RPC Register as player " + num);
		this.playerNum = num;
		PlayerManager.Instance.AddPlayer(this, num);
		if(isLocalPlayer){
			BuildUI();
		}
	}

	[ClientRpc]
	public void RpcTest(int num){
		Debug.Log("RPC Test " + num);
	}

	void BuildUI(){
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[playerNum]);
		ProgramUI progUI = GameObject.FindObjectOfType<ProgramUI>();
		progUI.BuildUIFromBlueprint(GameManager.Instance.programProfiles[playerNum]);
	}
}
