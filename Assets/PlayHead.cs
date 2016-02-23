using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayHead : MonoBehaviour {
	List<CommandSlot> slots;
	public float speed;
	CommandSlot home;

	void Awake () {
		slots = new List<CommandSlot> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (slots.Count > 0) {
			this.transform.position = Vector3.Lerp(transform.position, (slots[0].GetComponent<RectTransform>().position), speed * slots.Count);
			if(Vector3.Distance(transform.position, slots[0].GetComponent<RectTransform>().position) < .1f){
				slots.RemoveAt(0);
			}
		}
	}

	public void SetHome(CommandSlot slot){
		home = slot;
	}

	public void Reset(){
		MoveToSlot (home);
		slots.Clear ();
	}

	public void MoveToSlot(CommandSlot slot){
		this.transform.position = slot.GetComponent<RectTransform>().position;
	}

	public void AddSlot(CommandSlot slot){
		slots.Add (slot);
	}
}
