using UnityEngine;
using System.Collections;

public class DefaultBlock : Voxel {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator Move(Vector3 direction){
		float stepTime = ExecutionManager.STEP_TIME;
		float timer = 0;
		Vector3 startPos = position;
		Vector3 endPos = position + direction;
		Level.Instance.SetVoxel(this, endPos);
		
		while(timer < stepTime){
			timer += Time.deltaTime;
			this.transform.position = Vector3.Lerp(startPos, endPos, timer/stepTime);
			yield return null;
		}
		
		this.transform.position = endPos;
		SnapToGrid();
	}
}
