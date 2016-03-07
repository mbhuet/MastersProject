using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {
	static public GameUIManager Instance;

	public WinPopupCanvasControl winPopupCanvas;
	public ProgramButtonsCanvasControl programButtonsCanvas;

	//public OfflineCanvasControl offlineCanvas;

	void Start()
	{
		Instance = this;
		GameManager.OnLevelComplete += OnLevelComplete;
		programButtonsCanvas.Show ();
	}
	
	void OnLevelWasLoaded()
	{
		if (winPopupCanvas != null) winPopupCanvas.OnLevelWasLoaded();
		if (programButtonsCanvas != null) programButtonsCanvas.OnLevelWasLoaded();
	}

	void OnDestroy(){
		programButtonsCanvas.Hide ();
		winPopupCanvas.Hide ();
	}

	void OnLevelComplete(){
		winPopupCanvas.Show ();
	}

}
