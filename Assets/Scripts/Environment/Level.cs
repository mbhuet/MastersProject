using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Level : MonoBehaviour {
	public static Level Instance;
	Dictionary<Vector3, Voxel> grid;


	void Awake(){
		Instance = this;
		grid = new Dictionary<Vector3, Voxel>();
	}


	public void SetVoxel (Voxel vox, Vector3 pos){
		if (grid.ContainsKey (pos)) {
			Debug.Log ("voxel " + vox + " cannot occupy space " + pos + " because it is already occupied");
		}
		else{
			grid.Add (pos, vox);
		}
	}

	public Voxel GetVoxel(int col, int row, int height){
		Voxel vox;
		grid.TryGetValue (new Vector3 (col, row, height), out vox);
		return vox;
	}
	public Voxel GetVoxel (Vector3 pos){
		Voxel vox;
		grid.TryGetValue (pos, out vox);
		return vox;
	}

}
