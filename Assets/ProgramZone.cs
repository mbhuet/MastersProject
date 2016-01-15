using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class ProgramZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{

		Tile.tileBeingDragged.transform.SetParent(this.transform);
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
			return;
		Tile.tileBeingDragged.placeholderParent = this.transform;
		Tile.tileBeingDragged.placeholder.transform.SetParent(Tile.tileBeingDragged.placeholderParent);
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
			return;
		if (Tile.tileBeingDragged.placeholderParent == this.transform)
		Tile.tileBeingDragged.placeholderParent = Tile.tileBeingDragged.startParent;

	}

	#endregion
}
