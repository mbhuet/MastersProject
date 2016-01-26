using UnityEngine;
using System.Collections;

public class CarpenterAnt : Ant {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		type = AntType.CARPENTER;
		//StartCoroutine (Execute ());
	}
	
	// Update is called once per frame
	void Update () {

	}
}
