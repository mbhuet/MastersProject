using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class ProgramBlueprint : System.Object
{
	public AntType antType;
	public FunctionBlueprint[] availableFunctions;
}

public class ProgramPacket : System.Object
{

}

public class ProgramManager: NetworkBehaviour {
	AntType antType;
	AntFunction[] functions;
	Ant[] myAnts;
	int testVal = 0;
	ProgramUI programUI;

	public void LoadBlueprint(ProgramBlueprint blueprint){

		antType = blueprint.antType;
		functions = new AntFunction[blueprint.availableFunctions.Length];
		for(int i = 0; i< blueprint.availableFunctions.Length; i++){
			AntFunction func = new AntFunction();
			func.LoadBlueprint(blueprint.availableFunctions[i]);
			functions[i] = func;
		}
		myAnts = GameManager.Instance.getAntsOfType(antType);
	}

	void Awake(){
		programUI = GameObject.FindObjectOfType<ProgramUI>();
	}


	public void GetFunctions(){
		for(int i  = 0; i < functions.Length; i++){
		functions[i].SetCommandTiles(programUI.GetCommandTilesFromFunctionZone(i));
		}
	}

	[Command]
	void CmdUpdateProgram(){
		
	}

	[ClientRpc]
	public void RpcTest(int val){
		testVal = val;
	}
}
