using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

[Serializable]
public class WinPopupCanvasControl : CanvasControl
{
	public override void Show()
	{
		base.Show();
		
		var hooks = canvas.GetComponent<WinPopupHooks>();
		if (hooks == null)
			return;
		
		hooks.OnExitHook += OnGUIReturnToLevelSelect; 
		hooks.OnReplayHook += OnGUIReplay;
	}

	void OnGUIReturnToLevelSelect(){
		Application.LoadLevel ("LevelSelect");
	}

	void OnGUIReplay(){
		Hide ();
	}
}


[Serializable]
public class ProgramButtonsCanvasControl: CanvasControl
{
	public override void Show(){
		base.Show();
		
		Debug.Log ("ProgramButtonsControl Init");
		var hooks = canvas.GetComponent<ProgramButtonsHooks>();
		if (hooks == null)
			return;
		//Debug.Log ("ExecutionManager.Instance :: " + ExecutionManager.Instance);
		
		hooks.OnPause += ExecutionManager.Pause;
		hooks.OnPause += OnGUIPause;
		
		hooks.OnResume += ExecutionManager.Resume;
		hooks.OnResume += OnGUIResume;
		
		hooks.OnReady += PlayerManager.Instance.ToggleLocalPlayerReady;
		hooks.OnReady += OnGUIReady;
		
		hooks.OnReset += Level.Instance.ResetLevel;
		hooks.OnReset += OnGUIReset;
		
		ExecutionManager.BeginExecutionEvent += ShowRuntimeControls;
		
		ShowProgrammingControls ();
		
	}
	
	public void ShowRuntimeControls ()
	{
		var hooks = canvas.GetComponent<ProgramButtonsHooks>();
		if (hooks == null)
			return;
		hooks.resumeButton.gameObject.SetActive (false);
		hooks.pauseButton.gameObject.SetActive (true);
		hooks.resetButton.gameObject.SetActive (true);
		hooks.readyButton.gameObject.SetActive (false);
	}
	
	public void ShowProgrammingControls ()
	{
		var hooks = canvas.GetComponent<ProgramButtonsHooks>();
		if (hooks == null)
			return;
		hooks.resumeButton.gameObject.SetActive (false);
		hooks.pauseButton.gameObject.SetActive (false);
		hooks.resetButton.gameObject.SetActive (false);
		hooks.readyButton.gameObject.SetActive (true);
	}
	
	public void OnGUIReady ()
	{
		var hooks = canvas.GetComponent<ProgramButtonsHooks>();
		if (hooks == null)
			return;
		Debug.Log ("OnGUIReady");
		if (PlayerManager.Instance.localPlayer.isReady) {
			hooks.readyButton.GetComponent<Image>().color = Color.green;
		}
		else {
			hooks.readyButton.GetComponent<Image>().color = Color.white;
		}
		SetReadyButtonText ("Ready (" + PlayerManager.Instance.numReadyPlayers + "/" + GameManager.Instance.numPlayers + ")");
		
	}
	
	public void OnGUIPause ()
	{
		Debug.Log ("OnGUIPause");
		var hooks = canvas.GetComponent<ProgramButtonsHooks>();
		if (hooks == null)
			return;
		
		hooks.resumeButton.gameObject.SetActive (true);
		hooks.pauseButton.gameObject.SetActive (false);
		
	}
	
	public void OnGUIResume ()
	{
		Debug.Log ("OnGUIResume");
		
		var hooks = canvas.GetComponent<ProgramButtonsHooks> ();
		if (hooks == null)
			return;
		
		hooks.resumeButton.gameObject.SetActive (false);
		hooks.pauseButton.gameObject.SetActive (true);
	}
	
	public void OnGUIReset ()
	{
		Debug.Log ("OnGUIReset");
		ExecutionManager.Instance.StopExecution ();
		ShowProgrammingControls ();
		SetReadyButtonText ("Ready");
		OnGUIReady ();
		
	}
	
	public void SetReadyButtonText (string text)
	{
		var hooks = canvas.GetComponent<ProgramButtonsHooks>();
		if (hooks == null)
			return;
		if (hooks.readyButton.IsActive ()) {
			hooks.readyButton.GetComponentInChildren<Text> ().text = text;
		}
	}
}

