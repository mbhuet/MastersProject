using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CommandSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	public FunctionZone functionZone;
	public CommandTile tile;
	public int slotIndex = 0;


	public void SetTile(CommandTile newTile){
		newTile.transform.SetParent(this.transform, false);
		Debug.Log("set parent to slot");
		tile = newTile;
		tile.slot = this;
	}

	public void RemoveTile(){
//		Debug.Log("tile removed from " + this);

		if(tile.slot ==  this){
			tile.slot = null;
		}
		else{
			Debug.Log("tile's slot is not this, it is " + tile.slot);

		}
		//tile.transform.SetParent(Tile.FindInParents<Canvas>(gameObject).transform);
		tile = null;


	}

	#region IDropHandler implementation
	
	public void OnDrop (PointerEventData eventData)
	{
		if(tile!=null) return;
		SetTile(Tile.tileBeingDragged);
		functionZone.CloseGaps();
		 
	}
	
	#endregion
	
	#region IPointerEnterHandler implementation
	
	public void OnPointerEnter (PointerEventData eventData)
	{
//		Debug.Log("Pointer enter slot " + slotIndex);
		if(Tile.tileBeingDragged != null){
			functionZone.MakeGap(slotIndex);
		}
		/*
		if (eventData.pointerDrag == null)
			return;
		Tile.tileBeingDragged.placeholderParent = this.transform;
		Tile.tileBeingDragged.placeholder.transform.SetParent(Tile.tileBeingDragged.placeholderParent);
		*/

		//TODO All subsequent commandSlots must shift their tiles over to the next one. iF they are all occupied, do not allow a drop
	}
	
	#endregion
	
	#region IPointerExitHandler implementation
	
	public void OnPointerExit (PointerEventData eventData)
	{
		if(Tile.tileBeingDragged != null){

			Debug.Log("pointer exit slot " + slotIndex);

			functionZone.CloseGaps();
		}
		/*
		if (eventData.pointerDrag == null)
			return;
		if (Tile.tileBeingDragged.placeholderParent == this.transform)
			Tile.tileBeingDragged.placeholderParent = Tile.tileBeingDragged.startParent;
		*/
	}
	
	#endregion
	
	
}
