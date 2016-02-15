using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[Serializable]
public class ProgramButtonsControl
{
	[SerializeField]
	public Canvas prefab;
	Canvas m_Canvas;
	public Canvas canvas { get { return m_Canvas;} }


	ProgramControlsHooks hooks;

	public void Init(){
		Debug.Log ("ProgramButtonsControl Init");
		m_Canvas =  (Canvas)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
		hooks = canvas.GetComponent<ProgramControlsHooks> ();
		Debug.Log ("ExecutionManager.Instance :: " + ExecutionManager.Instance);


		hooks.OnPause += ExecutionManager.Pause;
		hooks.OnPause += OnGUIPause;

		hooks.OnResume += ExecutionManager.Resume;
		hooks.OnResume += OnGUIResume;

		hooks.OnReady += PlayerManager.Instance.ToggleLocalPlayerReady;
		hooks.OnReady += OnGUIReady;

		hooks.OnReset += Level.Instance.ResetLevel;
		hooks.OnReset += OnGUIReset;
		
		//ExecutionManager.Instance.EventBeginExecution += ShowRuntimeControls;

		ShowProgrammingControls ();

	}

	public void ShowRuntimeControls ()
	{
		hooks.resumeButton.gameObject.SetActive (false);
		hooks.pauseButton.gameObject.SetActive (true);
		hooks.resetButton.gameObject.SetActive (true);
		hooks.readyButton.gameObject.SetActive (false);
	}

	public void ShowProgrammingControls ()
	{
		hooks.resumeButton.gameObject.SetActive (false);
		hooks.pauseButton.gameObject.SetActive (false);
		hooks.resetButton.gameObject.SetActive (false);
		hooks.readyButton.gameObject.SetActive (true);
	}

	public void OnGUIReady ()
	{
		Debug.Log ("OnGUIReady");
		if (PlayerManager.Instance.localPlayer.isReady) {
			hooks.readyButton.GetComponent<Image>().color = Color.green;
		}
		else {
			hooks.readyButton.GetComponent<Image>().color = Color.white;
		}
	}

	public void OnGUIPause ()
	{
		Debug.Log ("OnGUIPause");

		hooks.resumeButton.gameObject.SetActive (true);
		hooks.pauseButton.gameObject.SetActive (false);
			
	}

	public void OnGUIResume ()
	{
		Debug.Log ("OnGUIResume");

		var hooks = canvas.GetComponent<ProgramControlsHooks> ();
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
		if (hooks.readyButton.IsActive ()) {
			hooks.readyButton.GetComponentInChildren<Text> ().text = text;
		}
	}
}

