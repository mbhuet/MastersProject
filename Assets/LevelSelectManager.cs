using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LevelSelectManager : MonoBehaviour {
	public LevelCanvasControl levelCanvas;

	static public LevelSelectManager Instance;

	// Use this for initialization
	void Start () {
		Instance = this;
		//Debug.Log (Network.isServer);
		//Debug.Log (NetworkServer.active);
		if (NetworkServer.active) {
			levelCanvas.Show ();
		}

	}
	
	// Update is called once per frame
	void Update () {
		//if (NetworkServer.active);

	}
}
