using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static CommandTile tileBeingDragged;
	
	#region IBeginDragHandler implementation
	public abstract void OnBeginDrag (PointerEventData eventData);
	#endregion
	
	#region IDragHandler implementation
	
	public abstract void OnDrag (PointerEventData eventData);
	
	#endregion
	
	#region IEndDragHandler implementation
	
	public abstract void OnEndDrag (PointerEventData eventData);
	
	#endregion

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();
		
		if (comp != null)
			return comp;
		
		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}
	
}
