using UnityEngine;
using System.Collections;

public class Switch : StaticVoxel {
	public bool pressed = false;

	Vector3 buttonUpPos;
	Vector3 buttonDownPos;

	public GameObject button;
	public SwitchBlock switchBox;


	// Use this for initialization
	void Start () {
		base.Start ();
		buttonUpPos = new Vector3 (0, .25f, 0);
		buttonDownPos = new Vector3 (0, .1f, 0);
		Level.Instance.AddSwitch (this);
	}


	public override void Reset(){
		base.Reset ();
		pressed = false;
		button.transform.localPosition = buttonUpPos;
	}

	public void CheckPressed(){
		Voxel voxAbove = Level.Instance.GetVoxel (position + Vector3.up);
		if (!pressed && voxAbove != null) {
			pressed = true;
			switchBox.Trigger(true);
			StartCoroutine("ButtonDown");

		} else if (pressed && voxAbove == null) {
			pressed = false;
			switchBox.Trigger(false);
			StartCoroutine("ButtonUp");
		}
	}

	IEnumerator ButtonUp(){
		button.transform.localPosition = buttonUpPos;
		yield return null;
	}

	IEnumerator ButtonDown(){
		button.transform.localPosition = buttonDownPos;
		yield return null;
	}

}
