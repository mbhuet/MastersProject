using UnityEngine;
using System.Collections;

public abstract class Voxel : MonoBehaviour {
	public Vector3 position;
	Vector3 startPosition;

	public bool isStatic;
	public bool canStackOn;
	public bool isPushable;
	public bool isBurnable;

	public bool heldInPlace = false;

	// Use this for initialization
	protected virtual void Start () {
		VoxelInit ();
	}

	void VoxelInit(){
		SnapToGrid ();
		startPosition = position;
	}

	virtual protected void SnapToGrid(){
		Level.Instance.RemoveVoxel(this, position);
		int col = Mathf.Max((int)(transform.position.x + .5f), 0);
		int row = Mathf.Max((int)(transform.position.z + .5f), 0);
		int height = Mathf.Max((int)transform.position.y, 0);

		position = new Vector3(col,height,row);
		transform.position = position;
		Level.Instance.SetVoxel(this, position);
	}

	public void HoldForStep(){
		heldInPlace = true;

	}

	public void ReturnToStartPosition(){
		transform.position = startPosition;
		SnapToGrid();
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
