using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TileBank : MonoBehaviour{
	public CommandTile tilePrefab;

	void Start(){
		if (tilePrefab != null){
			//Init (tilePrefab);
		}
	}

	public void Init(CommandTile tilePrefab, int arg){
		this.tilePrefab = tilePrefab;
		CommandTile tile = GameObject.Instantiate(tilePrefab);
		tile.argument = arg;
		tile.transform.SetParent (this.transform);
	}

}
