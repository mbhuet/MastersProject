using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour
{
	public static ExecutionManager Instance;
	public static float STEP_TIME = .5f;

	//for each player, we need to know the Function and command index at each step. Vector2(Function i, Command i)
	Dictionary<ProgramManager, Vector3> currentCommandDict; //Vector3(Player index, Function index, command index);
	Dictionary<Vector3, Voxel> intendedNextPositions;
	List<ProgramManager> programManagers_inProgress;
	List<ProgramManager> programManagers_finished;


	public delegate void BeginExecutionDelegate ();
	
	[SyncEvent]
	public event BeginExecutionDelegate
		EventBeginExecution;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
		EventBeginExecution += BeginExecution;
		currentCommandDict = new Dictionary<ProgramManager, Vector3> ();
		intendedNextPositions = new Dictionary<Vector3, Voxel> ();

		programManagers_inProgress = new List<ProgramManager> ();//[PlayerManager.Instance.maxPlayers];
		programManagers_finished = new List<ProgramManager> ();
	}

	void BeginExecution ()
	{
		currentCommandDict.Clear ();
		//Use PlayerManager to get list of Players
		for (int i = 0; i< PlayerManager.Instance.maxPlayers; i++) {
			ProgramManager curProgramManager = PlayerManager.Instance.players [i].GetComponent<ProgramManager> ();
			programManagers_inProgress.Add (curProgramManager);
			currentCommandDict.Add (curProgramManager, Vector3.zero + Vector3.right * i);
		}
		Debug.Log ("Found " + programManagers_inProgress.Count + " ProgramManagers");

		StartCoroutine (Execute ());

		//each player has a program manager with their ants stored
		//execution manager needs to know where all ants inted to be in one step
		//each program manager should update all of its ants one step
	}

	protected IEnumerator Execute ()
	{
		Debug.Log ("Begin Execution");

		while (programManagers_inProgress.Count>0) {
			UpdateCommandCoordinates ();
			FindConflicts ();
			ExecuteStep ();
			yield return new WaitForSeconds (STEP_TIME);
		}
		EndExecution ();
	}

	protected void EndExecution ()
	{
		//TODO signal UI to become interactable
		//TODO signal button to switch to ready
	}

	public void StopExecution ()
	{
		StopCoroutine (Execute ());
		EndExecution ();
	}

	void ExecuteStep ()
	{
		Debug.Log ("Execute Step");

		List<ProgramManager> toRemove = new List<ProgramManager> ();

		foreach (ProgramManager program in programManagers_inProgress) {
			Vector3 currentComCoords;
			currentCommandDict.TryGetValue (program, out currentComCoords);
			//Debug.Log(currentComCoords);
			Command com = ProgramManager.GetCommand (currentComCoords);
			if (com != Command.NONE) {
				program.ExecuteCommand (com);
			} else {
				toRemove.Add (program);
			} 
			currentCommandDict [program] = currentComCoords + Vector3.forward;

		}

		foreach (ProgramManager program in toRemove) {
			programManagers_inProgress.Remove (program);
			programManagers_finished.Add (program);
		}
	}

	protected void UpdateCommandCoordinates ()
	{
		Debug.Log ("Updating Command Coordinates");
		foreach (ProgramManager program in programManagers_inProgress) {
			Vector3 currentComCoords;
			currentCommandDict.TryGetValue (program, out currentComCoords);
			currentComCoords = program.ResolveCalls (currentComCoords);
			Debug.Log ("final coord is " + currentComCoords);
			currentCommandDict [program] = currentComCoords;
		}
	}

	protected void FindConflicts ()
	{
		Debug.Log ("Finding Conflicts");
		intendedNextPositions.Clear ();
		foreach (ProgramManager finishedProgram in programManagers_finished) {
			foreach (Ant ant in finishedProgram.GetAnts()) {
				DeclareVoxelIntention (ant, ant.position);
			}

		}
		foreach (ProgramManager progManager in programManagers_inProgress) {
			Command curCommand = ProgramManager.GetCommand (currentCommandDict [progManager]);
			foreach (Ant ant in progManager.GetAnts()) {

				Vector3 nextPos = ant.positionAfterCommand (curCommand);
				bool validMove = DeclareVoxelIntention (ant, nextPos);

				if (validMove) {
					switch (curCommand) {
					case Command.PUSH:
						Voxel voxAhead = Level.Instance.GetVoxel (ant.position + ant.forwardDirection);
						if (voxAhead.isPushable) {
							DeclareVoxelIntention (voxAhead, voxAhead.position + ant.forwardDirection);
						}
						break;
					case Command.FORWARD:
						//check for switches
						break;
					case Command.BACKWARD:
						//check for switches
						break;
					default:
						break;
					}
				}
			}
		}
	}



	//returns true if can execute as intended
	private bool DeclareVoxelIntention (Voxel vox, Vector3 intendedPos)
	{
		bool valid = true;

		if (!Level.Instance.positionInBounds (intendedPos)) {
			intendedPos = vox.position;
			vox.HoldForStep ();
			valid = false;
		}
		

		if (intendedNextPositions.ContainsKey (intendedPos)) {
			ResolveConflict (vox, intendedNextPositions [intendedPos], intendedPos);
			valid = false;
		} else {
			intendedNextPositions.Add (intendedPos, vox);
		}

		return valid;

		
	}

	private void ResolveConflict (Voxel voxA, Voxel voxB, Vector3 disputedPos)
	{
		//voxA and voxB both want the same position in one step
		intendedNextPositions.Remove (disputedPos);
		voxA.HoldForStep ();
		voxB.HoldForStep ();

		//TODO if voxA or voxB canStackOn, check to see if any Voxel intends to stand on it
		if (intendedNextPositions.ContainsKey (voxA.position)) {
			if (intendedNextPositions [voxA.position] != voxA) {
				ResolveConflict (voxA, intendedNextPositions [voxA.position], voxA.position);
				intendedNextPositions.Add (voxA.position, voxA);
			}


		} else {
			intendedNextPositions.Add (voxA.position, voxA);

		}

		if (intendedNextPositions.ContainsKey (voxB.position)) {
			if (intendedNextPositions [voxB.position] != voxB) {
				ResolveConflict (voxB, intendedNextPositions [voxB.position], voxB.position);
				intendedNextPositions.Add (voxB.position, voxB);
			}

		} else {
			intendedNextPositions.Add (voxB.position, voxB);
		}
	
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
