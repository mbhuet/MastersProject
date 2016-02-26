using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class GuiLobbyManager : NetworkLobbyManager
{
	public LobbyCanvasControl lobbyCanvas;
	public OfflineCanvasControl offlineCanvas;
	public OnlineCanvasControl onlineCanvas;
	public ExitToLobbyCanvasControl exitToLobbyCanvas;
	public ConnectingCanvasControl connectingCanvas;
	public PopupCanvasControl popupCanvas;
	public MatchMakerCanvasControl matchMakerCanvas;
	public JoinMatchCanvasControl joinMatchCanvas;


	public string onlineStatus;
	static public GuiLobbyManager s_Singleton;

	void Start()
	{
		s_Singleton = this;
		offlineCanvas.Show();
	}

	void OnLevelWasLoaded()
	{
		if (lobbyCanvas != null) lobbyCanvas.OnLevelWasLoaded();
		if (offlineCanvas != null) offlineCanvas.OnLevelWasLoaded();
		if (onlineCanvas != null) onlineCanvas.OnLevelWasLoaded();
		if (exitToLobbyCanvas != null) exitToLobbyCanvas.OnLevelWasLoaded();
		if (connectingCanvas != null) connectingCanvas.OnLevelWasLoaded();
		if (popupCanvas != null) popupCanvas.OnLevelWasLoaded();
		if (matchMakerCanvas != null) matchMakerCanvas.OnLevelWasLoaded();
		if (joinMatchCanvas != null) joinMatchCanvas.OnLevelWasLoaded();

	}

	public void SetFocusToAddPlayerButton()
	{
		if (lobbyCanvas == null)
			return;

		lobbyCanvas.SetFocusToAddPlayerButton();
	}

	// ----------------- Server callbacks ------------------

	public override void OnLobbyStopHost()
	{
		lobbyCanvas.Hide();
		offlineCanvas.Show();
	}

	// ----------------- Client callbacks ------------------

	public override void OnLobbyClientConnect(NetworkConnection conn)
	{
		connectingCanvas.Hide();
	}

	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
		connectingCanvas.Hide();
		StopHost();

		popupCanvas.Show("Client Error", errorCode.ToString());
	}

	public override void OnLobbyClientDisconnect(NetworkConnection conn)
	{
		lobbyCanvas.Hide();
		offlineCanvas.Show();
	}

	public override void OnLobbyStartClient(NetworkClient client)
	{
		if (matchInfo != null)
		{
			connectingCanvas.Show(matchInfo.address);
		}
		else
		{
			connectingCanvas.Show(networkAddress);
		}
	}

	public override void OnLobbyClientAddPlayerFailed()
	{
		popupCanvas.Show("Error", "No more players allowed.");
	}

	public override void OnLobbyClientEnter()
	{
		lobbyCanvas.Show();
		onlineCanvas.Show(onlineStatus);

		exitToLobbyCanvas.Hide();

	}

	public override void OnLobbyClientExit()
	{
		lobbyCanvas.Hide();
		onlineCanvas.Hide();

		if (Application.loadedLevelName == base.playScene)
		{
			exitToLobbyCanvas.Show();
		}
	}

	/*
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject thePlayer = (GameObject)Instantiate(base.playerPrefab, Vector3.zero, Quaternion.identity);
		Player player = thePlayer.GetComponent<Player>();
		NetworkServer.AddPlayerForConnection(conn, thePlayer, playerControllerId);
		PlayerManager.Instance.RegisterPlayer(player);
	}
	*/

	/*
	public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId){
		GameObject playerObj = (GameObject)Instantiate(base.gamePlayerPrefab, Vector3.zero, Quaternion.identity);
		Debug.Log(playerObj);
		Player player = playerObj.GetComponent<Player>();
		PlayerManager.Instance.RegisterPlayer(player);
		return playerObj;
	}
	*/



	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		int playerNum = lobbyPlayer.GetComponent<NetworkLobbyPlayer>().slot;
		gamePlayer.GetComponent<Player>().playerNum = playerNum;
//		Debug.Log("OnLobbyServerSceneLoadedForPlayer " + playerNum);
		//PlayerManager.Instance.RegisterPlayer(gamePlayer.GetComponent<Player>(), playerNum);
		return true;
	}


}
