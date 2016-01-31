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
	public CommandTile fire;
	public CommandTile push;
	public CommandTile build;
	public CommandTile funcTile;

	Dictionary<Command, CommandTile> commandDict;
	List<FunctionZone> functionZones;

	Command[] antCommands = {
		Command.FORWARD, 
		Command.BACKWARD, 
		Command.TURN_L, 
		Command.TURN_R, 
		Command.WAIT
	};

	Command[] fireCommands = {Command.FIRE};
	Command[] warriorCommands = {Command.PUSH};
	Command[] scoutCommands = {};
	Command[] carpenterCommands = {Command.BUILD};

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


		//ADD BASIC ANT COMMANDS
		foreach(Command com in antCommands){
			AddTileBankToCommandDock(com, commandDock, 0);
		}

		//ADD LOCAL FUNCTIONS
		for(int i = 1; i<localProgramManager.functions.Length; i++){
			if(!localProgramManager.functions[i].isGlobal){
			int arg = PlayerManager.Instance.localPlayer.playerNum * 10 + i;
			AddTileBankToCommandDock(Command.FUNCTION, commandDock, arg);
			}
		}


		//ADD GLOBAL FUNCTIONS
		for (int i = 0; i<GameManager.Instance.programProfiles.Length; i++) {
			for(int j =0; j< GameManager.Instance.programProfiles[i].availableFunctions.Length; j++){
				if(GameManager.Instance.programProfiles[i].availableFunctions[j].isGlobal){
					int arg = i * 10 + j;
					AddTileBankToCommandDock(Command.FUNCTION, commandDock, arg);
				}
			}
		}




		//ADD CLASS COMMANDS
		switch(localProgramManager.antType){
		case AntType.FIRE:
			foreach(Command com in fireCommands){
				AddTileBankToCommandDock(com, commandDock, 0);
			}
			break;
		case AntType.CARPENTER:
			foreach(Command com in carpenterCommands){
				AddTileBankToCommandDock(com, commandDock, 0);
			}
			break;
		case AntType.WARRIOR:
			foreach(Command com in warriorCommands){
				AddTileBankToCommandDock(com, commandDock, 0);
			}
			break;
		case AntType.SCOUT:
			foreach(Command com in scoutCommands){
				AddTileBankToCommandDock(com, commandDock, 0);
			}
			break;
		default:
			break;
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

	void AddTileBankToCommandDock(Command com, GameObject dock, int arg){
		TileBank bank = GameObject.Instantiate(tileBankPrefab);
		CommandTile tile;
		commandDict.TryGetValue(com, out tile);
		bank.Init(tile, arg);
		bank.transform.SetParent(dock.transform);
	}

	void BuildCommandDictionary(){
		commandDict = new Dictionary<Command, CommandTile>();
		commandDict.Add(Command.FORWARD, forward);
		commandDict.Add(Command.BACKWARD, backward);
		commandDict.Add(Command.TURN_L, turnLeft);
		commandDict.Add(Command.TURN_R, turnRight);
		commandDict.Add(Command.WAIT, wait);
		commandDict.Add(Command.FIRE, fire);
		commandDict.Add(Command.BUILD, build);
		commandDict.Add(Command.PUSH, push);
		commandDict.Add (Command.FUNCTION, funcTile);
	}

	public void SetCommandTileCollision(bool isOn){
//		Debug.Log("ProgramUI is turning collision " +isOn);

		foreach(FunctionZone funcZone in functionZones){
			funcZone.SetTileCollision(isOn);
		}
	}

	public void ReadyButton(){
		PlayerManager.Instance.localPlayer.SetReady (!PlayerManager.Instance.localPlayer.isReady);
		readyButton.image.color = (PlayerManager.Instance.localPlayer.isReady ? Color.green : Color.white);
	}


	public void SetButtonText(string text){
		readyButton.GetComponentInChildren<Text>().text = text;
	}
}
