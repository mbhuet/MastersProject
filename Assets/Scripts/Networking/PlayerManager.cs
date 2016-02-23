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
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadLevelProfiles(){
		for (int i = 0; i< GameManager.Instance.numPlayers; i++) {
			if(players[i] == null) Debug.LogError("not enough players in this scene, that shouldn't happen");
			else players[i].LoadLevelProfile();
		}
	}

	public void UpdateReadyPlayers(){
		int r = 0;
		for (int i = 0; i<players.Length; i++) {
			if(players[i] != null && players[i].isReady) r++;
		}
		numReadyPlayers = r;
		ProgramUI.Instance.controlCanvas.SetReadyButtonText ("Ready (" + numReadyPlayers + "/" + GameManager.Instance.numPlayers + ")");
	}

	public void ToggleLocalPlayerReady(){
		localPlayer.ToggleReady ();
	}

	public bool AllPlayersReady(){
		UpdateReadyPlayers ();
//		Debug.Log (numReadyPlayers + "/" + maxPlayers);
		return numReadyPlayers == GameManager.Instance.numPlayers;
	}


	public void AddPlayer(Player player, int num){
		players[num] = player;
	}

	public void RemovePlayer(int index){
		players[index] = null;
	}
}
