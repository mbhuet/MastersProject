using UnityEngine;
using System.Collections;

public abstract class Voxel : MonoBehaviour {
	protected Vector3 startPosition;
	protected Quaternion startRotation;

	public Vector3 position;
	public Vector3 forwardDirection;

	public bool isStatic;
	public bool canStackOn;
	public bool isBurnable;
	public bool isPushable;

	public bool isActive = true;
	public bool temporary = false;

	protected bool initialized = false;


	protected Vector3 intendedPosition;
	protected int intentionPriority = -1;
	protected Voxel intendedActor; //if another voxel is compelling this voxel to move, it is recorded here
	
	public delegate void IntentionDelegate();
	public IntentionDelegate NextAction;

	MeshRenderer[] renderers;


	// Use this for initialization
	protected virtual void Start () {
		if(!initialized)
		VoxelInit ();
	}

	public virtual void VoxelInit(){
		SnapToGrid ();
		startPosition = position;
		SnapDirection ();
		startRotation = transform.rotation;
		renderers = gameObject.GetComponentsInChildren<MeshRenderer> ();
		initialized = true;
	}




	protected virtual void SnapToGrid(){
		Level.Instance.RemoveVoxel(this, position);
		int col = Mathf.Max((int)(transform.position.x + .5f), 0);
		int row = Mathf.Max((int)(transform.position.z + .5f), 0);
		int height = Mathf.Max((int)transform.position.y, 0);

		position = new Vector3(col,height,row);
		transform.position = position;
		Level.Instance.SetVoxel(this, position);
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

	public void Burn(){
		SetVisible (false);
		isActive = false;
		Level.Instance.RemoveVoxel (this, this.position);
	}
	
	public void Assemble(){
		Debug.Log ("Assemble");
		this.transform.position = intendedPosition;
		SnapToGrid ();
		SetVisible (true);
		isActive = true;
	}



	public void SetVisible(bool isVisible){
		foreach (MeshRenderer mesh in renderers) {
			mesh.enabled = isVisible;
		}
	}

	public void SetIntention(Vector3 intendedPos, int comPriority, IntentionDelegate intendedFunc, Ant actor){
		SetIntention (intendedPos, comPriority, intendedFunc);
		intendedActor = actor;
	}
	
	public void SetIntention(Vector3 intendedPos, int comPriority, IntentionDelegate intendedFunc){
		intendedPosition = intendedPos;
		intentionPriority = comPriority;
		NextAction = intendedFunc;
	}
	
	public void ClearIntention(){
		intendedPosition = Vector3.one * -1;
		intentionPriority = -1;
		intendedActor = null;
		NextAction = null;
	}
	
	public Vector3 GetIntendedPosition(){
		return intendedPosition;
	}

	public int GetIntentionPriority(){
		return intentionPriority;
	}

	public IntentionDelegate GetIntendedFunc(){
		return NextAction;
	}


	public virtual void Reset(){
		StopAllCoroutines ();
		SetVisible(true);
		isActive = true;

		if (temporary) {
		}
	}


}
