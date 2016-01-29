using UnityEngine;
using System.Collections;

public abstract class Voxel : MonoBehaviour {
	public Vector3 position;

	public bool isPushable;
	public bool isBurnable;

	// Use this for initialization
	protected virtual void Start () {
		SnapToGrid ();
		VoxelInit ();
	}

	void VoxelInit(){
		Level.Instance.SetVoxel (this, position);
	}

	virtual protected void SnapToGrid(){
		int col = Mathf.Max((int)(transform.position.x + .5f), 0);
		int row = Mathf.Max((int)(transform.position.z + .5f), 0);
		int height = Mathf.Max((int)transform.position.y, 0);

		position = new Vector3(col,height,row);
		transform.position = position;
	}


}
