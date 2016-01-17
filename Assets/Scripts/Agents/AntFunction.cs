using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FunctionBlueprint : System.Object
{
	public bool isGlobal;
	public int numSlots;
	public Color color;
}

public class AntFunction {
	public int numSlots;
	public bool isGlobal;

	public Command[] commands;

	public void LoadBlueprint(FunctionBlueprint blueprint){
		isGlobal = blueprint.isGlobal;
		numSlots = blueprint.numSlots;
		commands = new Command[numSlots];
	}
}
