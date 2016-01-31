using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class FunctionBlueprint : System.Object
{
	public string funcName;
	public bool isGlobal;
	public int numSlots;
	public Color color;
}

[System.Serializable]
public class AntFunction {
	public int numSlots;
	public bool isGlobal;

	public CommandTile[] commandTiles;

	public List<Command> commands;
	public List<int> arguments;

	public void LoadBlueprint(FunctionBlueprint blueprint){
		isGlobal = blueprint.isGlobal;
		numSlots = blueprint.numSlots;
		commandTiles = new CommandTile[numSlots];
		commands = new List<Command> ();
		arguments = new List<int>();

	}

	//Local ExecutionManager will need CommandTile references to light them up while executing.
	public void SetCommandTiles(CommandTile[] coms){
		commandTiles = coms;
		for(int i = 0; i<coms.Length; i++){
			commands[i] = coms[i].command;
		}
	}

}
