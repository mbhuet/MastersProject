using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	//public Vector3 axis;
	public float speed;
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
		this.transform.RotateAround (transform.position, transform.forward, speed);
	}
}
