using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour {
	public static ExecutionManager Instance;

	//for each player, we need to know the Function and command index at each step. Vector2(Function i, Command i)
	Dictionary<ProgramManager, Vector2> currentCommand;
	ProgramManager[] programManagers;

	public delegate void BeginExecutionDelegate();
	
	[SyncEvent]
	public event BeginExecutionDelegate EventBeginExecution;

	// Use this for initialization
	void Awake () {
		Instance = this;
		EventBeginExecution += BeginExecution;
		currentCommand = new Dictionary<ProgramManager, Vector2>();
		programManagers = new ProgramManager[PlayerManager.Instance.maxPlayers];
	}


	void BeginExecution(){
		currentCommand.Clear();
		Debug.Log ("Begin Execution");
		//Use PlayerManager to get list of Players
		for (int i = 0; i< PlayerManager.Instance.maxPlayers; i++){
			programManagers[i] = PlayerManager.Instance.players[i].GetComponent<ProgramManager>();
		}

		//each player has a program manager with their ants stored
		//execution manager needs to know where all ants inted to be in one step
		//each program manager should update all of its ants one step
		//
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
