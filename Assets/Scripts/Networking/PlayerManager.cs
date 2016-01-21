using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour {
	public static PlayerManager Instance;
	public int maxPlayers = 4;

	[SyncVar]
	public int numReadyPlayers = 0;

	
	Player[] players;
	public Player localPlayer;

	ProgramUI programUI;

	
	// Use this for initialization
	void Awake () {
		Instance = this;
		players = new Player[maxPlayers];
		programUI = GameObject.FindObjectOfType<ProgramUI> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateReadyPlayers(){
		int r = 0;
		for (int i = 0; i<players.Length; i++) {
			if(players[i] != null && players[i].isReady) r++;
		}
		numReadyPlayers = r;
		UpdateReadyButtonText (numReadyPlayers);
	}

	void UpdateReadyButtonText(int numReady){
//		Debug.Log ("UpdateReadyButtonText");
		programUI.SetButtonText("Ready (" + numReady + "/" + maxPlayers + ")");
	}
	

	public void AddPlayer(Player player, int num){
		players[num] = player;
	}

	[Server]
	public void RegisterPlayer(Player player){
		bool registered = false;
		for(int i = 0; i< maxPlayers; i++){
			if(players[i] == null && !registered){
				AddPlayer(player, i);
				player.playerNum = i;
				player.RpcRegister(i);
				registered = true;
				Debug.Log("Register new Player as " + i);

			}
			else if (players[i] != null){
				players[i].RpcRegister (i);
				Debug.Log("Register existing Player at " + i);
			}
		}
	}

	public void RemovePlayer(int index){
		players[index] = null;
	}
}
