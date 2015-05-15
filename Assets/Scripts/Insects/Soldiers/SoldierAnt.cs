using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SoldierAnt : Insect
{	
	// Public variables
	public float attackPower = 2f;
	public float shieldEnergy = 100.0f;

	public float fieldOfView = 90f;
	public float longViewDistance = 20f; 

	// Initialization
	protected override void Start()
	{
		base.Start();
	}
	
	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
	}

	// Sensors
	protected void OnCollisionStay(Collision collision)
	{
		//Enemy?
		if (collision.gameObject.tag.Equals("Enemy")) {
			UpdateEnergy(-(collision.gameObject.GetComponent<Enemy>().attackPower)); //Lose health
		}
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Concat Food and Enemies for checking
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy").Concat(GameObject.FindGameObjectsWithTag("Ant")).ToArray();
		return Utils.CheckFieldOfView(this.gameObject, objs, fieldOfView, longViewDistance);
	}
}