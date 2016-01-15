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
	public GameObject placeholder = null;
	public Transform startParent = null;
	public Transform placeholderParent = null;
	bool fromBank = true;

	#region IBeginDragHandler implementation
	public override void OnBeginDrag (PointerEventData eventData)
	{
		Canvas canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;
		startParent = this.transform.parent;
		placeholderParent = startParent;

		placeholder = new GameObject ();
		LayoutElement le = placeholder.AddComponent<LayoutElement> ();
		le.preferredWidth = this.GetComponent<LayoutElement> ().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement> ().preferredHeight;
		le.flexibleWidth = 0;
		le.flexibleHeight = 0;

		if (fromBank) {
			GameObject freshTile = GameObject.Instantiate (this.gameObject);
			freshTile.transform.SetParent (this.transform.parent);
			freshTile.name = this.name;
		} else {
			placeholder.transform.SetParent (this.transform.parent);
			placeholder.transform.SetSiblingIndex (this.transform.GetSiblingIndex());


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


		//Will move placeholder
		int newSibIndex = placeholderParent.childCount;

		for (int i=0; i<placeholderParent.childCount; i++) {
			if(this.transform.position.x < placeholderParent.GetChild(i).position.x){
				newSibIndex = i;
				if(placeholder.transform.GetSiblingIndex() < newSibIndex){
					newSibIndex--;
				}
				break;
			}
		}
		placeholder.transform.SetSiblingIndex(newSibIndex);

	}
	
	#endregion
	
	#region IEndDragHandler implementation
	
	public override void OnEndDrag (PointerEventData eventData)
	{
		tileBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		this.transform.SetSiblingIndex (placeholder.transform.GetSiblingIndex ());
		Destroy (placeholder);
		Debug.Log (transform.parent.name);
		if (transform.parent.name == "Canvas") {
			Debug.Log(this.GetComponentInParent<Canvas> ());
			Destroy(this.gameObject);
		}
		fromBank = false;
	}
	
	#endregion
}
