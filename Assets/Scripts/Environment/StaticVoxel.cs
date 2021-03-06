﻿using UnityEngine;
using System.Collections;

public abstract class StaticVoxel : Voxel {

	// Use this for initialization
	protected virtual void Start () {
		base.Start ();
		isStatic = true;
		isPushable = false;
		intendedPosition = position;
	}

	public override Vector3 GetIntendedPosition ()
	{
		return position;
	}
	

}
