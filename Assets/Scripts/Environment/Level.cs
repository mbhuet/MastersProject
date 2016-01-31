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
		pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
		if (grid.ContainsKey (pos)) {
			Debug.Log ("voxel " + vox + " overwriting space " + pos + ", previously occupied by " + grid[pos]);
			grid[pos] = vox;
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
	public void RemoveVoxel(Voxel vox, Vector3 pos){
		if(grid.ContainsKey(pos) && grid[pos] == vox){
		grid.Remove(pos);
		}
	}


	//will check if there are any static floor voxels beneath this position that could make it possible
	public bool positionInBounds(Vector3 pos){
		//if there is a static voxel in this space, it is not in bounds
		Voxel vox = GetVoxel(pos);
		if(vox != null && vox.isStatic){
			return false;
		}

		//if there is no ground below this space, it is not in bounds
		int height = (int)pos.y;
		while(height >= 0){
			height -= 1;
			Voxel ground = GetVoxel(new Vector3(pos.x, height, pos.z));
			if(ground != null && ground.isStatic){
				return true;
			}
		}
		return false;
	}

	public Dictionary<Vector3, Voxel> GetGrid(){
		return grid;
	}

}
