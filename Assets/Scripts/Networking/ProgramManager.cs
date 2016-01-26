using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class ProgramBlueprint : System.Object
{
	public AntType antType;
	public FunctionBlueprint[] availableFunctions;
}

public class ProgramManager: NetworkBehaviour {
	AntType antType;
	AntFunction[] functions;

	Ant[] myAnts;
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
		if (isLocalPlayer) {
			programUI = GameObject.FindObjectOfType<ProgramUI> ();
		}
	}

	public void ExecuteCommand(Command com){
		Debug.Log("Execute Command " + com);
		foreach(Ant ant in myAnts){
			Debug.Log("Executing in ant " + ant);

			ant.ExecuteCommand(com, 1);
		}
	}

	public Vector2 GetFollowingCommandCoordinates(Vector2 current){
		return current + Vector2.up;
		//TODO return actual next command by looking at the current one
	}


	public void GetFunctions(){
		for(int i  = 0; i < functions.Length; i++){
		functions[i].SetCommandTiles(programUI.GetCommandTilesFromFunctionZone(i));
		}
	}

	public Command GetCommand(int funcIndex, int comIndex){
		if (funcIndex >= functions.Length || comIndex >= functions[funcIndex].commands.Count){
			Debug.Log("GetCommand out of bounds");
			Debug.Log(funcIndex + "/" + functions.Length);
			Debug.Log(comIndex + "/" + functions[funcIndex].commands.Count);
			return Command.NONE;
		}
		return functions[funcIndex].commands[comIndex];
	}
	public Command GetCommand(Vector2 coords){
		int funcIndex = (int)coords.x;
		int comIndex = (int)coords.y;

		return GetCommand(funcIndex, comIndex);
	}

	public void AddCommand(int funcIndex, Command com, int comIndex){
		Debug.Log ("AddCommand local ");
		Debug.Log (functions[funcIndex].commands);
		if (isServer) {
			RpcAddCommand (funcIndex, com, comIndex);
		} else {
			CmdAddCommand(funcIndex, com, comIndex);
		}
	}

	public void RemoveCommand(int funcIndex, int comIndex){
		Debug.Log ("RemoveCommand local");
		if (isServer) {
			RpcRemoveCommand (funcIndex, comIndex);
		} else {
			CmdRemoveCommand(funcIndex, comIndex);
		}

	}

	[Command]
	void CmdAddCommand(int funcIndex, Command com, int comIndex){
		Debug.Log ("AddCommand Cmd");
		RpcAddCommand (funcIndex, com, comIndex);
	}

	[Command]
	void CmdRemoveCommand(int funcIndex, int comIndex){
		Debug.Log ("RemoveCommand Cmd");
		RpcRemoveCommand (funcIndex, comIndex);
	}

	[ClientRpc]
	void RpcAddCommand(int funcIndex, Command com, int comIndex){
		Debug.Log ("AddCommand Rpc");
		functions [funcIndex].commands.Insert(comIndex, com);
	}

	[ClientRpc]
	void RpcRemoveCommand(int funcIndex, int comIndex){
		Debug.Log ("RemoveCommand Rpc");
		functions [funcIndex].commands.RemoveAt(comIndex);
	}
}
