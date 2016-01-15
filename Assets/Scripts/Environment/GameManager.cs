using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	List<Player> players;

	void Awake(){
		Instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddPlayer(Player p){
		players.Add (p);
	}
}
