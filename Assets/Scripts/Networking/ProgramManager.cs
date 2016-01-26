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
		if (isLocalPlayer) {
			programUI = GameObject.FindObjectOfType<ProgramUI> ();
		}
	}


	public void GetFunctions(){
		for(int i  = 0; i < functions.Length; i++){
		functions[i].SetCommandTiles(programUI.GetCommandTilesFromFunctionZone(i));
		}
	}

	/*
	void SubmitFunctionsToServer(){
		for (int i = 0; i<functions.Length; i++) {
			CmdUpdateFunction (functions [i].SerializeCommands (), i);
		}

	}
		*/

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


	[ClientRpc]
	public void RpcTest(int val){
		testVal = val;
	}
}
