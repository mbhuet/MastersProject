using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager Instance;
	public int maxPlayers = 4;

	public int numReadyPlayers = 0;

	
	public Player[] players;
	public Player localPlayer;

	ProgramUI programUI;

	
	// Use this for initialization
	void Awake () {
		Instance = this;
		players = new Player[maxPlayers];
		programUI = GameObject.FindObjectOfType<ProgramUI> ();
		Debug.Log ("PlayerManager Awake");
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
		ProgramUI.Instance.controlCanvas.SetReadyButtonText ("Ready (" + numReadyPlayers + "/" + maxPlayers + ")");
	}

	public void ToggleLocalPlayerReady(){
		localPlayer.ToggleReady ();
	}

	public bool AllPlayersReady(){
		UpdateReadyPlayers ();
//		Debug.Log (numReadyPlayers + "/" + maxPlayers);
		return numReadyPlayers == maxPlayers;
	}


	public void AddPlayer(Player player, int num){
		players[num] = player;
	}

	public void RemovePlayer(int index){
		players[index] = null;
	}
}
