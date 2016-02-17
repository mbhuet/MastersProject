using UnityEngine;
using System.Collections;

public class SwitchBlock : DynamicVoxel
{
	public Vector3 moveDirection;
	public bool willMove = false;
	bool isMoved = false;

	// Use this for initialization
	void Start ()
	{
		base.Start ();
		Level.Instance.AddSwtichBlock (this);
	}

	public void Trigger (bool switchDown)
	{
		Debug.Log ("SwitchBlock trigger " + switchDown);
		willMove = true;
	}

	public void ChangePosition(){
		Voxel voxAbove = Level.Instance.GetVoxel (position + Vector3.up);
		if (isMoved) {
			if(voxAbove != null) voxAbove.StartCoroutine("Move", -moveDirection);
			StartCoroutine ("Move", -moveDirection);
			isMoved = false;
		} else {
			if(voxAbove != null) voxAbove.StartCoroutine("Move", moveDirection);
			StartCoroutine("Move", moveDirection);
			isMoved = true;
		}
		willMove = false;
	}

	public Vector3 NextPosition ()
	{
		if (!willMove) {
			return position;
		} else if (isMoved) {
			return startPosition;
		} else {
			return startPosition + moveDirection;
		}

	}

	public override void Reset ()
	{
		base.Reset ();
		isMoved = false;
		willMove = false;

	}
}
