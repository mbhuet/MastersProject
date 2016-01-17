using UnityEngine;
using System.Collections;

public class FireAnt : Ant {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		type = AntType.FIRE;

		//StartCoroutine (Execute ());
	}
	
	// Update is called once per frame
	void Update () {

	}
}
