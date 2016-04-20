using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ButtonTest : MonoBehaviour {

	void Start(){
		Button butt = gameObject.GetComponent<Button> ();
		butt.onClick.AddListener (Hello);
	}

	void Hello(){
		Debug.Log ("HELLo");
	}
}
