using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.EventSystems;

public class PlayerLobby : NetworkLobbyPlayer
{
	public GameObject playerPanelPrefab;

	public GameObject playerPanel;

	// cached components
	ColorControl cc;
	NetworkLobbyPlayer lobbyPlayer;

	void Awake()
	{
		cc = GetComponent<ColorControl>();
		lobbyPlayer = GetComponent<NetworkLobbyPlayer>();
	}

	public override void OnClientEnterLobby()
	{
		if (playerPanel == null)
		{
			playerPanel = (GameObject)Instantiate(playerPanelPrefab, Vector3.zero, Quaternion.identity);
			playerPanel.transform.SetParent(GuiLobbyManager.s_Singleton.lobbyCanvas.canvas.transform.GetChild(0).transform, false);
		}

		var hooks = playerPanel.GetComponent<PlayerCanvasHooks>();
		//hooks.panelPos.localPosition = new Vector3(GetPlayerPos(lobbyPlayer.slot), 0, 0);
		hooks.SetColor(cc.myColor);
		hooks.SetReady(lobbyPlayer.readyToBegin);

		//EventSystem.current.SetSelectedGameObject(hooks.colorButton.gameObject);
	}

	public override void OnClientExitLobby()
	{
		if (playerPanel != null)
		{
			Destroy(playerPanel.gameObject);
		}
	}

	public override void OnClientReady(bool readyState)
	{
		var hooks = playerPanel.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(readyState);
	}

	/*
	float GetPlayerPos(int slot)
	{
		var lobby = NetworkManager.singleton as GuiLobbyManager;
		if (lobby == null)
		{
			// no lobby?
			return slot * 200;
		}

		// this spreads the player canvas panels out across the screen
		var screenWidth = playerPanel.pixelRect.width;
		screenWidth -= 200; // border padding
		var playerWidth = screenWidth / (lobby.maxPlayers-1);
		return -(screenWidth / 2) + slot * playerWidth;
	}
	*/

	public override void OnStartLocalPlayer()
	{
		if (playerPanel == null)
		{
			playerPanel = (GameObject)Instantiate(playerPanelPrefab, Vector3.zero, Quaternion.identity);
//			playerPanel.sortingOrder = 1;
		}

		// setup button hooks
		var hooks = playerPanel.GetComponent<PlayerCanvasHooks>();
		//hooks.panelPos.localPosition = new Vector3(GetPlayerPos(lobbyPlayer.slot), 0, 0);
		playerPanel.transform.SetParent(GuiLobbyManager.s_Singleton.lobbyCanvas.canvas.transform.GetChild(0).transform, false);

		hooks.SetColor(cc.myColor);

		hooks.OnColorChangeHook = OnGUIColorChange;
		hooks.OnReadyHook = OnGUIReady;
		hooks.OnRemoveHook = OnGUIRemove;
		hooks.SetLocalPlayer();
	}

	void OnDestroy()
	{
		if (playerPanel != null)
		{
			Destroy(playerPanel.gameObject);
		}
	}

	public void SetColor(Color color)
	{
		var hooks = playerPanel.GetComponent<PlayerCanvasHooks>();
		hooks.SetColor(color);
	}

	public void SetReady(bool ready)
	{
		var hooks = playerPanel.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(ready);
	}

	[Command]
	public void CmdExitToLobby()
	{
		var lobby = NetworkManager.singleton as GuiLobbyManager;
		if (lobby != null)
		{
			lobby.ServerReturnToLobby();
		}
	}

	// events from UI system

	void OnGUIColorChange()
	{
		if (isLocalPlayer)
			cc.ClientChangeColor();
	}

	void OnGUIReady()
	{
		if (isLocalPlayer)
			lobbyPlayer.SendReadyToBeginMessage();
	}

	void OnGUIRemove()
	{
		if (isLocalPlayer)
		{
			ClientScene.RemovePlayer(lobbyPlayer.playerControllerId);

			var lobby = NetworkManager.singleton as GuiLobbyManager;
			if (lobby != null)
			{
				lobby.SetFocusToAddPlayerButton();
			}
		}
	}
}

