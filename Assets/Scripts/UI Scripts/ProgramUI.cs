using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProgramUI : MonoBehaviour
{
	public static ProgramUI Instance;
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
	PlayHead playHead;
	public GameObject functionsPanel;
	public static int tileSize;
	ProgramManager localProgramManager;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
		canvas = this.GetComponent<Canvas> ();
		BuildCommandDictionary ();
		functionZones = new List<FunctionZone> ();
		FindFunctionZones ();
		tileSize = Screen.height / 10;
		playHead = transform.GetComponentInChildren<PlayHead> ();
	}

	void Start ()
	{
	}

	void FindFunctionZones ()
	{
		foreach (FunctionZone funcZone in gameObject.GetComponentsInChildren<FunctionZone>()) {
			functionZones.Add (funcZone);
		}
	}

	public void SetLocalProgramManager (ProgramManager prog)
	{
		localProgramManager = prog;
	}

	public void CloseAllGaps(){
				foreach (FunctionZone funcZone in functionZones) {
						funcZone.CloseGaps ();
					}
			}

	public CommandTile[] GetCommandTilesFromFunctionZone (int index)
	{
		if (index >= functionZones.Count)
			return null;
		return functionZones [index].GetCommandTiles ();
	}

	public void AddPlayHeadTarget (CommandSlot slot)
	{
		playHead.AddSlot (slot);
	}

	public void ResetPlayHead ()
	{
		playHead.Reset ();
	}

	public void BuildUIFromBlueprint (ProgramBlueprint blueprint)
	{
		int numFunctions = blueprint.availableFunctions.Length;

		commandDock = GameObject.Instantiate (commandDockPrefab);
		commandDock.transform.SetParent (canvas.transform, false);
		commandDock.GetComponent<GridLayoutGroup> ().cellSize = Vector2.one * tileSize;


		//ADD BASIC ANT COMMANDS
		foreach (Command com in antCommands) {
			AddTileBankToCommandDock (com, commandDock, 0);
		}

		//ADD LOCAL FUNCTIONS
		for (int i = 1; i<localProgramManager.functions.Length; i++) {
			//if(!localProgramManager.functions[i].isGlobal){
			int arg = PlayerManager.Instance.localPlayer.playerNum * 10 + i;
			AddTileBankToCommandDock (Command.FUNCTION, commandDock, arg);
			//}
		}


		//ADD GLOBAL FUNCTIONS
		for (int i = 0; i<GameManager.Instance.programProfiles.Length; i++) {
			if (i != localProgramManager.GetComponent<Player> ().playerNum) {
				for (int j =0; j< GameManager.Instance.programProfiles[i].availableFunctions.Length; j++) {
					if (GameManager.Instance.programProfiles [i].availableFunctions [j].isGlobal) {
						int arg = i * 10 + j;
						AddTileBankToCommandDock (Command.FUNCTION, commandDock, arg);
					}
				}
			}
		}




		//ADD CLASS COMMANDS
		switch (localProgramManager.antType) {
		case AntType.FIRE:
			foreach (Command com in fireCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			break;
		case AntType.CARPENTER:
			foreach (Command com in carpenterCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			break;
		case AntType.WARRIOR:
			foreach (Command com in warriorCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			break;
		case AntType.SCOUT:
			foreach (Command com in scoutCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			break;
		case AntType.DEFAULT:
			break;

		case AntType.ALL:
			foreach (Command com in fireCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			foreach (Command com in carpenterCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			foreach (Command com in warriorCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			foreach (Command com in scoutCommands) {
				AddTileBankToCommandDock (com, commandDock, 0);
			}
			break;
		default:
			break;
		}


		for (int i = 0; i< numFunctions; i++) {
			Text funcTitle = GameObject.Instantiate (textPrefab) as Text;
			funcTitle.text = blueprint.availableFunctions [i].funcName;
			funcTitle.transform.SetParent (functionsPanel.transform, false);
			GameObject func = GameObject.Instantiate (functionZonePrefab);
			functionZones.Add (func.GetComponent<FunctionZone> ());
			func.transform.SetParent (functionsPanel.transform, false);
			func.GetComponent<FunctionZone> ().Init (localProgramManager, blueprint.availableFunctions [i].numSlots, i);
			func.GetComponent<GridLayoutGroup> ().cellSize = Vector2.one * tileSize;
		}

		Rect playRect = playHead.GetComponent<RectTransform> ().rect;
		playRect.width = tileSize;
		playRect.height = tileSize;
		playHead.GetComponent<RectTransform> ().sizeDelta = Vector2.one * tileSize;
		playHead.AddSlot (functionZones [0].GetSlot (0));
		playHead.SetHome (functionZones [0].GetSlot (0));
	
	}

	void AddTileBankToCommandDock (Command com, GameObject dock, int arg)
	{
		TileBank bank = GameObject.Instantiate (tileBankPrefab);
		CommandTile tile;
		commandDict.TryGetValue (com, out tile);
		bank.Init (tile, arg);
		bank.transform.SetParent (dock.transform);
		bank.GetComponent<GridLayoutGroup> ().cellSize = Vector2.one * tileSize;

	}

	void BuildCommandDictionary ()
	{
		commandDict = new Dictionary<Command, CommandTile> ();
		commandDict.Add (Command.FORWARD, forward);
		commandDict.Add (Command.BACKWARD, backward);
		commandDict.Add (Command.TURN_L, turnLeft);
		commandDict.Add (Command.TURN_R, turnRight);
		commandDict.Add (Command.WAIT, wait);
		commandDict.Add (Command.FIRE, fire);
		commandDict.Add (Command.BUILD, build);
		commandDict.Add (Command.PUSH, push);
		commandDict.Add (Command.FUNCTION, funcTile);
	}

	public void SetCommandTileCollision (bool isOn)
	{
//		Debug.Log("ProgramUI is turning collision " +isOn);

		foreach (FunctionZone funcZone in functionZones) {
			funcZone.SetTileCollision (isOn);
		}
	}

	public void DisableInteractivity ()
	{
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}

	public void EnableInteractivity ()
	{
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}
}
