using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.transform.rotation = Camera.main.transform.rotation;
	}
	

}
