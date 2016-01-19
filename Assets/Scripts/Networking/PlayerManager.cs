using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {
	public static PlayerManager Instance;
	public int maxPlayers = 4;

	Player[] players;

	// Use this for initialization
	void Awake () {
		Instance = this;
		players = new Player[maxPlayers];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddPlayer(Player player, int num){
		players[num] = player;
	
	}

	[Server]
	public void RegisterPlayer(Player player){
		bool registered = false;
		for(int i = 0; i< maxPlayers; i++){
			if(players[i] == null && !registered){
				players[i] = player;
				player.playerNum = i;
				player.RpcRegister(i);
				registered = true;
				Debug.Log("Register new Player as " + i);

			}
			else if (players[i] != null){
				players[i].RpcRegister (i);
				Debug.Log("Register existing Player as " + i);
			}
		}
	}

	public void RemovePlayer(int index){
		players[index] = null;
	}
}
