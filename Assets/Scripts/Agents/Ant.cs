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
	static float stepTime = 1; //how many seconds should it take to execute one command
	protected Vector3 startPosition;
	protected Vector3 startDirection;
	protected Vector3 forwardDirection;

	protected static Dictionary<Command, string> commandDict;

	protected List<Command> availableCommands;

	void Awake(){
		Debug.Log("Ant awake at " + Time.time);

	}
	// Use this for initialization
	protected override void Start () {
		base.Start ();
		SnapDirection ();
		startDirection = (transform.forward);
		commandDict = new Dictionary<Command, string>{
			{Command.FORWARD, "MoveForward"},
			{Command.TURN_R, "TurnRight"},
			{Command.TURN_L, "TurnLeft"}
		};
		Debug.Log("Ant start at " + Time.time);
		GameManager.Instance.RegisterAnt(this);
	}
	
	// Update is called once per frame
	void Update () {
	
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

	public void ExecuteCommand(Command com, float stepTime){
		Ant.stepTime = stepTime;
			SnapToGrid ();
			SnapDirection ();

			if (commandIsPossible(com)){
				StartCoroutine(commandDict[com]);
			}
			else{
				//TODO run error co-routine
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
		bool isPossible = false;
		//TODO CHECK Server for conflicts
		switch (com) {
		case Command.FORWARD:
			Vector3 forFloorPos = forwardDirection + new Vector3(col, height - 1, row);
			if (Level.Instance.GetVoxel(forFloorPos) != null){
				Vector3 nextPos = forwardDirection + new Vector3(col, height, row);
				if(Level.Instance.GetVoxel(nextPos) == null){
					Debug.Log(nextPos);
					isPossible = true;
				}
			}
			break;
		case Command.BACKWARD:
			Vector3 backFloorPos = forwardDirection + new Vector3(col, height - 1, row);
			if (Level.Instance.GetVoxel(backFloorPos) != null){
				Vector3 nextPos = forwardDirection + new Vector3(col, height, row);
				if(Level.Instance.GetVoxel(nextPos) == null){
					isPossible = true;
				}
			}
			break;
		case Command.TURN_L:
			isPossible = true;
			break;
		case Command.TURN_R:
			isPossible = true;
			break;
		case Command.WAIT:
			isPossible = true;
			break;
		default:
			break;

		}
		return isPossible;
	}

}

public class AntPosition{
	public Vector3 position;
	public Vector3 direction; //(each is -1, 0 or 1, designating movement in that direction with MoveForward)
}


