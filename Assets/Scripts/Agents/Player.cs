using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
	AntType antClass;
	int playerNum;


	/*
	 * Player objects should spawn at (0,0,0) and know which class of ant they will be controlling
	 * Should have a list of command tiles
	 * 
	 * Should be able to send their command list, with class of Ant to the server
	 * Recieve command lists for other Ant classes
	 */ 

	[Server]
	void DeclareReady(){
	}

	void SubmitCommands(){
	}

	void RecieveCommands(){
	}


	[Client]
	public void Test(){
	
	
	}
}
