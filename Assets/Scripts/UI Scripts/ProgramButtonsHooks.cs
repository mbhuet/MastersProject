using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgramButtonsHooks : MonoBehaviour{

	public delegate void ButtonHook();

	public ButtonHook OnPause;
	public ButtonHook OnResume;
	public ButtonHook OnReset;
	public ButtonHook OnReady;

	public Button readyButton;
	public Button pauseButton;
	public Button resumeButton;
	public Button resetButton;
	
	
	public void UIPause(){
		OnPause ();
	}
	
	public void UIResume(){
		OnResume ();
	}

	
	public void UIReset(){
		OnReset ();
	}

	public void UIReady(){
		OnReady ();
	}

}
