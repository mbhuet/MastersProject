using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AntType{
	FIRE,
	CARPENTER,
	WARRIOR,
	SNIPER,
	MEDIC,
	QUEEN,
	SCOUT
}

public abstract class Ant : Voxel {
	static float stepTime = 1; //how many seconds should it take to execute one command
	protected Vector3 startPosition;
	protected Vector3 startDirection;
	protected Vector3 forwardDirection;

	protected static Dictionary<Command, string> commandDict;

	protected List<Command> availableCommands;
	public List<Command> func1;

	// Use this for initialization
	protected void Start () {
		base.Start ();
		SnapDirection ();
		startDirection = (transform.forward);
		commandDict = new Dictionary<Command, string>{
			{Command.FORWARD, "MoveForward"},
			{Command.TURN_R, "TurnRight"},
			{Command.TURN_L, "TurnLeft"}
		};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ExecuteStep(){
	}

	public void AddCommand(int i, Command com){
		func1.Insert(i,com);
	}

	public void RemoveCommand(int i){
		func1.RemoveAt (i);
	}

	protected void SnapDirection(){
		Vector3 forward = transform.forward;
		Vector3 abs = new Vector3 (Mathf.Abs (forward.x), Mathf.Abs (forward.y), Mathf.Abs (forward.z));
		int x = 0;
		int y = 0;
		int z = 0;
		if (abs.x >= abs.y && abs.x >= abs.z) {
			x = forward.x > 0 ? 1 : -1;
		}
		else if (abs.y >= abs.x && abs.y >= abs.z) {
			y = forward.y > 0 ? 1 : -1;
		}
		else if (abs.z >= abs.y && abs.z >= abs.x) {
			z = forward.z > 0 ? 1 : -1;
		}

		Vector3 new_forward = new Vector3 (x, y, z);
		transform.rotation = Quaternion.LookRotation (new_forward);
		forwardDirection = new_forward;
	}

	protected IEnumerator Execute(){

		foreach(Command com in func1){
			SnapToGrid ();
			SnapDirection ();

			if (commandIsPossible(com)){
				StartCoroutine(commandDict[com]);
			}
			else{
				//TODO run error co-routine
			}

			yield return new WaitForSeconds(stepTime);
		}
	}

	protected IEnumerator MoveForward(){
		float timer = 0;
			Vector3 startPos = this.transform.position;
		Vector3 endPos = this.transform.position + this.transform.forward;

		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.position = Vector3.Lerp(startPos, endPos, timer/stepTime);
			yield return null;
		}

		this.transform.position = endPos;
	}


	protected IEnumerator TurnRight(){
		float timer = 0;
		Quaternion startRot = this.transform.rotation;
		Quaternion endRot = this.transform.rotation * Quaternion.Euler(0,90,0);
		
		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.rotation = Quaternion.Lerp(startRot, endRot, timer/stepTime);
			yield return null;
		}
		this.transform.rotation = endRot;
	}
	protected IEnumerator TurnLeft(){
		float timer = 0;
		Quaternion startRot = this.transform.rotation;
		Quaternion endRot = this.transform.rotation * Quaternion.Euler(0,-90,0);
		
		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.rotation = Quaternion.Lerp(startRot, endRot, timer/stepTime);
			yield return null;
		}
		this.transform.rotation = endRot;
	}

	protected bool commandIsPossible(Command com){
		//TODO CHECK Server for conflicts
		switch (com) {
		case Command.FORWARD:
			Vector3 forFloorPos = forwardDirection + new Vector3(col, row, height - 1);
			if (Level.Instance.GetVoxel(forFloorPos) != null){
				Vector3 nextPos = forwardDirection + new Vector3(col, row, height);
				if(Level.Instance.GetVoxel(nextPos) == null){
					Debug.Log(nextPos);
					return true;
				}
			}
			break;
		case Command.BACKWARD:
			Vector3 backFloorPos = forwardDirection + new Vector3(col, row, height - 1);
			if (Level.Instance.GetVoxel(backFloorPos) != null){
				Vector3 nextPos = forwardDirection + new Vector3(col, row, height);
				if(Level.Instance.GetVoxel(nextPos) == null){
					return true;
				}
			}
			break;
		case Command.TURN_L:
			return true;
			break;
		case Command.TURN_R:
			return true;
			break;
		case Command.WAIT:
			return true;
			break;
		default:
			return true;
			break;

		}
		return false;
	}

}

public class AntPosition{
	public Vector3 position;
	public Vector3 direction; //(each is -1, 0 or 1, designating movement in that direction with MoveForward)
}


