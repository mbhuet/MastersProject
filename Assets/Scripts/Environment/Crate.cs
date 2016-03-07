using UnityEngine;
using System.Collections;

public class Crate : DynamicVoxel {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	public override void VoxelInit(){
		if (!initialized) {
			base.VoxelInit ();
			Level.Instance.AddCrate (this);
		}
	}


	public override void Reset(){
		if (temporary)
			Level.Instance.RemoveCrate (this);
		base.Reset ();

	}


}
