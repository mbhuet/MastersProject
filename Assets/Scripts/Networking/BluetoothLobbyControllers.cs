using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using LostPolygon.AndroidBluetoothMultiplayer;

[Serializable]
public class BluetoothConnectCanvasControl : CanvasControl
{
	public override void Show ()
	{
		base.Show ();
		
		var hooks = canvas.GetComponent<BluetoothConnectHooks> ();
		if (hooks == null)
			return;

		hooks.OnFind = OnGUIFindGame;
		hooks.OnHost = OnGUIStartHost;
	}

	void OnGUIStartHost ()
	{
#if UNITY_ANDROID
		Debug.Log ("OnGUIStartHost");
		BluetoothLobbyManager.Instance.StartBluetoothHost ();
		BluetoothLobbyManager.Instance.onlineCanvas.Show("Host");
		BluetoothLobbyManager.Instance.offlineCanvas.Hide ();
#endif
#if UNITY_EDITOR
		BluetoothLobbyManager.Instance.StartHost();
		BluetoothLobbyManager.Instance.onlineCanvas.Show("Host");
#endif
	}

	void OnGUIFindGame ()
	{
#if UNITY_ANDROID
		Debug.Log ("OnGUIFindGame");
		BluetoothLobbyManager.Instance.ConnectToBluetoothServer ();
		BluetoothLobbyManager.Instance.onlineCanvas.Show("Client");
		BluetoothLobbyManager.Instance.offlineCanvas.Hide ();
#endif
#if UNITY_EDITOR
		BluetoothLobbyManager.Instance.StartClient();
		BluetoothLobbyManager.Instance.onlineCanvas.Show("Client");
#endif
	}

}

[Serializable]
public class BluetoothLobbyCanvasControl : CanvasControl
{
	public override void Show ()
	{
		base.Show ();
		
		var hooks = canvas.GetComponent<BluetoothLobbyCanvasHooks> ();
		if (hooks == null)
			return;
	}


}

[Serializable]
public class BluetoothOnlineCanvasControl : CanvasControl
{
	public void Show (string status)
	{
		base.Show ();
		
		BluetoothLobbyManager.Instance.offlineCanvas.Hide ();
		
		var hooks = canvas.GetComponent<OnlineControlHooks> ();
		if (hooks == null)
			return;
		
		hooks.OnStopHook = OnGUIStop;
		
		hooks.SetAddress (BluetoothLobbyManager.Instance.networkAddress);
		hooks.SetStatus (status);
		
		BluetoothLobbyManager.Instance.onlineStatus = status;
		
		EventSystem.current.firstSelectedGameObject = hooks.firstButton.gameObject;
		EventSystem.current.SetSelectedGameObject (hooks.firstButton.gameObject);
	}
	
	public void OnGUIStop ()
	{
		BluetoothLobbyManager.Instance.popupCanvas.Hide ();
		BluetoothLobbyManager.Instance.StopHost ();
	}
}

[Serializable]
public class BluetoothExitToLobbyCanvasControl : CanvasControl
{
	public override void Show ()
	{
		base.Show ();
		
		var hooks = canvas.GetComponent<ExitToLobbyHooks> ();
		if (hooks == null)
			return;
		
		hooks.OnExitHook = OnGUIExitToLobby;
		
		EventSystem.current.firstSelectedGameObject = hooks.firstButton.gameObject;
		EventSystem.current.SetSelectedGameObject (hooks.firstButton.gameObject);
	}
	
	public override void OnLevelWasLoaded ()
	{
		if (canvas == null)
			return;
		
		var hooks = canvas.GetComponent<ExitToLobbyHooks> ();
		if (hooks == null)
			return;
		
		EventSystem.current.firstSelectedGameObject = hooks.firstButton.gameObject;
		EventSystem.current.SetSelectedGameObject (hooks.firstButton.gameObject);
	}
	
	public void OnGUIExitToLobby ()
	{
		foreach (var player in BluetoothLobbyManager.Instance.lobbySlots) {
			if (player != null) {
				var playerLobby = player as PlayerLobby;
				if (playerLobby) {
					playerLobby.CmdExitToLobby ();
				}
			}
		}
	}
}
