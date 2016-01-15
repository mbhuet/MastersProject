using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour {
	Player player;
	Text panelText;
	// Use this for initialization
	void Start () {
		panelText = GetComponentInChildren<Text> ();
		SetText ("No Player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetText(string text){
		panelText.text = text;
	}

	public void SetPlayer(Player player){
		this.player = player;
	}
}
