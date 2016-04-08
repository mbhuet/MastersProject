using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using LostPolygon.AndroidBluetoothMultiplayer;
using LostPolygon.AndroidBluetoothMultiplayer.Examples;

public class BluetoothLobbyManager : NetworkLobbyManager
{
	public BluetoothLobbyCanvasControl lobbyCanvas;
	public BluetoothConnectCanvasControl offlineCanvas;
	public BluetoothOnlineCanvasControl onlineCanvas;
	public BluetoothExitToLobbyCanvasControl exitToLobbyCanvas;
	public ConnectingCanvasControl connectingCanvas;
	public PopupCanvasControl popupCanvas;
	public MatchMakerCanvasControl matchMakerCanvas;
	public JoinMatchCanvasControl joinMatchCanvas;
	
	public string onlineStatus;
	static public BluetoothLobbyManager Instance;
	
	NetworkClient myClient;
	public bool isAtStartup = true;
	
	private const string kLocalIp = "127.0.0.1"; // An IP for Network.Connect(), must always be 127.0.0.1
	private const int kPort = 28000; // Local server IP. Must be the same for client and server
	
	private bool _initResult;

	
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


	void Start()
	{
		Instance = this;
		offlineCanvas.Show();

#if UNITY_ANDROID
		// Setting the UUID. Must be unique for every application
		_initResult = AndroidBluetoothMultiplayer.Initialize("8ce255c0-200a-11e0-ac64-0800200c9a66");
		
		// Enabling verbose logging. See log cat!
		AndroidBluetoothMultiplayer.SetVerboseLog(true);
		
		// Registering the event delegates
		AndroidBluetoothMultiplayer.ListeningStarted += OnBluetoothListeningStarted;
		AndroidBluetoothMultiplayer.ListeningStopped += OnBluetoothListeningStopped;
		AndroidBluetoothMultiplayer.AdapterEnabled += OnBluetoothAdapterEnabled;
		AndroidBluetoothMultiplayer.AdapterEnableFailed += OnBluetoothAdapterEnableFailed;
		AndroidBluetoothMultiplayer.AdapterDisabled += OnBluetoothAdapterDisabled;
		AndroidBluetoothMultiplayer.DiscoverabilityEnabled += OnBluetoothDiscoverabilityEnabled;
		AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed += OnBluetoothDiscoverabilityEnableFailed;
		AndroidBluetoothMultiplayer.ConnectedToServer += OnBluetoothConnectedToServer;
		AndroidBluetoothMultiplayer.ConnectionToServerFailed += OnBluetoothConnectionToServerFailed;
		AndroidBluetoothMultiplayer.DisconnectedFromServer += OnBluetoothDisconnectedFromServer;
		AndroidBluetoothMultiplayer.ClientConnected += OnBluetoothClientConnected;
		AndroidBluetoothMultiplayer.ClientDisconnected += OnBluetoothClientDisconnected;
		AndroidBluetoothMultiplayer.DevicePicked += OnBluetoothDevicePicked;
		#endif

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
		lobbyCanvas.Hide ();
		onlineCanvas.Hide ();
		
		if (Application.loadedLevelName == base.playScene) {
			exitToLobbyCanvas.Show ();
		}
	}
	
	
	
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		int playerNum = lobbyPlayer.GetComponent<NetworkLobbyPlayer>().slot;
		gamePlayer.GetComponent<Player>().playerNum = playerNum;
		return true;
	}

#if UNITY_ANDROID
	private BluetoothMultiplayerMode _desiredMode = BluetoothMultiplayerMode.None;

	
	// Don't forget to unregister the event delegates!
	protected void OnDestroy() {
		AndroidBluetoothMultiplayer.ListeningStarted -= OnBluetoothListeningStarted;
		AndroidBluetoothMultiplayer.ListeningStopped -= OnBluetoothListeningStopped;
		AndroidBluetoothMultiplayer.AdapterEnabled -= OnBluetoothAdapterEnabled;
		AndroidBluetoothMultiplayer.AdapterEnableFailed -= OnBluetoothAdapterEnableFailed;
		AndroidBluetoothMultiplayer.AdapterDisabled -= OnBluetoothAdapterDisabled;
		AndroidBluetoothMultiplayer.DiscoverabilityEnabled -= OnBluetoothDiscoverabilityEnabled;
		AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed -= OnBluetoothDiscoverabilityEnableFailed;
		AndroidBluetoothMultiplayer.ConnectedToServer -= OnBluetoothConnectedToServer;
		AndroidBluetoothMultiplayer.ConnectionToServerFailed -= OnBluetoothConnectionToServerFailed;
		AndroidBluetoothMultiplayer.DisconnectedFromServer -= OnBluetoothDisconnectedFromServer;
		AndroidBluetoothMultiplayer.ClientConnected -= OnBluetoothClientConnected;
		AndroidBluetoothMultiplayer.ClientDisconnected -= OnBluetoothClientDisconnected;
		AndroidBluetoothMultiplayer.DevicePicked -= OnBluetoothDevicePicked;
	}
	
	
	


	public void StartBluetoothHost(){
		if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled ()) {
			AndroidBluetoothMultiplayer.RequestEnableDiscoverability (120);
			//Network.Disconnect (); // Just to be sure
			AndroidBluetoothMultiplayer.StartServer (kPort);
		} else {
			// Otherwise we have to enable Bluetooth first and wait for callback
			_desiredMode = BluetoothMultiplayerMode.Server;
			AndroidBluetoothMultiplayer.RequestEnableDiscoverability (120);
		}
	}
	
	public void ConnectToBluetoothServer(){
		if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled()) {
			Network.Disconnect(); // Just to be sure
			AndroidBluetoothMultiplayer.ShowDeviceList(); // Open device picker dialog
		} else {
			// Otherwise we have to enable Bluetooth first and wait for callback
			_desiredMode = BluetoothMultiplayerMode.Client;
			AndroidBluetoothMultiplayer.RequestEnableBluetooth();
		}
	}	


	#region Bluetooth events
	
	private void OnBluetoothListeningStarted() {
		Debug.Log("Event - ListeningStarted");
		networkPort = kPort;
		StartHost ();
		// Starting Unity networking server if Bluetooth listening started successfully
		//Network.InitializeServer(4, kPort, false);
	}
	
	private void OnBluetoothListeningStopped() {
		Debug.Log("Event - ListeningStopped");
		
		// For demo simplicity, stop server if listening was canceled
		AndroidBluetoothMultiplayer.Stop();
	}
	
	private void OnBluetoothDevicePicked(BluetoothDevice device) {
		Debug.Log("Event - DevicePicked: " + BluetoothExamplesTools.FormatDevice(device));
		
		// Trying to connect to a device user had picked
		AndroidBluetoothMultiplayer.Connect(device.Address, kPort);
	}
	
	private void OnBluetoothClientDisconnected(BluetoothDevice device) {
		Debug.Log("Event - ClientDisconnected: " + BluetoothExamplesTools.FormatDevice(device));
	}
	
	private void OnBluetoothClientConnected(BluetoothDevice device) {
		Debug.Log("Event - ClientConnected: " + BluetoothExamplesTools.FormatDevice(device));
	}
	
	private void OnBluetoothDisconnectedFromServer(BluetoothDevice device) {
		Debug.Log("Event - DisconnectedFromServer: " + BluetoothExamplesTools.FormatDevice(device));
		
		// Stopping Unity networking on Bluetooth failure
		
		Network.Disconnect();
	}
	
	private void OnBluetoothConnectionToServerFailed(BluetoothDevice device) {
		Debug.Log("Event - ConnectionToServerFailed: " + BluetoothExamplesTools.FormatDevice(device));
	}
	
	private void OnBluetoothConnectedToServer(BluetoothDevice device) {
		Debug.Log("Event - ConnectedToServer: " + BluetoothExamplesTools.FormatDevice(device));
		
		// Trying to negotiate a Unity networking connection, 
		// when Bluetooth client connected successfully
		networkAddress = kLocalIp;
		networkPort = kPort;
		StartClient ();
		//Network.Connect(kLocalIp, kPort);
	}
	
	private void OnBluetoothAdapterDisabled() {
		Debug.Log("Event - AdapterDisabled");
	}
	
	private void OnBluetoothAdapterEnableFailed() {
		Debug.Log("Event - AdapterEnableFailed");
	}
	
	private void OnBluetoothAdapterEnabled() {
		Debug.Log("Event - AdapterEnabled");
		
		// Resuming desired action after enabling the adapter
		switch (_desiredMode) {
		case BluetoothMultiplayerMode.Server:
			Network.Disconnect();
			AndroidBluetoothMultiplayer.StartServer(kPort);
			break;
		case BluetoothMultiplayerMode.Client:
			Network.Disconnect();
			AndroidBluetoothMultiplayer.ShowDeviceList();
			break;
		}
		
		_desiredMode = BluetoothMultiplayerMode.None;
	}
	
	private void OnBluetoothDiscoverabilityEnableFailed() {
		Debug.Log("Event - DiscoverabilityEnableFailed");
	}
	
	private void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration) {
		Debug.Log(string.Format("Event - DiscoverabilityEnabled: {0} seconds", discoverabilityDuration));
	}
	
	#endregion Bluetooth events
	
	#region Network events
	
	private void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player disconnected: " + player.GetHashCode());
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	private void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Can't connect to the networking server");
		
		// Stopping all Bluetooth connectivity on Unity networking disconnect event
		AndroidBluetoothMultiplayer.Stop();
	}
	
	private void OnDisconnectedFromServer() {
		Debug.Log("Disconnected from server");
		
		// Stopping all Bluetooth connectivity on Unity networking disconnect event
		AndroidBluetoothMultiplayer.Stop();
	}
	
	private void OnConnectedToServer() {
		Debug.Log("Connected to server - (OnConnectedToServer)");
		
		// Instantiating a simple test actor
		//Network.Instantiate(ActorPrefab, new Vector3(Random.Range(-40f, 40f), Random.Range(-40f, 40f), 0), Quaternion.identity, 0);
		//		NetworkServer.Spawn(ActorPrefab);
		
	}
	
	private void OnServerInitialized() {
		Debug.Log("Server initialized");
		// Instantiating a simple test actor
		if (Network.isServer) {
			//Network.Instantiate(ActorPrefab, Vector3.zero, Quaternion.identity, 0);
		}
	}
	
	// Create a server and listen on a port
	public void SetupServer()
	{
		NetworkServer.Listen(kPort);
		
	}
	
	#endregion Network events
#endif
	



	
	
}
