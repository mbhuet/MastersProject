using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
	GridLayoutGroup gridLayoutGroup;
	LobbyPanel[] panels; 
	// Use this for initialization
	void Start () {
		gridLayoutGroup = GetComponent<GridLayoutGroup> ();
		gridLayoutGroup.cellSize = new Vector2 (Screen.width / 2, Screen.height / 2);
		panels = transform.GetComponentsInChildren<LobbyPanel> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
