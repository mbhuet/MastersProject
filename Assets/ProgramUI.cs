using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgramUI : MonoBehaviour {
	Canvas canvas;
	ProgramManager programManager;
	public GameObject functionZonePrefab;
	public GameObject commandDockPrefab;
	public TileBank tileBankPrefab;

	public CommandTile forward;
	public CommandTile backward;
	public CommandTile turnLeft;
	public CommandTile turnRight;
	public CommandTile wait;

	Dictionary<Command, CommandTile> commandDict;

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

	// Use this for initialization
	void Awake () {
		canvas = this.GetComponent<Canvas>();
		BuildCommandDictionary();
	}
	


	public void BuildUIFromBlueprint(ProgramBlueprint blueprint){
		int numFunctions = blueprint.availableFunctions.Length;

		commandDock = GameObject.Instantiate(commandDockPrefab);
		commandDock.transform.parent = canvas.transform;
		RectTransform comDockRect = commandDock.GetComponent<RectTransform>();

		foreach(Command com in antCommands){
			TileBank bank = GameObject.Instantiate(tileBankPrefab);
			CommandTile tile;
			commandDict.TryGetValue(com, out tile);
			bank.Init(tile);
			bank.transform.parent = commandDock.transform;
		}


		for(int i = 0; i< numFunctions; i++){
			GameObject func = GameObject.Instantiate(functionZonePrefab);
			func.transform.parent = canvas.transform;
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

	public void SetProgramManager(ProgramManager prog){
		programManager = prog;
	}

	void UpdateProgram(){
		
	}
}
