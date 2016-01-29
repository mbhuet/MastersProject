using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour
{
	public static ExecutionManager Instance;

	//for each player, we need to know the Function and command index at each step. Vector2(Function i, Command i)
	Dictionary<ProgramManager, Vector2> currentCommandDict;
	List<ProgramManager> programManagers;

	public delegate void BeginExecutionDelegate ();
	
	[SyncEvent]
	public event BeginExecutionDelegate
		EventBeginExecution;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
		EventBeginExecution += BeginExecution;
		currentCommandDict = new Dictionary<ProgramManager, Vector2> ();
		programManagers = new List<ProgramManager> ();//[PlayerManager.Instance.maxPlayers];
	}

	void BeginExecution ()
	{
		currentCommandDict.Clear ();
		//Use PlayerManager to get list of Players
		for (int i = 0; i< PlayerManager.Instance.maxPlayers; i++) {
			ProgramManager curProgramManager = PlayerManager.Instance.players [i].GetComponent<ProgramManager> ();
			programManagers.Add (curProgramManager);
			currentCommandDict.Add (curProgramManager, Vector2.zero);
		}
		Debug.Log("Found " + programManagers.Count + " ProgramManagers");

		StartCoroutine(Execute());

		//each player has a program manager with their ants stored
		//execution manager needs to know where all ants inted to be in one step
		//each program manager should update all of its ants one step
	}

	protected IEnumerator Execute ()
	{
		Debug.Log ("Begin Execution");

		while (programManagers.Count>0) {
			ExecuteStep ();
			yield return new WaitForSeconds (1);
		}
		EndExecution ();
	}


	protected void EndExecution(){
		//TODO signal UI to become interactable
		//TODO signal button to switch to ready
	}

	public void StopExecution(){
		StopCoroutine (Execute());
		EndExecution ();
	}

	void ExecuteStep ()
	{
		Debug.Log ("Execute Step");

		List<ProgramManager> toRemove = new List<ProgramManager>();
		foreach (ProgramManager program in programManagers) {
			Vector2 currentCom;
			currentCommandDict.TryGetValue (program, out currentCom);
			Command com = program.GetCommand (currentCom);
			if (com != Command.NONE) {
				program.ExecuteCommand (com);
			}
			Vector2 nextCom = program.GetFollowingCommandCoordinates (currentCom);

			if (program.GetCommand(nextCom) == Command.NONE) {
				toRemove.Add(program);
			} else {
				currentCommandDict[program] = nextCom;
			}
		}

		foreach(ProgramManager program in toRemove){
			programManagers.Remove(program);
		}
	}

	protected void FindConflicts(){
		//GET grid from Level.Instance
		//For each ant, get its next position
		//Condition handling
			//if pushing, update box position
			//if on switch, update switch box position
		//TODO must tell Ants that will be in conflict to run error coroutine
	}

	[Server]
	public void TriggerExecutionEvent ()
	{
		EventBeginExecution ();
	}

	[Command]
	public void CmdReady ()
	{
		EventBeginExecution ();
	}



}
