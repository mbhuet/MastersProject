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

public class Ant : DynamicVoxel {
	public AntType type;


	public ParticleSystem fireFX;
	public Crate cratePrefab;
	Crate buildCrate;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		GameManager.Instance.RegisterAnt(this);
	}



	public void ExecuteCommand(Command com){
			SnapToGrid ();
			SnapDirection ();
			//StartCoroutine(commandDict[com]);
	}

	public void MoveForward(){
		StartCoroutine ("Move", forwardDirection);
	}

	public void MoveBackward(){
		StartCoroutine ("Move", -forwardDirection);
	}



	public void Wait(){
		
	}

	public void Push(){
		StartCoroutine ("Move", forwardDirection);
	}

	public void Fire(){
		fireFX.Emit (5);
	}

	public void Build(){
		buildCrate.Teleport(position + forwardDirection);
		buildCrate = null;

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
}


