using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
	public CommandTile tile{
		get{
			if (transform.childCount > 0){
				return transform.GetChild(0).gameObject.GetComponent<CommandTile>();
			}
			return null;
		}
	}


	#region IDropHandler implementation
	public virtual void OnDrop (PointerEventData eventData)
	{
		Debug.Log ("OnDrop");
		if (!tile) {
			Tile.tileBeingDragged.transform.SetParent(transform);
		}
		if (Tile.tileBeingDragged == tile) {
			Tile.tileBeingDragged.transform.SetParent(transform);
		}
	}
	#endregion
}
