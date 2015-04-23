﻿using UnityEngine;
using System.Collections;

public class EnemyReactive : Insect {

	public float attackPower = 5.0f;

	// Initialization
	protected override void Start() 
	{
		base.Start();
	}
	
	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		Move();
	}

	// Sensors
	protected override void OnCollisionEnter(Collision collision) {

		base.OnCollisionEnter(collision);

		//Ant?
		if (collision.gameObject.name.Contains("ant")) {
			collision.gameObject.GetComponent<Insect> ().UpdateEnergy(-attackPower); //Attack
			collided = true;
		}
		//Food?
		if (collision.gameObject.name.Contains("food")) {
			collided = true;
		}
	}
	
	// Reactors
	protected override void Move() 
	{
		if (!collided) {
			base.Move(); // Move forward
			Rotate(randomMax);
		} else {
			Rotate(randomMin);
			collided = false;
		}
	}
}
