using UnityEngine;
using System.Collections;

public abstract class Voxel : MonoBehaviour {
	public Vector3 position;

	public bool isStatic;
	public bool canStackOn;
	public bool isPushable;
	public bool isBurnable;

	public bool heldInPlace = false;

	// Use this for initialization
	protected virtual void Start () {
		VoxelInit ();
	}

	void VoxelInit(){
		SnapToGrid ();
	}

	virtual protected void SnapToGrid(){
		Level.Instance.RemoveVoxel(this, position);
		int col = Mathf.Max((int)(transform.position.x + .5f), 0);
		int row = Mathf.Max((int)(transform.position.z + .5f), 0);
		int height = Mathf.Max((int)transform.position.y, 0);

		position = new Vector3(col,height,row);
		transform.position = position;
		Level.Instance.SetVoxel(this, position);
	}

	public void HoldForStep(){
		heldInPlace = true;

	}

}
