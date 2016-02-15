using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Level : MonoBehaviour {
	public static Level Instance;
	Dictionary<Vector3, Voxel> grid;
	public List<Voxel> nonStaticVoxels;
	public List<Crate> crates;
	public List<Switch> switches;
	public List<SwitchBlock> switchBlocks;



	void Awake(){
		Instance = this;
		nonStaticVoxels = new List<Voxel>();
		grid = new Dictionary<Vector3, Voxel>();
		crates = new List<Crate> ();
		switches = new List<Switch> ();
		switchBlocks = new List<SwitchBlock> ();
	}

	void Start(){
	}

	public void AddNonStaticVoxel(Voxel vox){
		nonStaticVoxels.Add (vox);
	}

	public void AddCrate(Crate crate){
		crates.Add (crate);
	}

	public void AddSwitch(Switch newSwitch){
		switches.Add (newSwitch);
	}

	public void AddSwtichBlock(SwitchBlock block){
		switchBlocks.Add (block);
	}

	public void ResetLevel(){
		foreach(Voxel vox in nonStaticVoxels){
			vox.Reset();
		}
	}




	public void SetVoxel (Voxel vox, Vector3 pos){
		pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
		if (grid.ContainsKey (pos)) {
//			Debug.Log ("voxel " + vox + " overwriting space " + pos + ", previously occupied by " + grid[pos]);
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
			if(ground != null && ground.canStackOn){
				return true;
			}
		}
		return false;
	}

	public Dictionary<Vector3, Voxel> GetGrid(){
		return grid;
	}

}
