using UnityEngine;
using System.Collections;

public class ScavengerAnt : Insect 
{	
	// Food
	protected GameObject food = null;
	protected Rigidbody foodRigidBody = null;

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
		if (collision.gameObject.gameObject.name.Contains("food") && !CarryingFood()) {
			Load(collision);
		}

		//QueenWall?
		if (collision.gameObject.name.Contains("queen_wall") && CarryingFood()) {
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
			food.transform.position = rigidBody.transform.position + new Vector3(0.0f, 1.5f, 0.0f);
		}
	}

	protected virtual void Load(Collision collision) 
	{
		food = collision.gameObject;
		foodRigidBody = food.GetComponent<Rigidbody>();

		foodRigidBody.useGravity = false;
		foodRigidBody.freezeRotation = true;
	}
	
	protected virtual void Unload() 
	{
		//TODO: Gather energy before unloading the object
		
		//Throw food
		foodRigidBody.useGravity = true;
		foodRigidBody.freezeRotation = false;

		food.GetComponent<Rigidbody>().AddForce(rigidBody.transform.forward * 150.0f);

		food = null;
		foodRigidBody = null;
	}
}