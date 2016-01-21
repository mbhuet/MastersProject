using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour {
	public static ExecutionManager Instance;

	public delegate void BeginExecutionDelegate();
	
	[SyncEvent]
	public event BeginExecutionDelegate EventBeginExecution;

	// Use this for initialization
	void Awake () {
		Instance = this;
		EventBeginExecution += BeginExecution;
	}


	void BeginExecution(){
		Debug.Log ("Begin Execution");
	}

	[Server]
	public void TriggerExecutionEvent(){
		EventBeginExecution ();
	}

	[Command]
	public void CmdReady() {
		EventBeginExecution();
	}



}
