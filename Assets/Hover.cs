using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {
	public bool isBobbing;
	public float speed;
	public float heightDelta;

	// Use this for initialization
	void Start () {
		StartCoroutine (BobRoutine());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator BobRoutine(){
		Vector3 startPos = transform.position;
		while(isBobbing){
			this.transform.position = new Vector3(startPos.x,  
			                                      startPos.y + Mathf.Sin(Time.time * speed) * heightDelta,
			                                      startPos.z);
			yield return null;
		}
	}
}
