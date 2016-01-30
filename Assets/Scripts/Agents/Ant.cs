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
	public AntType type;
	public Vector3 forwardDirection;

	protected static Dictionary<Command, string> commandDict;

	protected List<Command> availableCommands;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		SnapDirection ();
		commandDict = new Dictionary<Command, string>{
			{Command.FORWARD, "MoveForward"},
			{Command.TURN_R, "TurnRight"},
			{Command.TURN_L, "TurnLeft"},
			{Command.WAIT, "Wait"},
			{Command.BACKWARD, "MoveBackward"}
		};
		Debug.Log("Ant start at " + Time.time);
		GameManager.Instance.RegisterAnt(this);
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

	public void ExecuteCommand(Command com){
			SnapToGrid ();
			SnapDirection ();

				StartCoroutine(commandDict[com]);
	}

	protected IEnumerator MoveForward(){
		float stepTime = ExecutionManager.STEP_TIME;

		float timer = 0;
		Vector3 startPos = this.transform.position;
		Vector3 endPos = this.transform.position + this.transform.forward;
		Level.Instance.SetVoxel(this, endPos);

		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.position = Vector3.Lerp(startPos, endPos, timer/stepTime);
			yield return null;
		}

		this.transform.position = endPos;
		SnapToGrid();

	}
	protected IEnumerator MoveBackward(){
		float stepTime = ExecutionManager.STEP_TIME;

		float timer = 0;
		Vector3 startPos = this.transform.position;
		Vector3 endPos = this.transform.position - this.transform.forward;
		Level.Instance.SetVoxel(this, endPos);

		
		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.position = Vector3.Lerp(startPos, endPos, timer/stepTime);
			yield return null;
		}
		
		this.transform.position = endPos;
		SnapToGrid();
	}


	protected IEnumerator TurnRight(){
		float stepTime = ExecutionManager.STEP_TIME;

		float timer = 0;
		Quaternion startRot = this.transform.rotation;
		Quaternion endRot = this.transform.rotation * Quaternion.Euler(0,90,0);
		
		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.rotation = Quaternion.Lerp(startRot, endRot, timer/stepTime);
			yield return null;
		}
		this.transform.rotation = endRot;
		SnapDirection();
	}
	protected IEnumerator TurnLeft(){
		float stepTime = ExecutionManager.STEP_TIME;

		float timer = 0;
		Quaternion startRot = this.transform.rotation;
		Quaternion endRot = this.transform.rotation * Quaternion.Euler(0,-90,0);
		
		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.rotation = Quaternion.Lerp(startRot, endRot, timer/stepTime);
			yield return null;
		}
		this.transform.rotation = endRot;
		SnapDirection();
	}

	protected IEnumerator Wait(){
		float stepTime = ExecutionManager.STEP_TIME;

		yield return new WaitForSeconds (stepTime);
	}

	protected IEnumerator Push(){
		float stepTime = ExecutionManager.STEP_TIME;

		yield return new WaitForSeconds (stepTime);

	}

	protected IEnumerator Fire(){
		float stepTime = ExecutionManager.STEP_TIME;

		yield return new WaitForSeconds (stepTime);

	}

	protected IEnumerator Build(){
		float stepTime = ExecutionManager.STEP_TIME;

		yield return new WaitForSeconds (stepTime);

	}


	public Vector3 positionAfterCommand(Command com){
		switch (com) {
		case Command.FORWARD:
			return position + forwardDirection;
			break;
		case Command.BACKWARD:
			return position - forwardDirection;
			break;
		case Command.TURN_L:
			break;
		case Command.TURN_R:
			break;
		case Command.WAIT:
			break;
		case Command.PUSH:
			if(Level.Instance.GetVoxel(position + forwardDirection).isPushable)
				return position + forwardDirection;
			else
				return position;
		default:
			break;
			
		}

		return position;
		
	}

}


