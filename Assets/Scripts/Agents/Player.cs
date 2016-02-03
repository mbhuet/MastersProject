using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	public ProgramManager programManager;
	NetworkPlayer netPlayer;

	[SyncVar]
	public bool isReady = false;

	[SyncVar]
	public int playerNum = -1;

	bool registered = false;


	void Awake(){
		programManager = this.GetComponent<ProgramManager>();
//		Debug.Log("Player awake");
	}

	void Start(){
		if(playerNum != -1)
		Register();
		else{
			Debug.Log("playerNum not getting set before Start()");
		}
	}

	public override void OnStartLocalPlayer(){
		netPlayer = Network.player;
//		Debug.Log("OnStartLocalPlayer");

	}

	void Register(){
		if (registered)
			return;
		Debug.Log("RPC Register as player " + playerNum);
		PlayerManager.Instance.AddPlayer(this, playerNum);
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[playerNum]);
		if(isLocalPlayer){
			PlayerManager.Instance.localPlayer = this;

//			Debug.Log("Local Player here");
//			Debug.Log(PlayerManager.Instance.localPlayer);

			BuildUI();
		}
		registered = true;
	}

	[ClientRpc]
	public void RpcRegister(int num){
		if (registered)
			return;
		Debug.Log("RPC Register as player " + num);
		this.playerNum = num;
		PlayerManager.Instance.AddPlayer(this, num);
		programManager.LoadBlueprint(GameManager.Instance.programProfiles[num]);
		if(isLocalPlayer){
			PlayerManager.Instance.localPlayer = this;
			Debug.Log("Local Player here");
			BuildUI();
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
