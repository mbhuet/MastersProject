using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FunctionBlueprint : System.Object
{
	public string funcName;
	public bool isGlobal;
	public int numSlots;
	public Color color;
}

public class AntFunction {
	public int numSlots;
	public bool isGlobal;

	public CommandTile[] commandTiles;

	public void LoadBlueprint(FunctionBlueprint blueprint){
		isGlobal = blueprint.isGlobal;
		numSlots = blueprint.numSlots;
		commandTiles = new CommandTile[numSlots];
	}

	public void SetCommandTiles(CommandTile[] coms){
		commandTiles = coms;
	}
}
