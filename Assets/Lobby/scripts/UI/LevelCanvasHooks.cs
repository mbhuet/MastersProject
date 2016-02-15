using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelCanvasHooks : MonoBehaviour{

	public delegate void CanvasHook();
	public delegate void ButtonHook(string s);
	
	public ButtonHook OnLevelSelectHook;


	
	public void UILoadLevel(string name)
	{
		if (OnLevelSelectHook != null)
			OnLevelSelectHook.Invoke(name);
//		Debug.Log("UILoadLevel");

	}

}
