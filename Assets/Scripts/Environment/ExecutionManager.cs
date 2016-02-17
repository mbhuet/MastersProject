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

	public void Init(){
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
			{Command.FORWARD, 2},
			{Command.BACKWARD, 2},
			{Command.FIRE, 3},
			{Command.BUILD, 4}

		};
		programManagers_inProgress = new List<ProgramManager> ();//[PlayerManager.Instance.maxPlayers];
		programManagers_finished = new List<ProgramManager> ();
		movingVoxels = new List<Voxel> ();
	}


	public static void Pause(){
		paused = true;
	}
	public static void Resume(){
		paused = false;
	}


	void BeginExecution ()
	{
		ProgramUI.Instance.controlCanvas.ShowRuntimeControls ();
		PlayerManager.Instance.localPlayer.SetReady (false);



		//Use PlayerManager to get list of Players
		for (int i = 0; i< PlayerManager.Instance.maxPlayers; i++) {
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
			while(!ReadyForNextStep()){ 
//				Debug.Log("waiting for ReadyForNextStep");
				yield return null;
			}
			foreach(Switch switchVox in Level.Instance.switches){
				switchVox.CheckPressed();
			}
		}
		EndExecution ();
	}

	protected bool ReadyForNextStep(){
		//make sure everything is settled before beginning another step
		return (!paused && movingVoxels.Count == 0);
	}

	protected void EndExecution ()
	{
		Debug.Log ("----------------End Execution--------------");

		ProgramUI.Instance.ResetPlayHead ();
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
			if (vox.NextAction != null)
			vox.NextAction();
		}

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
	//	Debug.Log ("Updating Command Coordinates");
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
			vox.ClearIntention();
		}
	//	Debug.Log ("Finding Conflicts");


		//SWITCHES
		foreach (SwitchBlock switchBlock in Level.Instance.switchBlocks) {
			if(switchBlock.willMove){
				DeclareVoxelIntention(switchBlock, switchBlock.NextPosition(), switchBlock.ChangePosition, -1, true);
			}
			else{
				DeclareVoxelIntention (switchBlock, switchBlock.position, switchBlock.Stay, -1, true);
			}
		}


		//CRATES
		foreach (Crate crate in Level.Instance.crates) {
			if(crate.isActive)
			DeclareVoxelIntention(crate, crate.position, crate.Stay, 10, true);
		}


		//FINISHED ANTS
		foreach (ProgramManager finishedProgram in programManagers_finished) {
			foreach (Ant ant in finishedProgram.GetAnts()) {
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



	private void DeclareVoxelIntention (DynamicVoxel vox, Vector3 intendedPos, Voxel.IntentionDelegate intendedFunc, int priority, bool forceIntention)
	{
		Debug.Log ("Declare Voxel Intention " + vox + " pos: " + intendedPos + " func: " + intendedFunc);

		Vector3 oldIntendedPos = vox.GetIntendedPosition ();
		if (intendedNextPositions.ContainsKey (oldIntendedPos) && intendedNextPositions [oldIntendedPos] == vox) {
			intendedNextPositions.Remove(oldIntendedPos);
		}


		if (!forceIntention){
			bool valid = positionPossibleNextStep (intendedPos);
			if (!valid) {
				intendedPos = vox.position;
				vox.HoldForStep ();
			}
		}

		vox.SetIntention (intendedPos, priority, intendedFunc);


		if (intendedNextPositions.ContainsKey (intendedPos)) {
			ResolveConflict (vox, intendedNextPositions [intendedPos], intendedPos);
		} else {
			intendedNextPositions.Add (intendedPos, vox);
		}

		Voxel voxAbove = Level.Instance.GetVoxel(vox.position + Vector3.up);
		if (voxAbove != null){
			if(voxAbove.GetType() == typeof(DynamicVoxel)){
				DynamicVoxel dynVoxAbove = voxAbove.GetComponent<DynamicVoxel>();
				dynVoxAbove.HoldForStep();
				DeclareVoxelIntention(dynVoxAbove, intendedPos + Vector3.up, dynVoxAbove.MoveDirection(intendedPos - vox.position), priority, true);
			}
		}
	}

	private void DeclareAntIntention (Ant ant, Command command)
	{
		Vector3 intendedPos = ant.positionAfterCommand (command);
		bool valid = positionPossibleNextStep(intendedPos);
		Debug.Log (ant + " intends to " + command + "\nintendedPos is " + intendedPos + " valid = " + valid);

		if (!valid) {
			DeclareVoxelIntention(ant, ant.position, ant.Wait, 0, false);
		}

		else{
		switch (command) {
		case Command.FORWARD:
			DeclareVoxelIntention(ant, intendedPos, ant.MoveForward, commandPriority[command], false);
			break;
		case Command.BACKWARD:
			DeclareVoxelIntention(ant, intendedPos, ant.MoveBackward, commandPriority[command], false);
			break;
		case Command.TURN_L:
			DeclareVoxelIntention(ant, intendedPos, ant.TurnLeft, commandPriority[command], false);
			break;
		case Command.TURN_R:
			DeclareVoxelIntention(ant, intendedPos, ant.TurnRight, commandPriority[command], false);
			break;
		case Command.PUSH:
			Debug.Log("Declare Push");
			if (intendedNextPositions.ContainsKey (intendedPos)) {
				DynamicVoxel pushPosVox;
				intendedNextPositions.TryGetValue(intendedPos, out pushPosVox);
				Debug.Log("pushPosVox " + ((pushPosVox == null) ? "null" : pushPosVox.name));
				//if this ant is pushing, and the voxel ahead is pushable, and it currently intends to stay put
				if(command == Command.PUSH && pushPosVox.isPushable && pushPosVox.position == pushPosVox.GetIntendedPosition()){
					Debug.Log("pushable and staying put");

					DeclareVoxelIntention(pushPosVox, pushPosVox.position + ant.forwardDirection, pushPosVox.MoveDirection(ant.forwardDirection), commandPriority[command], false);
					DeclareVoxelIntention(ant, intendedPos, ant.Push, commandPriority[command], false);
				}
				else{
					DeclareVoxelIntention(ant, ant.position, ant.Wait, commandPriority[Command.WAIT], false);
				}
			}
			break;
		case Command.FIRE:
			DeclareVoxelIntention(ant, intendedPos, ant.Fire, commandPriority[Command.FIRE], false);
			DynamicVoxel firePosVox;
			intendedNextPositions.TryGetValue(ant.position + ant.forwardDirection, out firePosVox);
			if(firePosVox != null && firePosVox.isBurnable && firePosVox.GetIntendedPosition() == firePosVox.position){
				intendedNextPositions.Remove(firePosVox.position);
				DeclareVoxelIntention(firePosVox, Vector3.one * -1, firePosVox.Burn, commandPriority[Command.FIRE], false);
			}
			break;
		case Command.BUILD:
			Debug.Log("Declare Build");
			DynamicVoxel buildPosVox;
			intendedNextPositions.TryGetValue(ant.position + ant.forwardDirection, out buildPosVox);
			if(buildPosVox == null){
				DeclareVoxelIntention(ant, ant.position, ant.Build, commandPriority[Command.BUILD], false);
				Crate buildCrate = ant.GetBuildCrate();
				DeclareVoxelIntention(buildCrate, ant.position + ant.forwardDirection, buildCrate.Assemble, commandPriority[Command.BUILD], false);
			}
			else{
					Debug.Log("Build Pos occupied by " + buildPosVox);
				DeclareVoxelIntention(ant, intendedPos, ant.Wait, commandPriority[Command.WAIT], false);
				valid = false;
			}
			break;
		default:
			DeclareVoxelIntention(ant, ant.positionAfterCommand(command), ant.Wait, commandPriority[Command.WAIT], false);
			break;
		}
		}
		
	}

	private void ResolveConflict (DynamicVoxel voxA, DynamicVoxel voxB, Vector3 disputedPos)
	{
		Debug.Log ("Conflict between " + voxA + " and " + voxB + " over " + disputedPos);
		intendedNextPositions.Remove (disputedPos);

		int voxAPriority = voxA.GetIntentionPriority ();
		int voxBPriority = voxB.GetIntentionPriority ();

		//EAUAl priority, no one gets to move
		if (voxAPriority == voxBPriority) {
			Debug.Log ("Equal Priority, no one moves");

			voxA.HoldForStep ();
			voxB.HoldForStep ();
			DeclareVoxelIntention(voxA, voxA.position, voxA.Stay, -1, true);
			DeclareVoxelIntention(voxB, voxB.position, voxB.Stay, -1, true);

		} else if (voxAPriority < voxBPriority) {
			Debug.Log (voxA + " has priority.");

			DeclareVoxelIntention(voxA, disputedPos, voxA.GetIntendedFunc(), voxAPriority, true);
			voxB.HoldForStep ();
			DeclareVoxelIntention(voxB, voxB.position, voxB.Stay, -1, true);

		} else if (voxBPriority < voxAPriority) {
			Debug.Log (voxB + " has priority.");

			DeclareVoxelIntention(voxB, disputedPos, voxB.GetIntendedFunc(), voxBPriority, true);
			voxA.HoldForStep ();
			DeclareVoxelIntention(voxA, voxA.position, voxA.Stay, -1, true);
		}	
	}

	bool positionPossibleNextStep(Vector3 intendedPos){
		if (!Level.Instance.positionInBounds(intendedPos)) {
			Debug.Log("position " + intendedPos + " not in boudns");
			return false;
		}

		Vector3 intendedFloorPos = intendedPos + Vector3.down;
		Voxel intendedFloor = null;

		if (intendedNextPositions.ContainsKey (intendedFloorPos)) {
			intendedFloor = intendedNextPositions [intendedFloorPos];
		} else if (Level.Instance.GetVoxel (intendedFloorPos) != null && Level.Instance.GetVoxel(intendedFloorPos).isStatic) {
			intendedFloor = Level.Instance.GetVoxel(intendedFloorPos);
		}
//		Debug.Log ("intendedFloorPos " + intendedFloorPos + ((intendedFloor == null) ? " is empty " : " has vox " + intendedFloor));


		bool validFloor = (intendedFloor != null && intendedFloor.canStackOn && intendedFloor.position == intendedFloorPos);
		if(intendedFloor != null)
//		Debug.Log (validFloor + "/n" + "intended floor " + intendedFloor + "/nintendedFloor.canStackOn " + intendedFloor.canStackOn + "/nintendedFloor.position" + intendedFloor.position);
		if (!validFloor) {
			return false;
		}


		return true;
	
	}

	List<ProgramManager> SortProgramsByCommandPriority(List<ProgramManager> list){
		if (list.Count <= 1)
			return list;
		List<ProgramManager> sortedList = new List<ProgramManager> ();
		foreach (ProgramManager program in list) {
			Command com = ProgramManager.GetCommand (currentCommandDict[program]);
			int priority = commandPriority[com];
			int i = 0;
			while(i<sortedList.Count && commandPriority[ProgramManager.GetCommand (currentCommandDict[sortedList[i]])] <= priority){

				i++;
			}
			sortedList.Insert(i, program);
		}
		return sortedList;
	}

	public void AddMovingVoxel(Voxel vox){
		if(!movingVoxels.Contains(vox))
		movingVoxels.Add (vox);
	}

	public void RemoveMovingVoxel(Voxel vox){
		if (movingVoxels.Contains (vox))
			movingVoxels.Remove (vox);
	}

	[Server]
	public void TriggerExecutionEvent ()
	{
		EventBeginExecution ();
	}



}
