using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SoldierAnt : Insect
{	
	// Public variables
	public float attackPower = 5.0f;
	public float shieldEnergy = 100.0f;

	public float fieldOfView = 90f;
	public float longViewDistance = 20f; 
	public float closeViewDistance = 5f;

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
	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);

		//Enemy?
		if (collision.gameObject.tag.Equals("Enemy")) {
			collision.gameObject.GetComponent<EnemyReactive>().UpdateEnergy(-attackPower); //Attack
			collided = true;
		}
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Concat Food and Enemies for checking
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy").Concat(GameObject.FindGameObjectsWithTag("Ant")).ToArray();
		Dictionary<string, List<GameObject>> result;
		
		result = Utils.CheckFieldOfView(this.gameObject, objs, fieldOfView, longViewDistance, closeViewDistance);
		
		return result;
	}
}