using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

[Serializable]
public class LevelCanvasControl : CanvasControl
{
	public override void Show()
	{
		base.Show();
		
		var hooks = canvas.GetComponent<LevelCanvasHooks>();
		if (hooks == null)
			return;
		
		hooks.OnLevelSelectHook += OnGUILevelSelected; 
		
	}
	
	public void OnGUILevelSelected(string levelName){
		//		Debug.Log("OnGUILevelSelected");
		Hide ();
		GuiLobbyManager.s_Singleton.ServerChangeScene(levelName);
	}
}