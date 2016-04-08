using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using LostPolygon.AndroidBluetoothMultiplayer;
using LostPolygon.AndroidBluetoothMultiplayer.Examples;


public class BluetoothNetworkManager: MonoBehaviour {

#if UNITY_ANDROID
	public int maxPlayers;
	public int minPlayers;

	public string lobbySceneName;
	public string playSceneName;

	protected const string kLocalIp = "127.0.0.1"; // An IP for Network.Connect(), must always be 127.0.0.1
	protected const int kPort = 28000; // Local server IP. Must be the same for client and server
	protected bool _initResult;
	protected BluetoothMultiplayerMode _desiredMode = BluetoothMultiplayerMode.None;


	private void Awake() {
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
	}
	
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
	public void StartHostBluetooth(){
		Debug.Log ("StartHostBluetooth\n-------\n------------\n------------\n----------\n--------\n------------\n----------");
		
		if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled()) {
			Debug.Log ("Bluetooth is Enabled");
			
			AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
			Network.Disconnect(); // Just to be sure
			AndroidBluetoothMultiplayer.StartServer(kPort);
			
		} else {
			// Otherwise we have to enable Bluetooth first and wait for callback
			Debug.Log ("Bluetooth is Disabled");
			
			_desiredMode = BluetoothMultiplayerMode.Server;
			AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
		}

		
	}
	
	public void StartClientBluetooth(){
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
	
	protected virtual void OnBluetoothListeningStarted() {
		Debug.Log("Event - ListeningStarted");
		
		// Starting Unity networking server if Bluetooth listening started successfully
		Network.InitializeServer(maxPlayers, kPort, false);
	}
	
	protected virtual void OnBluetoothListeningStopped() {
		Debug.Log("Event - ListeningStopped");
		
		// For demo simplicity, stop server if listening was canceled
		AndroidBluetoothMultiplayer.Stop();
	}
	
	protected virtual void OnBluetoothDevicePicked(BluetoothDevice device) {
		Debug.Log("Event - DevicePicked: " + BluetoothExamplesTools.FormatDevice(device));
		
		// Trying to connect to a device user had picked
		AndroidBluetoothMultiplayer.Connect(device.Address, kPort);
	}
	
	protected virtual void OnBluetoothClientDisconnected(BluetoothDevice device) {
		Debug.Log("Event - ClientDisconnected: " + BluetoothExamplesTools.FormatDevice(device));
	}
	
	protected virtual void OnBluetoothClientConnected(BluetoothDevice device) {
		Debug.Log("Event - ClientConnected: " + BluetoothExamplesTools.FormatDevice(device));
	}
	
	protected virtual void OnBluetoothDisconnectedFromServer(BluetoothDevice device) {
		Debug.Log("Event - DisconnectedFromServer: " + BluetoothExamplesTools.FormatDevice(device));
		
		// Stopping Unity networking on Bluetooth failure
		Network.Disconnect();
	}
	
	protected virtual void OnBluetoothConnectionToServerFailed(BluetoothDevice device) {
		Debug.Log("Event - ConnectionToServerFailed: " + BluetoothExamplesTools.FormatDevice(device));
	}
	
	private void OnBluetoothConnectedToServer(BluetoothDevice device) {
		Debug.Log("Event - ConnectedToServer: " + BluetoothExamplesTools.FormatDevice(device));
		
		// Trying to negotiate a Unity networking connection, 
		// when Bluetooth client connected successfully
		Network.Connect(kLocalIp, kPort);
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
		
		TestActor[] objects = GameObject.FindObjectsOfType(typeof(TestActor)) as TestActor[];
		if (objects != null) {
			foreach (TestActor obj in objects) {
				GameObject.Destroy(obj.gameObject);
			}
		}
	}
	
	protected virtual void OnConnectedToServer() {
		Debug.Log("Connected to server");
		
		// Instantiating a simple test actor
		//Network.Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity, 0);
	}
	
	protected virtual void OnServerInitialized() {
		Debug.Log("Server initialized");
		
		// Instantiating a simple test actor
		if (Network.isServer) {
			//Network.Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity, 0);
		}
	}
	
	#endregion Network events
#endif
}
