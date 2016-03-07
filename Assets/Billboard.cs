using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Update () {
		this.transform.rotation = Camera.main.transform.rotation;
	}
	

}
