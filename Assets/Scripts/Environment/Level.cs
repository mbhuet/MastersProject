using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Level : MonoBehaviour {
	public static Level Instance;
	Dictionary<Vector3, Voxel> grid;
	public List<DynamicVoxel> dynamicVoxels;
	public List<Crate> crates;
	public List<Switch> switches;
	public List<SwitchBlock> switchBlocks;
	public List<DynamicVoxel> tempsToDestroy;



	void Awake(){
		Instance = this;
		dynamicVoxels = new List<DynamicVoxel>();
		grid = new Dictionary<Vector3, Voxel>();
		crates = new List<Crate> ();
		switches = new List<Switch> ();
		switchBlocks = new List<SwitchBlock> ();
	}

	void Start(){
	}

	public void AddDynamicVoxel(DynamicVoxel vox){
		dynamicVoxels.Add (vox);
	}

	public void RemoveDynamicVoxel(DynamicVoxel vox){
		if(dynamicVoxels.Contains(vox))
			dynamicVoxels.Add (vox);
	}


	public void AddCrate(Crate crate){
		crates.Add (crate);
	}

	public void RemoveCrate(Crate crate){
		if(crates.Contains(crate))
		crates.Remove (crate);
	}

	public void AddSwitch(Switch newSwitch){
		switches.Add (newSwitch);
	}

	public void AddSwtichBlock(SwitchBlock block){
		switchBlocks.Add (block);
	}

	public void ResetLevel(){
		foreach(DynamicVoxel vox in dynamicVoxels){
			vox.Reset();
		}
		foreach(Switch floorSwitch in switches){
			floorSwitch.Reset();
		}
		DestroyTemps ();
	}

	public void MarkTempForDestruction(DynamicVoxel tempVox){
		tempsToDestroy.Add (tempVox);
	}

	void DestroyTemps(){
		foreach (DynamicVoxel vox in tempsToDestroy) {
			dynamicVoxels.Remove(vox);
			GameObject.Destroy(vox.gameObject);
		}
		tempsToDestroy.Clear ();
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
		grid.TryGetValue (new Vector3 (col, height, row), out vox);
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
	public bool positionInBounds(Voxel vox, Vector3 pos){
		//if there is a static voxel in this space, it is not in bounds
		Voxel voxAtPos = GetVoxel(pos);
		if(voxAtPos != null && voxAtPos.isStatic){
			if(vox.GetComponent<Ant>() != null && voxAtPos.collectable){
				//this is fine
			}
			else{
				return false;
			}
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
		return true;//false;
	}

	public Dictionary<Vector3, Voxel> GetGrid(){
		return grid;
	}

}
