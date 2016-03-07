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
			ClearIntention ();
		}
	}

	//an internal intention can be overidden by external of higher priority
	//an external intention that moves will not override an internal intention that is stationary
	//two move actions cannot both occur
	//two still intentions cannot both occur
	//if heldforstep, it cannot change moveAction;
	public void SetIntention(Voxel motivator, Vector3 intendedPos, int comPriority, IntentionDelegate intendedFunc, bool involvesMovement){
//		Debug.Log ("SetIntention:: held: " + heldInPlace + " self motivated: " + (motivator == this) + " involvesMovement: " + involvesMovement);
		if (heldInPlace && motivator == this && involvesMovement) {
//			Debug.Log("Voxel " + this + " is held in place and cannot move itself this step");
			return;
		}
			

		if (involvesMovement && comPriority < intentionPriority) {
//			Debug.Log("Setting move action for " + this);
			intentionPriority = comPriority;
			intendedPosition = intendedPos;
			MoveAction = intendedFunc;
		} else if (!involvesMovement){
//			Debug.Log("Setting still action for " + this);
			StillAction = intendedFunc;
		}
	}
	
	public void ClearIntention(){
		intendedPosition = position;
		intentionPriority = 99;
		intendedActor = null;
		MoveAction = null;
		StillAction = null;
		heldInPlace = false;
	}
	
	public override Vector3 GetIntendedPosition(){
		return intendedPosition;
	}
	
	public int GetIntentionPriority(){
		return intentionPriority;
	}
	
	public IntentionDelegate GetIntendedMoveFunc(){
		return MoveAction;
	}


	public IEnumerator Move (Vector3 direction)
	{
		ExecutionManager.Instance.AddMovingVoxel (this);
//		Debug.Log ("Voxel " + this + " is moving " + direction);
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

	public void Teleport(Vector3 pos){
		transform.position = pos;
		SnapToGrid ();
		SnapDirection ();
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

	public void TurnRight(){
		StartCoroutine ("Turn", Vector3.up * 90);
	}
	public void TurnLeft(){
		StartCoroutine ("Turn", Vector3.up * -90);
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
