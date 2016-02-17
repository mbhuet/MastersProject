using UnityEngine;
using System.Collections;

public abstract class DynamicVoxel : Voxel
{
	public bool heldInPlace = false;

	protected override void Start () {
		base.Start ();
	}

	public override void VoxelInit(){
		if (!initialized) {
			base.VoxelInit ();
			Level.Instance.AddDynamicVoxel (this);
		}
	}


	public IEnumerator Move (Vector3 direction)
	{
		ExecutionManager.Instance.AddMovingVoxel (this);
		Debug.Log ("Voxel " + this + " is moving " + direction);
		float stepTime = ExecutionManager.STEP_TIME;
		float timer = 0;
		Vector3 startPos = position;
		Vector3 endPos = position + direction;
		Level.Instance.SetVoxel (this, endPos);
		
		while (timer < stepTime) {
			timer += Time.deltaTime;
			this.transform.position = Vector3.Lerp (startPos, endPos, timer / stepTime);
			yield return null;
		}
		
		this.transform.position = endPos;
		SnapToGrid ();
		ExecutionManager.Instance.RemoveMovingVoxel (this);
	}

	public IEnumerator Turn (Vector3 rotation)
	{
		float stepTime = ExecutionManager.STEP_TIME;
		ExecutionManager.Instance.AddMovingVoxel (this);
		
		float timer = 0;
		Quaternion startRot = this.transform.rotation;
		Quaternion endRot = this.transform.rotation * Quaternion.Euler (rotation.x, rotation.y, rotation.z);
		
		while (timer < stepTime) {
			timer += Time.deltaTime;
			this.transform.rotation = Quaternion.Lerp (startRot, endRot, timer / stepTime);
			yield return null;
		}
		ExecutionManager.Instance.RemoveMovingVoxel (this);
		this.transform.rotation = endRot;
		SnapDirection ();
	}

	public void HoldForStep ()
	{
		heldInPlace = true;
		
	}

	public void Stay ()
	{
	}

	public IntentionDelegate MoveDirection (Vector3 direction)
	{
		direction.Normalize ();
		if (direction.x > 0) {
			return MoveX_Pos;
		} else if (direction.x < 0) {
			return MoveX_Neg;
		} else if (direction.y > 0) {
			return MoveY_Pos;
		} else if (direction.y < 0) {
			return MoveY_Neg;
		} else if (direction.z > 0) {
			return MoveZ_Pos;
		} else if (direction.z < 0) {
			return MoveZ_Neg;
		}

		return Stay;
	}

	public void MoveX_Pos(){
		StartCoroutine ("Move", Vector3.right);
	}
	public void MoveX_Neg(){
		StartCoroutine ("Move", Vector3.left);
	}
	public void MoveY_Pos(){
		StartCoroutine ("Move", Vector3.up);
	}
	public void MoveY_Neg(){
		StartCoroutine ("Move", Vector3.down);
	}
	public void MoveZ_Pos(){
		StartCoroutine ("Move", Vector3.forward);
	}
	public void MoveZ_Neg(){
		StartCoroutine ("Move", Vector3.back);
	}


	public void ReturnToStartPosition ()
	{
		transform.position = startPosition;
		SnapToGrid ();
		transform.rotation = startRotation;
		SnapDirection ();
	}

	public virtual void Reset ()
	{
		ReturnToStartPosition ();
		if (temporary) {
			Level.Instance.MarkTempForDestruction(this);
		}
		base.Reset ();
	}


}
