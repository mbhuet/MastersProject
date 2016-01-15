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

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetVoxel (Voxel vox, int col, int row, int height){
		Vector3 pos = new Vector3 (col,row,height);
		if (grid.ContainsKey (pos)) {
			Debug.Log ("voxel " + vox + " cannot occupy space " + pos + " because it is already occupied");
		}
		else{
			grid.Add (new Vector3 (col, row, height), vox);
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

	void generateLevel(int rows, int cols){
		for (int r= 0; r < rows; r++) {
			for (int c = 0; c < cols; c++){

			}
		}
	}

	void loadLevel(string filename){
	}
}
