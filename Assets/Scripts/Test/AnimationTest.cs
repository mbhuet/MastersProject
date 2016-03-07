using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {
	Animation anim;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animation>();
		anim.Play("idle");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
