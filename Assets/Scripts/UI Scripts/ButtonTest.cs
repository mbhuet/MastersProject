using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ButtonTest : MonoBehaviour {
	public GameObject programPanel;

	public void printProgram(){
		List<Command> list = new List<Command> ();
		string comString = "";
		for (int i = 0; i< programPanel.transform.childCount; i++) {
			CommandTile com = programPanel.transform.GetChild(i).GetComponent<CommandTile>();
			if(com == null){
				continue;
			}

			list.Add(com.command);
			comString += com.command + ", ";
		}
		Debug.Log (comString);
	}
}
