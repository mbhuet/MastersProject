using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager {

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject thePlayer = (GameObject)Instantiate(base.playerPrefab, Vector3.zero, Quaternion.identity);
		Player player = thePlayer.GetComponent<Player>();
		//player.Init(NetworkServer.connections.Count);
		NetworkServer.AddPlayerForConnection(conn, thePlayer, playerControllerId);
		//player.RpcTest(55);
		PlayerManager.Instance.RegisterPlayer(player);
	}

}
