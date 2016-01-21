using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour {
	public static ExecutionManager Instance;

	public delegate void BeginExecutionDelegate();
	
	[SyncEvent]
	public event BeginExecutionDelegate EventBeginExecution;

	public delegate void TestDelegate();
	
	[SyncEvent]
	public event TestDelegate EventTest;



	// Use this for initialization
	void Awake () {
		Instance = this;
		EventBeginExecution += BeginExecution;
		EventTest += Test;

	}


	void BeginExecution(){
		Debug.Log ("Begin Execution");
	}

	void Test(){
		Debug.Log ("test successful");
	}

	[Command]
	public void CmdReady() {
		EventBeginExecution();
	}

	[Command]
	public void CmdTest() {
		EventTest();
	}




}
