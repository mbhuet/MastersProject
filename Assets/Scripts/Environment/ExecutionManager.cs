using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour {
	Dictionary<NetworkPlayer, int> testVals;

	// Use this for initialization
	void Start () {
		testVals = new Dictionary<NetworkPlayer, int>();

		if(isClient)
			SetVal(2);
		else if(isServer)
			SetVal(1);
	}
	
	public void SetVal(int val){
	//	CmdSetVal(Network.player, val);
	}




}
