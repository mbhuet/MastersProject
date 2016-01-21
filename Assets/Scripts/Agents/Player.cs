using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	ProgramManager programManager;
	NetworkPlayer netPlayer;

	[SyncVar]
	public bool isReady = false;

	[SyncVar]
	public int playerNum;

	bool registered = false;


	void Awake(){
		programManager = this.GetComponent<ProgramManager>();
	}

	public override void OnStartLocalPlayer(){
		netPlayer = Network.player;
	}

	[ClientRpc]
	public void RpcRegister(int num){
		if (registered)
			return;
		//Debug.Log("RPC Register as player " + num);
		this.playerNum = num;
		PlayerManager.Instance.AddPlayer(this, num);
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[num]);
		if(isLocalPlayer){
			Debug.Log("Local Player here");
			BuildUI();
			PlayerManager.Instance.localPlayer = this;
		}
		registered = true;
	}

	[ClientRpc]
	public void RpcTest(int num){
		Debug.Log("RPC Test " + num);
	}

	public void SetReady(bool isReady){
		this.isReady = isReady;
		CmdPlayerReady (isReady);
		//PlayerManager.Instance.CmdPlayerReady (); //Tells the server to send out a message to all other clients that a player is/ is not ready
	}

	[Command]
	void CmdPlayerReady(bool isReady){
		this.isReady = isReady;
		RpcPlayerReady (isReady);
		if (PlayerManager.Instance.AllPlayersReady ()) {
			ExecutionManager.Instance.TriggerExecutionEvent();
		}
	}

	[ClientRpc]
	void RpcPlayerReady(bool isReady){
		this.isReady = isReady;
		PlayerManager.Instance.UpdateReadyPlayers ();
	}

	void BuildUI(){
//		programManager.LoadBlueprint(GameManager.Instance.programProfiles[playerNum]);
		ProgramUI progUI = GameObject.FindObjectOfType<ProgramUI>();
		progUI.SetLocalProgramManager (programManager);
		progUI.BuildUIFromBlueprint(GameManager.Instance.programProfiles[playerNum]);
	}
}
