using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayHead : MonoBehaviour {
	List<CommandSlot> slots;
	public float speed;

	// Use this for initialization
	void Start () {
		slots = new List<CommandSlot> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (slots.Count > 0) {
			//Debug.Log (slots[0].GetComponent<RectTransform>().anchoredPosition + "/n" + 
			//           slots[0].GetComponent<RectTransform>().TransformPoint(slots[0].GetComponent<RectTransform>().anchoredPosition));

			this.transform.position = Vector3.Lerp(transform.position, (slots[0].GetComponent<RectTransform>().position), speed * slots.Count);
			if(Vector3.Distance(transform.position, slots[0].GetComponent<RectTransform>().position) < .1f){
				slots.RemoveAt(0);
			}
		}
	}

	public void MoveToSlot(CommandSlot slot){
		//Debug.Log (slot.GetComponent<RectTransform>().rect.center);
		this.transform.position = slot.GetComponent<RectTransform>().position;
	}

	public void AddSlot(CommandSlot slot){
		slots.Add (slot);
	}
}
