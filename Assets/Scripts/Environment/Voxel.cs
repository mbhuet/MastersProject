using UnityEngine;
using System.Collections;

public abstract class Voxel : MonoBehaviour {
	public int row;
	public int col;
	public int height;

	// Use this for initialization
	protected void Start () {
		SnapToGrid ();
		VoxelInit ();
	}

	void VoxelInit(){
		Level.Instance.SetVoxel (this, col, height, row);
	}

	virtual protected void SnapToGrid(){
		col = Mathf.Max((int)(transform.position.x + .5f), 0);
		row = Mathf.Max((int)(transform.position.z + .5f), 0);
		height = Mathf.Max((int)transform.position.y, 0);
		transform.position = new Vector3(col,height,row);
	}


}
