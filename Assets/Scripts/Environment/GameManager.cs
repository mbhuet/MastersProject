using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public int numPlayers;

	public ProgramBlueprint[] programProfiles = new ProgramBlueprint[4];
	List<Ant> allAnts;
	List<Food> allFood;

	public delegate void GameAction();
	public static event GameAction OnLevelComplete;
	

	void Awake(){
//		Debug.Log("Game Manager Awake at " + Time.time);
		Instance = this;
		allAnts = new List<Ant>();
		allFood = new List<Food> ();
	}

	// Use this for initialization
	void Start () {
		Food.OnCollect += CheckForCompletion;
		OnLevelComplete += LevelCompleted ;
		PlayerManager.Instance.LoadLevelProfiles ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			GuiLobbyManager.s_Singleton.SendReturnToLobby ();
		}
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

	public void RegisterFood(Food food){
		allFood.Add (food);
	}
	
	public bool allFoodCollected(){
		foreach (Food food in allFood) {
			if (!food.collected)
				return false;
		}
		return true;
	}
	
	void CheckForCompletion(){
		if (allFoodCollected ()) {
			OnLevelComplete();
		}
	}

	void LevelCompleted(){
		Debug.Log("Level Completed");

	}






}
