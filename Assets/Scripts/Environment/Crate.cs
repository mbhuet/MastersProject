using UnityEngine;
using System.Collections;

public class Crate : Voxel {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		Level.Instance.AddCrate (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
