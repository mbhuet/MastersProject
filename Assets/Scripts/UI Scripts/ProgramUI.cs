﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProgramUI : MonoBehaviour {
	Canvas canvas;
	public GameObject functionZonePrefab;
	public GameObject commandDockPrefab;
	public Text textPrefab;
	public TileBank tileBankPrefab;

	public CommandTile forward;
	public CommandTile backward;
	public CommandTile turnLeft;
	public CommandTile turnRight;
	public CommandTile wait;

	Dictionary<Command, CommandTile> commandDict;
	List<FunctionZone> functionZones;

	Command[] antCommands = {
		Command.FORWARD, 
		Command.BACKWARD, 
		Command.TURN_L, 
		Command.TURN_R, 
		Command.WAIT
	};

	Command[] fireCommands;
	Command[] warriorCommands;
	Command[] scoutCommands;
	Command[] carpenterCommands;

	GameObject commandDock;
	public GameObject functionsPanel;
	public Button readyButton;

	ProgramManager localProgramManager;

	// Use this for initialization
	void Awake () {
		canvas = this.GetComponent<Canvas>();
		BuildCommandDictionary();
		functionZones = new List<FunctionZone>();
		FindFunctionZones();

	}

	void FindFunctionZones(){
		foreach(FunctionZone funcZone in gameObject.GetComponentsInChildren<FunctionZone>()){
			functionZones.Add(funcZone);
		}
	}

	public void SetLocalProgramManager(ProgramManager prog){
		localProgramManager = prog;
	}

	public CommandTile[] GetCommandTilesFromFunctionZone(int index){
		if(index >= functionZones.Count) return null;
		return functionZones[index].GetCommandTiles();
	}


	public void BuildUIFromBlueprint(ProgramBlueprint blueprint){
		int numFunctions = blueprint.availableFunctions.Length;

		commandDock = GameObject.Instantiate(commandDockPrefab);
		commandDock.transform.SetParent(canvas.transform, false);

		foreach(Command com in antCommands){
			TileBank bank = GameObject.Instantiate(tileBankPrefab);
			CommandTile tile;
			commandDict.TryGetValue(com, out tile);
			bank.Init(tile);
			bank.transform.SetParent(commandDock.transform);
		}
		//TODO CHECK ProgramBlueprints for other players and make TileBanks for their global functions


		for(int i = 0; i< numFunctions; i++){
			Text funcTitle = GameObject.Instantiate(textPrefab) as Text;
			funcTitle.text = blueprint.availableFunctions[i].funcName;
			funcTitle.transform.SetParent(functionsPanel.transform, false);
			GameObject func = GameObject.Instantiate(functionZonePrefab);
			functionZones.Add(func.GetComponent<FunctionZone>());
			func.transform.SetParent (functionsPanel.transform, false);
			func.GetComponent<FunctionZone>().Init(localProgramManager, blueprint.availableFunctions[i].numSlots, i);
		}
	
	}

	void BuildCommandDictionary(){
		commandDict = new Dictionary<Command, CommandTile>();
		commandDict.Add(Command.FORWARD, forward);
		commandDict.Add(Command.BACKWARD, backward);
		commandDict.Add(Command.TURN_L, turnLeft);
		commandDict.Add(Command.TURN_R, turnRight);
		commandDict.Add(Command.WAIT, wait);
	}

	public void SetCommandTileCollision(bool isOn){
//		Debug.Log("ProgramUI is turning collision " +isOn);

		foreach(FunctionZone funcZone in functionZones){
			funcZone.SetTileCollision(isOn);
		}
	}

	public void ReadyButton(){
		PlayerManager.Instance.localPlayer.SetReady (!PlayerManager.Instance.localPlayer.isReady);
		readyButton.image.color = (PlayerManager.Instance.localPlayer.isReady ? Color.white : Color.green);
	}


	public void SetButtonText(string text){
		readyButton.GetComponentInChildren<Text>().text = text;
	}
}
