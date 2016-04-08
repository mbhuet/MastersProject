using UnityEngine;
using System.Collections;

public class AntPopup : MonoBehaviour {
	bool bob;
	string message; 

	// Use this for initialization
	void Start () {
	}

	public void Init(string message, bool bob){
		this.bob = bob;
		this.message = message;
		StartCoroutine (Ascend(1, 10));
	}

	public void StartTimer(float time){
		StartCoroutine (Countdown(time));
	}
	
	// Update is called once per frame

	IEnumerator Countdown(float duration){
		float timePassed = 0;
		while (timePassed < duration) {
			timePassed += Time.deltaTime;
			yield return null;
		}
		GameObject.Destroy (this.gameObject);
	}

	IEnumerator Bob(float deltaHeight, float speed){
		Vector3 startPos = transform.localPosition;//+ Vector3.up * deltaHeight / 2;
		float time = 0;


		while (bob) {
			time += Time.deltaTime * speed;
			transform.localPosition = startPos + Mathf.Sin(time) * Vector3.up * deltaHeight;
			yield return null;

		}

	}

	IEnumerator Ascend(float height, float speed){
		Vector3 targetPos = this.transform.localPosition + Vector3.up * height;
		while (Vector3.Distance(transform.localPosition, targetPos) > .01f) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, speed * Time.deltaTime);
			yield return null;
		}

		transform.localPosition = targetPos;

		if (bob) {
			StartCoroutine(Bob (.05f, 1));
		}
	}


}
