using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierAnt : Insect
{	
	// Public variables
	public float attackPower = 5.0f;
	public float shieldEnergy = 100.0f;

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
		if (collision.gameObject.name.Contains("enemy")) {
			collision.gameObject.GetComponent<EnemyReactive>().UpdateEnergy(-attackPower); //Attack
			collided = true;
		}
	}
}