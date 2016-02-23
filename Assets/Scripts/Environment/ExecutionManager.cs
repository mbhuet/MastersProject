using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ExecutionManager : NetworkBehaviour
{
	public static ExecutionManager Instance;
	public static float STEP_TIME = .5f;
	static bool paused = false;

	//for each player, we need to know the Function and command index at each step. Vector2(Function i, Command i)
	Dictionary<ProgramManager, Vector3> currentCommandDict; //Vector3(Player index, Function index, command index);
	Dictionary<Vector3, DynamicVoxel> intendedNextPositions;
	Dictionary<Command, int> commandPriority;
	List<ProgramManager> programManagers_inProgress;
	List<ProgramManager> programManagers_finished;
	List<Voxel> movingVoxels;

	public delegate void BeginExecutionDelegate ();
	
	[SyncEvent]
	public event BeginExecutionDelegate
		EventBeginExecution;

	public void Init ()
	{
		Instance = this;
	}

	// Use this for initialization
	void Awake ()
	{
		Instance = this;

		Debug.Log ("ExecutionManager Awake");
		EventBeginExecution += BeginExecution;
		currentCommandDict = new Dictionary<ProgramManager, Vector3> ();
		intendedNextPositions = new Dictionary<Vector3, DynamicVoxel> ();
		commandPriority = new Dictionary<Command, int>{
			{Command.WAIT, 0},
			{Command.NONE, 0},
			{Command.SENSE, 0},
			{Command.TURN_R, 0},
			{Command.TURN_L, 0},
			{Command.PUSH, 1},
			{Command.FIRE, 2},
			{Command.FORWARD, 3},
			{Command.BACKWARD, 3},
			{Command.BUILD, 4}


		};
		programManagers_inProgress = new List<ProgramManager> ();//[PlayerManager.Instance.maxPlayers];
		programManagers_finished = new List<ProgramManager> ();
		movingVoxels = new List<Voxel> ();
	}

	public static void Pause ()
	{
		paused = true;
	}

	public static void Resume ()
	{
		paused = false;
	}

	void BeginExecution ()
	{
		ProgramUI.Instance.controlCanvas.ShowRuntimeControls ();
		ProgramUI.Instance.DisableInteractivity ();
		PlayerManager.Instance.localPlayer.SetReady (false);
		//Debug.Log ("Begin Execution");


		//Use PlayerManager to get list of Players
		for (int i = 0; i< GameManager.Instance.numPlayers; i++) {
			ProgramManager curProgramManager = PlayerManager.Instance.players [i].GetComponent<ProgramManager> ();
			programManagers_inProgress.Add (curProgramManager);
			currentCommandDict.Add (curProgramManager, Vector3.zero + Vector3.right * i);
		}
//		Debug.Log ("Found " + programManagers_inProgress.Count + " ProgramManagers");

		StartCoroutine ("Execute");

		//each player has a program manager with their ants stored
		//execution manager needs to know where all ants inted to be in one step
		//each program manager should update all of its ants one step
	}

	protected IEnumerator Execute ()
	{
		Debug.Log ("-------------Begin Execution---------------");

		while (programManagers_inProgress.Count>0) {
			UpdateCommandCoordinates ();
			FindConflicts ();
			ExecuteStep ();
			yield return new WaitForSeconds (STEP_TIME);
			while (!ReadyForNextStep()) { 
//				Debug.Log("waiting for ReadyForNextStep");
				yield return null;
			}
			foreach (Switch switchVox in Level.Instance.switches) {
				switchVox.CheckPressed ();
			}
		}
		EndExecution ();
	}

	protected bool ReadyForNextStep ()
	{
		//make sure everything is settled before beginning another step
		return (!paused && movingVoxels.Count == 0);
	}

	protected void EndExecution ()
	{
		Debug.Log ("----------------End Execution--------------");

		ProgramUI.Instance.ResetPlayHead ();
		ProgramUI.Instance.EnableInteractivity ();

		currentCommandDict.Clear ();
		programManagers_inProgress.Clear ();
		programManagers_finished.Clear ();
		movingVoxels.Clear ();
		intendedNextPositions.Clear ();
		paused = false;

		//TODO signal UI to become interactable
		//TODO signal button to switch to ready
	}

	public void StopExecution ()
	{
		StopCoroutine ("Execute");
		EndExecution ();
	}

	void ExecuteStep ()
	{
		Debug.Log ("Begin Execute Step");

		List<ProgramManager> toRemove = new List<ProgramManager> ();

		foreach (DynamicVoxel vox in Level.Instance.dynamicVoxels) {
			if (vox.MoveAction != null)
				vox.MoveAction ();
			if (vox.StillAction != null)
				vox.StillAction ();
		}

		foreach (ProgramManager program in programManagers_inProgress) {
			Vector3 currentComCoords;
			currentCommandDict.TryGetValue (program, out currentComCoords);
			Debug.Log(currentComCoords);
			Command com = ProgramManager.GetCommand (currentComCoords);
			if (com == Command.NONE) {
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
//			Debug.Log ("final coord is " + currentComCoords);
			currentCommandDict [program] = currentComCoords;
		}
	}

	protected void FindConflicts ()
	{
		intendedNextPositions.Clear ();
		foreach (DynamicVoxel vox in Level.Instance.dynamicVoxels) {
			vox.ClearIntention ();
		}
		//	Debug.Log ("Finding Conflicts");


		//SWITCHES
		foreach (SwitchBlock switchBlock in Level.Instance.switchBlocks) {
			if (switchBlock.willMove) {
				DeclareVoxelIntention (switchBlock, switchBlock, switchBlock.NextPosition (), switchBlock.ChangePosition, true, -1, true);
			} else {
				DeclareVoxelIntention (switchBlock, switchBlock, switchBlock.position, switchBlock.Stay, false, -1, true);
			}
		}


		//CRATES
		foreach (Crate crate in Level.Instance.crates) {
//			Debug.Log("checking crates");
			if (crate.isActive){
//				Debug.Log(crate);

				DeclareVoxelIntention (crate, crate, crate.position, crate.Stay, false, 10, true);
			}
		}


		//FINISHED ANTS
		foreach (ProgramManager finishedProgram in programManagers_finished) {
			foreach (Ant ant in finishedProgram.GetAnts()) {
//				Debug.Log ("Finished ant will Wait");
				DeclareAntIntention (ant, Command.WAIT);
			}

		}

		//ACTIVE ANTS
		programManagers_inProgress = SortProgramsByCommandPriority (programManagers_inProgress);
		foreach (ProgramManager progManager in programManagers_inProgress) {
			Command curCommand = ProgramManager.GetCommand (currentCommandDict [progManager]);
			foreach (Ant ant in progManager.GetAnts()) {
				DeclareAntIntention (ant, curCommand);
			}
		}

	}


	/*
	 * If this voxel has already delcared an intention, remove it from the dict
	 *		if new intention doesn't work out, it will just get added back
	 * Check to see if this intention is possible
	 * 		if not, convert this into a Stay
	 * 		exception, if this voxel has already been moved by another, i don't want to override that intention, must merge it
	 *
	 * if this voxel will be moving, check to see if there's anything on top that will need to come along.
	 */
	private void DeclareVoxelIntention (DynamicVoxel vox, DynamicVoxel motivator, Vector3 intendedPos, Voxel.IntentionDelegate intendedFunc, bool involvesMovement, int priority, bool forceIntention)
	{
		Debug.Log ("Declare Voxel Intention " + vox + " pos: " + intendedPos + " func: " + intendedFunc);
		Vector3 oldIntendedPos = vox.GetIntendedPosition ();
		if (intendedNextPositions.ContainsKey (oldIntendedPos) && intendedNextPositions [oldIntendedPos] == vox) {
			intendedNextPositions.Remove (oldIntendedPos);
		}

		if (involvesMovement && !forceIntention) {
			bool valid = positionPossibleNextStep (vox, intendedPos);
			if (!valid) {
				intendedPos = vox.position;
				vox.HoldForStep ();
				intendedFunc = vox.Stay;
				priority = 0;
				involvesMovement = false;
			}
		}

		vox.SetIntention (motivator, intendedPos, priority, intendedFunc, involvesMovement);

		intendedPos = vox.GetIntendedPosition ();

		if (intendedNextPositions.ContainsKey (intendedPos) && intendedNextPositions [intendedPos] != vox) {
			ResolveConflict (vox, intendedNextPositions [intendedPos], intendedPos);
		} else {
			intendedNextPositions.Add (vox.GetIntendedPosition (), vox);
		}

		if (involvesMovement) {
//			Debug.Log("involvesMovement true");
			Vector3 abovePos = vox.position + Vector3.up;
			Voxel voxAbove = Level.Instance.GetVoxel (abovePos);
//			Debug.Log("voxAbove at pos " + (abovePos) + " ==  null " + (voxAbove == null));

			if (voxAbove != null) {
				DynamicVoxel dynVoxAbove = voxAbove.GetComponent<DynamicVoxel> ();
				if (dynVoxAbove != null) {
//					Debug.Log (dynVoxAbove + " is above " + vox + " and will be moved with it");
					dynVoxAbove.HoldForStep ();
					DeclareVoxelIntention (dynVoxAbove, vox, intendedPos + Vector3.up, dynVoxAbove.MoveDirection (intendedPos - vox.position), true, priority, false);
				}
			}
		}
	}

	private void DeclareAntIntention (Ant ant, Command command)
	{
		Vector3 intendedPos = ant.positionAfterCommand (command);
		Debug.Log (ant + " intends to " + command + "\nintendedPos is " + intendedPos);
		if (command == Command.BACKWARD ||
			command == Command.FORWARD ||
			command == Command.PUSH) {
			bool valid = positionPossibleNextStep (ant, intendedPos);
//			Debug.Log ("valid = " + valid);

			if (!valid) {
				command = Command.WAIT;
			}
		}

	
		switch (command) {
		case Command.FORWARD:
			DeclareVoxelIntention (ant, ant, intendedPos, ant.MoveForward, true, commandPriority [command], false);
			break;
		case Command.BACKWARD:
			DeclareVoxelIntention (ant, ant, intendedPos, ant.MoveBackward, true, commandPriority [command], false);
			break;
		case Command.TURN_L:
			DeclareVoxelIntention (ant, ant, intendedPos, ant.TurnLeft, false, commandPriority [command], false);
			break;
		case Command.TURN_R:
			DeclareVoxelIntention (ant, ant, intendedPos, ant.TurnRight, false, commandPriority [command], false);
			break;
		case Command.PUSH:
//			Debug.Log ("Declare Push");
			if (intendedNextPositions.ContainsKey (intendedPos)) {
				DynamicVoxel pushPosVox;
				intendedNextPositions.TryGetValue (intendedPos, out pushPosVox);
//				Debug.Log ("pushPosVox " + ((pushPosVox == null) ? "null" : pushPosVox.name));
				//if this ant is pushing, and the voxel ahead is pushable, and it currently intends to stay put
				if (command == Command.PUSH && pushPosVox.isPushable && pushPosVox.position == pushPosVox.GetIntendedPosition ()) {
//					Debug.Log ("pushable and staying put");

					DeclareVoxelIntention (pushPosVox, ant, pushPosVox.position + ant.forwardDirection, pushPosVox.MoveDirection (ant.forwardDirection), true, commandPriority [command], false);
					DeclareVoxelIntention (ant, ant, intendedPos, ant.Push, true, commandPriority [command], false);
				} else {
					DeclareVoxelIntention (ant, ant, ant.position, ant.Wait, false, commandPriority [Command.WAIT], false);
				}
			}
			break;
		case Command.FIRE:
//			Debug.Log ("Declare Fire");
			DeclareVoxelIntention (ant, ant, intendedPos, ant.Fire, false, commandPriority [Command.FIRE], false);
			DynamicVoxel firePosVox;
			intendedNextPositions.TryGetValue (ant.position + ant.forwardDirection, out firePosVox);
//			Debug.Log("Fire pos " + ant.position + ant.forwardDirection + " firePosBox = " + ((firePosVox == null)? "null" : firePosVox.name));
			if (firePosVox != null && firePosVox.isBurnable && firePosVox.GetIntendedPosition () == firePosVox.position) {
//				Debug.Log("Will burn " + firePosVox);
				intendedNextPositions.Remove (firePosVox.position);
				DeclareVoxelIntention (firePosVox, ant, Vector3.one * -1, firePosVox.Burn, false, commandPriority [Command.FIRE], false);
			}
			break;
		case Command.BUILD:
//			Debug.Log ("Declare Build");
			DynamicVoxel buildPosVox;
			intendedNextPositions.TryGetValue (ant.position + ant.forwardDirection, out buildPosVox);
			if (buildPosVox == null) {
				DeclareVoxelIntention (ant, ant, ant.position, ant.Build, false, commandPriority [Command.BUILD], false);
				Crate buildCrate = ant.GetBuildCrate ();
				DeclareVoxelIntention (buildCrate, ant, ant.position + ant.forwardDirection, buildCrate.Assemble, false, commandPriority [Command.BUILD], false);
			} else {
//				Debug.Log ("Build Pos occupied by " + buildPosVox);
				DeclareVoxelIntention (ant, ant, intendedPos, ant.Wait, false, commandPriority [Command.WAIT], false);
			}
			break;
		default:
			DeclareVoxelIntention (ant, ant, ant.positionAfterCommand (Command.WAIT), ant.Wait, false, commandPriority [Command.WAIT], false);
			break;
		
		}
		
	}

	private void ResolveConflict (DynamicVoxel voxA, DynamicVoxel voxB, Vector3 disputedPos)
	{
//		Debug.Log ("Conflict between " + voxA + " and " + voxB + " over " + disputedPos);
		intendedNextPositions.Remove (disputedPos);

		int voxAPriority = voxA.GetIntentionPriority ();
		int voxBPriority = voxB.GetIntentionPriority ();

		//EAUAl priority, no one gets to move
		if (voxAPriority == voxBPriority) {
//			Debug.Log ("Equal Priority " + voxAPriority + ", no one moves");

			voxA.HoldForStep ();
			voxB.HoldForStep ();
			DeclareVoxelIntention (voxA, voxB, voxA.position, voxA.Stay, true, 0, true);
			DeclareVoxelIntention (voxB, voxA, voxB.position, voxB.Stay, true, 0, true);

		} else if (voxAPriority < voxBPriority) {
//			Debug.Log (voxA + " has priority.");

			DeclareVoxelIntention (voxA, voxA, disputedPos, voxA.GetIntendedMoveFunc (), true, voxAPriority, true);
			voxB.HoldForStep ();
			DeclareVoxelIntention (voxB, voxA, voxB.position, voxB.Stay, true, 0, true);

		} else if (voxBPriority < voxAPriority) {
//			Debug.Log (voxB + " has priority.");

			DeclareVoxelIntention (voxB, voxB, disputedPos, voxB.GetIntendedMoveFunc (), true, voxBPriority, true);
			voxA.HoldForStep ();
			DeclareVoxelIntention (voxA, voxB, voxA.position, voxA.Stay, true, 0, true);
		}	
	}

	bool positionPossibleNextStep (Voxel vox, Vector3 intendedPos)
	{
		if (!Level.Instance.positionInBounds (vox, intendedPos)) {
//			Debug.Log ("position " + intendedPos + " not in bounds");
			return false;
		}

		Vector3 intendedFloorPos = intendedPos + Vector3.down;
		Voxel intendedFloor = null;

		if (intendedNextPositions.ContainsKey (intendedFloorPos)) {
			intendedFloor = intendedNextPositions [intendedFloorPos];
		} else if (Level.Instance.GetVoxel (intendedFloorPos) != null && Level.Instance.GetVoxel (intendedFloorPos).isStatic) {
			intendedFloor = Level.Instance.GetVoxel (intendedFloorPos);
		}


		bool validFloor = (intendedFloor != null && intendedFloor.canStackOn && intendedFloor.GetIntendedPosition () == intendedFloorPos);
//		Debug.Log ("intendedFloorPos " + intendedFloorPos + ((intendedFloor == null) ? " is empty " : (" has vox " + intendedFloor + " intends to be in place  = " + (intendedFloor.GetIntendedPosition () == intendedFloorPos))));

		if (!validFloor) {
			return false;
		}


		return true;
	
	}

	List<ProgramManager> SortProgramsByCommandPriority (List<ProgramManager> list)
	{
		if (list.Count <= 1)
			return list;
		List<ProgramManager> sortedList = new List<ProgramManager> ();
		foreach (ProgramManager program in list) {
			Command com = ProgramManager.GetCommand (currentCommandDict [program]);
			int priority = commandPriority [com];
			int i = 0;
			while (i<sortedList.Count && commandPriority[ProgramManager.GetCommand (currentCommandDict[sortedList[i]])] <= priority) {

				i++;
			}
			sortedList.Insert (i, program);
		}
		return sortedList;
	}

	public void AddMovingVoxel (Voxel vox)
	{
		if (!movingVoxels.Contains (vox))
			movingVoxels.Add (vox);
	}

	public void RemoveMovingVoxel (Voxel vox)
	{
		if (movingVoxels.Contains (vox))
			movingVoxels.Remove (vox);
	}

	[Server]
	public void TriggerExecutionEvent ()
	{
		EventBeginExecution ();
	}



}
