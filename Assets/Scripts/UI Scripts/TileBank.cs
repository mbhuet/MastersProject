using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TileBank : MonoBehaviour{
	public CommandTile tilePrefab;

	void Start(){
		CommandTile tile = GameObject.Instantiate(tilePrefab);
		tile.transform.SetParent (this.transform);
	}
}
