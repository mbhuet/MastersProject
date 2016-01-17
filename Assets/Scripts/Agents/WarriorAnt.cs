using UnityEngine;
using System.Collections;

public class WarriorAnt : Ant {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		type = AntType.WARRIOR;

		//StartCoroutine (Execute ());
	}
	
	// Update is called once per frame
	void Update () {

	}
}
