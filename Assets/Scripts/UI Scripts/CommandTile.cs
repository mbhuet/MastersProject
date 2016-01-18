using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Command{
	FORWARD,
	BACKWARD,
	WAIT,
	TURN_L,
	TURN_R
}

public class CommandTile : Tile {
	public Command command;
	public Transform startParent = null;
	bool fromBank = true;
	public CommandSlot slot;

	#region IBeginDragHandler implementation
	public override void OnBeginDrag (PointerEventData eventData)
	{
//		Debug.Log("tile OnBeginDrag");
		Canvas canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

		if(slot != null){
			Debug.Log("removing picked up tile from slot");
			slot.RemoveTile();
		}

		startParent = this.transform.parent;
		canvas.GetComponent<ProgramUI>().SetCommandTileCollision(false);

		if (fromBank) {
			GameObject freshTile = GameObject.Instantiate (this.gameObject);
			freshTile.transform.SetParent (this.transform.parent, false);
			freshTile.name = this.name;

		} else {



		}

		this.transform.SetParent(canvas.transform);
		tileBeingDragged = this;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;



	}
	#endregion
	
	#region IDragHandler implementation
	
	public override void OnDrag (PointerEventData eventData)
	{
		tileBeingDragged.transform.position = Input.mousePosition;

	}
	
	#endregion
	
	#region IEndDragHandler implementation
	
	public override void OnEndDrag (PointerEventData eventData)
	{
		tileBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		if (transform.parent.name == "Canvas") {
			Debug.Log(this.GetComponentInParent<Canvas> ());
			Destroy(this.gameObject);
		}
		fromBank = false;

		Canvas canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;
		canvas.GetComponent<ProgramUI>().SetCommandTileCollision(true);
	}
	
	#endregion
}
