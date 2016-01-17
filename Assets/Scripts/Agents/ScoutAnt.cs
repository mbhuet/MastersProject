using UnityEngine;
using System.Collections;

public class ScoutAnt : Ant {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		type = AntType.SCOUT;

		//StartCoroutine (Execute ());
	}
	
	// Update is called once per frame
	void Update () {

	}
}
