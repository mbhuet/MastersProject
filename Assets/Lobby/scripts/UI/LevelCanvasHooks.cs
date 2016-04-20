using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class LevelCanvasHooks : MonoBehaviour{

	public GameObject buttonPrefab;
	public SceneInfo[] scenes;

	public delegate void CanvasHook();
	public delegate void ButtonHook(string s);
	
	public ButtonHook OnLevelSelectHook;
	Dictionary<string, string> sceneNames;
	
	public void UILoadLevel(string name)
	{
		Debug.Log ("UILoadLevel " + name);
		if (OnLevelSelectHook != null)
			OnLevelSelectHook.Invoke(name);
//		Debug.Log("UILoadLevel");
	}

	public void UILoadLevel(){

	}

	public void Test(){
		Debug.Log ("ButtonTest ");

	}

	public void MakeButtons(){
		sceneNames = new Dictionary<string, string> ();

		GameObject panel = transform.FindChild ("Panel").gameObject;
		foreach(SceneInfo info in scenes){
			GameObject buttonObject = GameObject.Instantiate(buttonPrefab);
			buttonObject.transform.SetParent(panel.transform);

			Button button = buttonObject.GetComponent<Button>();
			Button b2 = button;
			b2.onClick.AddListener(() => { UILoadLevel(info.fileName); });
			//button.onClick.AddListener(Test);

			Text textObj = buttonObject.transform.FindChild("Text").GetComponent<Text>();
			textObj.text = info.displayName;

			Image imageObj = buttonObject.GetComponent<Image>();
			//imageObj.mainTexture = info.image;
		}
	}

}

[System.Serializable]
public class SceneInfo
{
	public string fileName;
	public string displayName;
	public int requiredPlayers;
	public Texture2D image;
	public string notes;

}
