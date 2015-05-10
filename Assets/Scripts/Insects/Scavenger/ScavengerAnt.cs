﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAnt : Insect 
{	
	// Food
	protected GameObject food = null;
	protected Rigidbody foodRigidBody = null;
	
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
	protected bool CarryingFood() 
	{
		if (food != null) {
			return true;
		} else {
			return false;
		}
	}
		
	protected override void OnCollisionEnter(Collision collision) 
	{
		base.OnCollisionEnter(collision);

		//CarryFood?
		if (collision.gameObject.tag.Equals("Food") && !CarryingFood()) {
			Load(collision);

		}

		//QueenWall?
		if (collision.gameObject.tag.Equals("Unload") && CarryingFood()) {
			Unload();
			collided = true;
		}
	}
	
	// Reactors
	protected override void Move() 
	{
		base.Move(); //Move forward

		//If carrying food move it as well
		if (CarryingFood()) {
			foodRigidBody.transform.position = rigidBody.transform.position + new Vector3(0.0f, 1.5f, 0.0f);
		}
	}

	protected virtual void Load(Collision collision) 
	{
		food = collision.gameObject;
		foodRigidBody = food.GetComponent<Rigidbody>();

		UpdateEnergy(food.GetComponent<Food>().ConsumeEnergy(0.2f)); // Gather 20% of the food energy
		food.GetComponent<Food>().SetTransport(true);

		foodRigidBody.useGravity = false;
		foodRigidBody.freezeRotation = true;

		//Reset rotation
		transform.rotation = Quaternion.identity;
	}
	
	protected virtual void Unload() 
	{
		//Throw food
		foodRigidBody.useGravity = true;
		foodRigidBody.freezeRotation = false;

		foodRigidBody.AddForce(rigidBody.transform.forward * 150.0f);
		food.GetComponent<Food>().SetTransport(false);

		food = null;
		foodRigidBody = null;
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Concat Food and Enemies for checking
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Food").ToArray();
		Dictionary<string, List<GameObject>> result;

		result = ViewConeController.CheckFieldOfView(this.gameObject, objs, fieldOfView, longViewDistance, closeViewDistance);

		return result;
	}
}