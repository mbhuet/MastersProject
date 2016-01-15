using UnityEngine;
using System.Collections;

public class FireAnt : Ant {

	// Use this for initialization
	void Start () {
		base.Start ();
		StartCoroutine (Execute ());
	}
	
	// Update is called once per frame
	void Update () {

	}
}
