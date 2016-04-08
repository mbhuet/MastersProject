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
	SCOUT,
	DEFAULT,
	ALL
}

public class Ant : DynamicVoxel {
	public int ownerPlayerNum;
	public AntType type;
	public ParticleSystem fireFX;
	public ParticleSystem dustFX;
	public Crate cratePrefab;
	public AntPopup popupPrefab;

	Crate buildCrate;
	Animator anim;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		if (GameManager.Instance != null) {
			GameManager.Instance.RegisterAnt (this);
		}
		anim = transform.FindChild ("ant").GetComponent<Animator> ();
		//SpawnPopup ("YOU");
	}

	public void MoveForward(){
		StartCoroutine ("Move", forwardDirection);
		anim.SetTrigger("walk");
	}

	public void MoveBackward(){
		StartCoroutine ("Move", -forwardDirection);
		anim.SetTrigger("walkBack");
	}

	public void Wait(){
		anim.SetTrigger ("idle");
	}

	public void Push(){
		//dustFX.duration = 1;//ExecutionManager.STEP_TIME;
		dustFX.Play ();
		StartCoroutine ("Move", forwardDirection);
		anim.SetTrigger ("push");
	}

	public void Fire(){
		fireFX.Emit (5);
		anim.SetTrigger ("fire");
	}

	public void Build(){
		buildCrate.Teleport(position + forwardDirection);
		buildCrate = null;
		anim.SetTrigger ("build");
	}

	public Crate GetBuildCrate(){
		if (buildCrate == null)
			MakeBuildCrate ();
		return buildCrate;
	}

	void MakeBuildCrate(){
		buildCrate = (Crate)GameObject.Instantiate (cratePrefab, Vector3.one * -1, Quaternion.identity);
		buildCrate.VoxelInit ();
		buildCrate.temporary = true;
		buildCrate.isActive = false;
		buildCrate.SetVisible (false);
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
			return position + forwardDirection;
		default:
			break;
			
		}

		return position;
		
	}

	public void SpawnPopup(string message){
		AntPopup popup = GameObject.Instantiate (popupPrefab, transform.position, Quaternion.identity) as AntPopup;
		popup.transform.SetParent (this.transform);
		popup.Init (message, true);
	}

	protected void SpawnPopup(string message, float duration){
		AntPopup popup = GameObject.Instantiate (popupPrefab, transform.position, Quaternion.identity) as AntPopup;
		popup.transform.SetParent (this.transform);
		popup.Init (message, true);
		popup.StartTimer (duration);
	}
}


