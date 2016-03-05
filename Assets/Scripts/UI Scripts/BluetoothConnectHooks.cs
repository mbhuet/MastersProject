using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class BluetoothConnectHooks : MonoBehaviour {

	public delegate void ButtonHook();
	
	public ButtonHook OnHost;
	public ButtonHook OnFind;
	
	public Button hostButton;
	public Button findButton;
	
	
	public void UIHost(){
		if (OnHost != null)
			OnHost.Invoke();
	}
	
	public void UIFind(){
		if (OnFind != null) {
			OnFind.Invoke();
		}
	}
}
