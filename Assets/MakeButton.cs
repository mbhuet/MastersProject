using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MakeButton : MonoBehaviour {
	public GameObject buttonPrefab;
	// Use this for initialization
	void Start () {
		GameObject buttonObj = GameObject.Instantiate (buttonPrefab);
		buttonObj.transform.SetParent (this.transform);
		Button butt = buttonObj.GetComponent<Button> ();
		butt.onClick.AddListener (Hello);
	}
	
	void Hello(){
		Debug.Log ("HELLo");
	}
}
