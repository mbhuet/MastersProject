using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerPanelHooks : MonoBehaviour {

	public PlayerLobby player;

	public delegate void CanvasHook();
	public CanvasHook OnReadyHook;

	public Button readyButton;
	public Text titleText;
	//public Text messagText;
	
	public void UIReady()
	{
		if (OnReadyHook != null)
			OnReadyHook.Invoke();
	}

	public void SetPlayer(PlayerLobby play){
		player = play;
		titleText.text = "Player " + play.slot;
	}

	public void RemovePlayer(){
		player = null;
		titleText.text = "No Player";
	}

}
