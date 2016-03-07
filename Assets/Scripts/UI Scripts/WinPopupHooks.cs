using UnityEngine;
using UnityEngine.UI;

public class WinPopupHooks : MonoBehaviour {
	
	public delegate void CanvasHook();
	public CanvasHook OnExitHook;
	public CanvasHook OnReplayHook;
	
	public Button replayButton;
	public Button exitButton;
	public Text titleText;
	//public Text messagText;
	
	public void UIExit()
	{
		if (OnExitHook != null)
			OnExitHook.Invoke();
	}

	public void UIReplay(){
		if (OnExitHook != null)
			OnExitHook.Invoke();
	}
}

