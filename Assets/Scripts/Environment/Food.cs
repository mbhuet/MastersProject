using UnityEngine;
using System.Collections;

public class Food : Voxel {
	public bool collected = false;
	public GameObject visual;
	ParticleSystem particleSys;

	public delegate void FoodDelegate();
	public static event FoodDelegate OnCollect;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		particleSys = GetComponentInChildren<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected override void VoxelInit(){
		base.VoxelInit();
		GameManager.Instance.RegisterFood (this);
	}

	public override void Reset(){
		base.Reset ();
		collected = false;
		visual.SetActive (true);
	}

	public void Collect(){
		collected = true;
		visual.SetActive (false);
		OnCollect ();
		particleSys.Emit (50);
	}

	void OnTriggerEnter(Collider col){
		Debug.Log ("Food Trigger Enter");
		if (col.GetComponent<Ant> () != null) {
			OnCollect();
			Collect();
		}
	}
}
