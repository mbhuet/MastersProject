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
	Dictionary<Vector3, Voxel> intendedNextPositions;
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
		intendedNextPositions = new Dictionary<Vector3, Voxel> ();

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
				Debug.Log("waiting for ReadyForNextStep");
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

		foreach (SwitchBlock switchBlock in Level.Instance.switchBlocks) {
			if(switchBlock.willMove){
				switchBlock.ChangePosition();
			}
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

	//	Debug.Log ("Finding Conflicts");


		//SWITCHES
		foreach (SwitchBlock switchBlock in Level.Instance.switchBlocks) {
//			Debug.Log(switchBlock);
			if(switchBlock.willMove){
//				Debug.Log(switchBlock + " will move to " + switchBlock.NextPosition());
				intendedNextPositions.Add (switchBlock.NextPosition(), switchBlock);
				Voxel voxAbove = Level.Instance.GetVoxel(switchBlock.position + Vector3.up);
				if (voxAbove != null){
					voxAbove.HoldForStep();
				}
			}
			else{
//				Debug.Log(switchBlock + " will stay at " + switchBlock.position);

				intendedNextPositions.Add (switchBlock.position, switchBlock);
			}
		}


		//CRATES
		foreach (Crate crate in Level.Instance.crates) {
			DeclareVoxelIntention(crate, crate.position);
		}


		//FINISHED ANTS
		foreach (ProgramManager finishedProgram in programManagers_finished) {

			foreach (Ant ant in finishedProgram.GetAnts()) {

				DeclareAntIntention (ant, Command.WAIT);
			}

		}

		//ACTIVE ANTS
		foreach (ProgramManager progManager in programManagers_inProgress) {

			Command curCommand = ProgramManager.GetCommand (currentCommandDict [progManager]);
			foreach (Ant ant in progManager.GetAnts()) {

				bool validMove = DeclareAntIntention (ant, curCommand);


			}
		}

	}



	//returns true if can execute as intended
	private bool DeclareVoxelIntention (Voxel vox, Vector3 intendedPos)
	{
//		Debug.Log ("Voxel Intention " + vox + " intended Pos " + intendedPos );
		bool valid = positionPossibleNextStep(intendedPos);
		
		if (!valid) {
			intendedPos = vox.position;
			vox.HoldForStep ();
		}
		

		if (intendedNextPositions.ContainsKey (intendedPos)) {
			ResolveConflict (vox, intendedNextPositions [intendedPos], intendedPos);
			valid = false;
		} else {
			intendedNextPositions.Add (intendedPos, vox);
		}

		return valid;
	}

	private bool DeclareAntIntention (Ant ant, Command command)
	{
		Vector3 intendedPos = ant.positionAfterCommand (command);
		bool valid = positionPossibleNextStep(intendedPos);
		Debug.Log (ant + " intends to " + command + "\nintendedPos is " + intendedPos + " valid " + valid);

		if (!valid) {
			intendedPos = ant.position;
			ant.HoldForStep ();
		}
		
		
		if (intendedNextPositions.ContainsKey (intendedPos)) {
			Voxel voxAhead = intendedNextPositions[intendedPos];
			//if this ant is pushing, and the voxel ahead is pushable, and it currently intends to stay put
			if(command == Command.PUSH && voxAhead.isPushable && voxAhead.position == intendedPos){
				intendedNextPositions[intendedPos] = ant;
				DeclareVoxelIntention(voxAhead, voxAhead.position + ant.forwardDirection);

			}
			else{
			ResolveConflict (ant, voxAhead, intendedPos);
				valid = false;

			}
		} else {
			intendedNextPositions.Add (intendedPos, ant);
		}
		
		return valid;
		
		
	}

	private void ResolveConflict (Voxel voxA, Voxel voxB, Vector3 disputedPos)
	{
		//voxA and voxB both want the same position in one step
		intendedNextPositions.Remove (disputedPos);
		voxA.HoldForStep ();
		voxB.HoldForStep ();



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
