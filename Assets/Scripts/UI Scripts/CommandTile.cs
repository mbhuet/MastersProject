using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Command{
	NONE,
	FORWARD,
	BACKWARD,
	WAIT,
	TURN_L,
	TURN_R,
	PUSH,
	FIRE,
	BUILD,
	SENSE,
	FUNCTION,


}

public class CommandTile : Tile {
	public Command command;
	public Transform startParent = null;
	protected bool fromBank = true;
	public CommandSlot slot;
	public int argument;


	public void Init(int arg){
		argument = arg;
		if (command == Command.FUNCTION) {
			Text text = GetComponentInChildren<Text>();
			text.fontSize = ProgramUI.tileSize/2;
			if (arg/10 == PlayerManager.Instance.localPlayer.playerNum){
				text.text = "F" + arg%10;
			}
			else{
				text.text = "F";
			}
		}
	}

	#region IBeginDragHandler implementation
	public override void OnBeginDrag (PointerEventData eventData)
	{
		Debug.Log("tile OnBeginDrag---------------------------------------------------------");
		Canvas canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;
		tileBeingDragged = this;

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
		GetComponent<CanvasGroup> ().blocksRaycasts = false;



	}
	#endregion
	
	#region IDragHandler implementation
	
	public override void OnDrag (PointerEventData eventData)
	{
		tileBeingDragged.transform.position = Input.mousePosition;// + Vector3.up * ProgramUI.tileSize;

	}
	
	#endregion
	
	#region IEndDragHandler implementation
	
	public override void OnEndDrag (PointerEventData eventData)
	{
		tileBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		if (transform.parent.GetComponent<Canvas>() != null) {
			Debug.Log("Parented to " + this.transform.parent.name);
			Destroy(this.gameObject);
		}
		fromBank = false;

		Canvas canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;
		canvas.GetComponent<ProgramUI>().SetCommandTileCollision(true);
		ProgramUI.Instance.CloseAllGaps ();

	}
	
	#endregion
}
