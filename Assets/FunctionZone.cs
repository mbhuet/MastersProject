using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FunctionZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	string funcName = "myFunc";
	Text funcTitle;
	CommandSlot[] slots;

	void Awake(){
		funcTitle = GetComponent<Text>();
	}

	void Start(){
		FindSlots();
	}

	void FindSlots(){
		slots = gameObject.GetComponentsInChildren<CommandSlot>();
		for(int i = 0; i<slots.Length; i++){
			slots[i].functionZone = this;
			slots[i].slotIndex = i;
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
		Debug.Log("gap requested at " +index);

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
		Debug.Log("close gaps");
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
		Debug.Log("moving tile from slot " + from + " to " + to);

		to.SetTile(from.tile);
		from.RemoveTile();
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
