using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager {

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject thePlayer = (GameObject)Instantiate(base.playerPrefab, Vector3.zero, Quaternion.identity);
		Player player = thePlayer.GetComponent<Player>();

		NetworkServer.AddPlayerForConnection(conn, thePlayer, playerControllerId);
	}
}
