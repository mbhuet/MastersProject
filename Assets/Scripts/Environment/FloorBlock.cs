using UnityEngine;
using System.Collections;

public class FloorBlock : Voxel {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override protected void SnapToGrid(){
		col = Mathf.Max((int)transform.position.x, 0);
		row = Mathf.Max((int)transform.position.z, 0);
		height = 0;
		transform.position = new Vector3(col,height,row);
	}


}