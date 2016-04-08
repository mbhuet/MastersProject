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
	public Ant[] ants;
	public FunctionBlueprint[] availableFunctions;
}

public class ProgramManager: NetworkBehaviour
{
	public AntType antType;
	public AntFunction[] functions;
	Ant[] myAnts;
	ProgramUI programUI;
	Stack<Vector3> functionStack;//will hold this programs place when it moves to another function
	public bool finishedExecution = false;

	public void LoadBlueprint (ProgramBlueprint blueprint)
	{

		antType = blueprint.antType;
		myAnts = blueprint.ants;

		foreach(Ant ant in myAnts){
			ant.SpawnPopup("YOU");
		}

		functions = new AntFunction[blueprint.availableFunctions.Length];
		for (int i = 0; i< blueprint.availableFunctions.Length; i++) {
			AntFunction func = new AntFunction ();
			func.LoadBlueprint (blueprint.availableFunctions [i]);
			functions [i] = func;
		}
	}

	void Awake ()
	{
		if (isLocalPlayer) {
			programUI = ProgramUI.Instance;
		}
		functionStack = new Stack<Vector3> ();
	}

	public Ant[] GetAnts ()
	{
		//myAnts = GameManager.Instance.getAntsOfType (antType);
		//myAnts = GameManager.Instance.getAntsForPlayerNum (GetComponent<Player>().playerNum);

		return myAnts;
	}

	/*
	public void ExecuteCommand (Command com)
	{
		foreach (Ant ant in myAnts) {
			if (ant.heldInPlace && 
			    	(com == Command.FORWARD || 
			 		 com == Command.BACKWARD ||
			 		 com == Command.PUSH)) {
				ant.ExecuteCommand (Command.WAIT);
				ant.heldInPlace = false;
			} else {
				//Debug.Log ("Execute Command " + com + " in ant " + ant);
				ant.ExecuteCommand (com);

			}
		}
	}
	*/

	public Vector3 ResolveCalls (Vector3 currentCoords)
	{
//		Debug.Log ("ResolveCalls(" + currentCoords + ")");
		if(this == PlayerManager.Instance.localPlayer.programManager && currentCoords.x == PlayerManager.Instance.localPlayer.playerNum){
			if (GetCommand(currentCoords) != Command.NONE){
			ProgramUI.Instance.AddPlayHeadTarget(this.functions[(int)currentCoords.y].commandTiles[(int)currentCoords.z].slot);
			}
		}
		Command com = GetCommand (currentCoords);
		switch (com) {
		case Command.FUNCTION:
			functionStack.Push (currentCoords);// will push the coordinates of the function tile
			int arg = GetArgument ((int)currentCoords.y, (int)currentCoords.z);
			if(arg == -1) Debug.Log("");
			int playerIndex = arg / 10;
			int funcIndex = arg % 10;
			currentCoords.x = playerIndex;
			currentCoords.y = funcIndex;
			currentCoords.z = 0;
			//Debug.Log ("Function leading to " + currentCoords);

			currentCoords = ResolveCalls (currentCoords);
			break;
		case Command.NONE:
			if (functionStack.Count > 0) {
				currentCoords = functionStack.Pop () + Vector3.forward;
				currentCoords = ResolveCalls (currentCoords);
			}
			break;
		default:
			break;
		}


		return currentCoords;
		
	}

	public void RetrieveCommandTilesFromUI ()
	{
		//Debug.Log ("RetrieveCommandTilesFromUI");
		for (int i  = 0; i < functions.Length; i++) {
//			Debug.Log(ProgramUI.Instance);

			functions [i].SetCommandTiles (ProgramUI.Instance.GetCommandTilesFromFunctionZone (i));
		}
	}

	public Command GetCommand (int funcIndex, int comIndex)
	{
		//Debug.Log ();
		if (funcIndex >= functions.Length) {
//			Debug.Log ("GetCommand func out of bounds");
//			Debug.Log ("Looking for function at func " + funcIndex + "/" + functions.Length);
			return Command.NONE;
		} else if (comIndex >= functions [funcIndex].commands.Count) {
//			Debug.Log ("GetCommand com out of bounds");
//			Debug.Log ("Looking for command at com" + comIndex + "/" + functions [funcIndex].commands.Count);
			return Command.NONE;

		}
		return functions [funcIndex].commands [comIndex];
	}

	public int GetArgument (int funcIndex, int argIndex)
	{
		if (funcIndex >= functions.Length) {
//			Debug.Log ("GetArgument out of bounds");
//			Debug.Log ("Looking for function at " + funcIndex + "/" + functions.Length);
			return -1;
		} else if (argIndex >= functions [funcIndex].arguments.Count) {
//			Debug.Log ("GetArgument out of bounds");
//			Debug.Log ("Looking for arg at " + argIndex + "/" + functions [funcIndex].arguments.Count);
			return -1;
		}
		return functions [funcIndex].arguments [argIndex];
	}

	public static Command GetCommand (Vector3 coords)
	{
		int playerIndex = (int)coords.x;
		int funcIndex = (int)coords.y;
		int comIndex = (int)coords.z;
		return PlayerManager.Instance.players [playerIndex].programManager.GetCommand (funcIndex, comIndex);
	}

	public static int GetArgument (Vector3 coords)
	{
		int playerIndex = (int)coords.x;
		int funcIndex = (int)coords.y;
		int argIndex = (int)coords.z;
		return PlayerManager.Instance.players [playerIndex].programManager.GetArgument (funcIndex, argIndex);
	}

	public void AddCommand (int funcIndex, Command com, int comIndex, int arg)
	{
//		Debug.Log ("AddCommand local ");
//		Debug.Log (functions [funcIndex].commands);
		RetrieveCommandTilesFromUI ();
		if (isServer) {
			RpcAddCommand (funcIndex, com, comIndex, arg);
		} else {
			CmdAddCommand (funcIndex, com, comIndex, arg);
		}
	}

	public void RemoveCommand (int funcIndex, int comIndex)
	{
//		Debug.Log ("RemoveCommand local");
		RetrieveCommandTilesFromUI ();

		if (isServer) {
			RpcRemoveCommand (funcIndex, comIndex);
		} else {
			CmdRemoveCommand (funcIndex, comIndex);
		}

	}

	[Command]
	void CmdAddCommand (int funcIndex, Command com, int comIndex, int arg)
	{
//		Debug.Log ("AddCommand Cmd");
		RpcAddCommand (funcIndex, com, comIndex, arg);
	}

	[Command]
	void CmdRemoveCommand (int funcIndex, int comIndex)
	{
//		Debug.Log ("RemoveCommand Cmd");
		RpcRemoveCommand (funcIndex, comIndex);
	}

	[ClientRpc]
	void RpcAddCommand (int funcIndex, Command com, int comIndex, int arg)
	{
//		Debug.Log ("AddCommand Rpc");
		functions [funcIndex].commands.Insert (comIndex, com);
		functions [funcIndex].arguments.Insert (comIndex, arg);

	}

	[ClientRpc]
	void RpcRemoveCommand (int funcIndex, int comIndex)
	{
//		Debug.Log ("RemoveCommand Rpc");
		functions [funcIndex].commands.RemoveAt (comIndex);
		functions [funcIndex].arguments.RemoveAt (comIndex);

	}
}
