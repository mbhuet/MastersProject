using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public ProgramBlueprint[] programProfiles = new ProgramBlueprint[4];
	List<Ant> allAnts;

	void Awake(){
		Instance = this;
		allAnts = new List<Ant>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RegisterAnt(Ant ant){
		if(!allAnts.Contains(ant))
			allAnts.Add(ant);
	}

	public Ant[] getAntsOfType(AntType type){
		List<Ant> myAnts = new List<Ant>();
		foreach(Ant ant in allAnts){
			if(ant.type == type){
				myAnts.Add(ant);
			}
		}
		return myAnts.ToArray();
	}




}
