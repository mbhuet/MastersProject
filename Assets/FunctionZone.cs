using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FunctionZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	string funcName = "myFunc";
	int funcIndex;
	Text funcTitle;
	CommandSlot[] slots;
	List<Command> commands;
	public GameObject commandSlotPrefab;
	ProgramManager localProgramManager;

	void Awake(){
		funcTitle = GetComponent<Text>();
		commands = new List<Command>();
	}

	void Start(){
		FindSlots();
	}

	public void Init(ProgramManager localProgMan, int numSlots, int index){
		localProgramManager = localProgMan;
//		Debug.Log (localProgramManager);
		funcIndex = index;
		slots = new CommandSlot[numSlots];
		for(int i = 0; i<slots.Length; i++){
			GameObject slot = GameObject.Instantiate(commandSlotPrefab);
			slot.transform.SetParent(this.transform);
			slots[i] = slot.GetComponent<CommandSlot>();
			slot.name = "CommandSlot " + i;
			slot.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * ProgramUI.tileSize;
		}
	}

	void FindSlots(){
		slots = gameObject.GetComponentsInChildren<CommandSlot>();
		for(int i = 0; i<slots.Length; i++){
			slots[i].functionZone = this;
			slots[i].slotIndex = i;
		}
	}

	public CommandSlot GetSlot(int i){
		return slots [i];
	}

	public void AddCommand(Command com, int index, int arg){
		if (index > commands.Count) {
			index = commands.Count;
			Debug.Log ("FunctionZone does not have a slot at this index");
		}
			commands.Insert (index, com);
			localProgramManager.AddCommand (funcIndex, com, index, arg);
	}

	public void RemoveCommand(int index){
		if (index > commands.Count - 1) {
			Debug.LogError ("trying to remove command at index " + index + " from list of length " + commands.Count);
		} else {
			commands.RemoveAt (index);
			localProgramManager.RemoveCommand(funcIndex, index);
		}
	}


	//Having the tiles catch raycasts is interfering with slot mouse over events
	public void SetTileCollision(bool isOn){
//		Debug.Log("FunctionZone is turning collision " +isOn);
		foreach (CommandSlot slot in slots){
			if(slot.tile != null){
				slot.tile.GetComponent<CanvasGroup>().blocksRaycasts = isOn;
			}
		}
	}

	public void MakeGap(int index){
//		Debug.Log("gap requested at " +index);

		bool spaceAvailable = false;
		foreach(CommandSlot slot in slots){
			if(slot.tile == null){
				spaceAvailable = true;
			}
		}
		if(!spaceAvailable){
			//There's no room
			return;
		}

		CloseGaps();

		for(int i = slots.Length - 2; i >= index ; i--){
			if(slots[i].tile != null){
				MoveTile(slots[i], slots[i+1]);
			}
			
		}
	}

	public void CloseGaps(){
//		Debug.Log("close gaps");
		for(int i = 0; i< slots.Length; i++){
			if(slots[i].tile == null){
				bool tileFound = false;
				int searchIndex = i+1;
				while(!tileFound && searchIndex < slots.Length){
					if(slots[searchIndex].tile != null){
						tileFound = true;
						MoveTile(slots[searchIndex], slots[i]); 
					
					}
					searchIndex++;
				}
			}
		}
	}

	public void MoveTile(CommandSlot from, CommandSlot to){
//		Debug.Log("moving tile from slot " + from + " to " + to);
		CommandTile movingTile = from.tile;
		from.RemoveTile();
		to.SetTile(movingTile);
	}

	public CommandTile[] GetCommandTiles(){
		List<CommandTile> tiles = new List<CommandTile>();
		for(int i = 0; i<slots.Length; i++){
			if(slots[i].tile != null){
				tiles.Add(slots[i].tile);
			}
		}
		return tiles.ToArray();
	}


	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		//TODO if the command is valid in this function, light up green, else light up red
		/*
		if (eventData.pointerDrag == null)
			return;
		Tile.tileBeingDragged.placeholderParent = this.transform;
		Tile.tileBeingDragged.placeholder.transform.SetParent(Tile.tileBeingDragged.placeholderParent);
		*/

	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		/*
		if (eventData.pointerDrag == null)
			return;
		if (Tile.tileBeingDragged.placeholderParent == this.transform)
		Tile.tileBeingDragged.placeholderParent = Tile.tileBeingDragged.startParent;
		*/
	}

	#endregion


}
