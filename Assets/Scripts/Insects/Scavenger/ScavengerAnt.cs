using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAnt : Insect 
{	
	// Food
	protected GameObject food = null;
	protected Rigidbody foodRigidBody = null;
	
	public float fieldOfView = 90f;
	public float longViewDistance = 25f; 
	public float closeViewDistance = 5f;

	protected bool run = false;
	protected bool carryingFood = false;

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
		//CarryFood?
		if (collision.gameObject.tag.Equals("Food") && !carryingFood) {
			Load(collision);
		}

		//QueenWall?
		if (collision.gameObject.name.Equals("queen_wall") && carryingFood) {
			Unload();
			collided = true;
		}

		base.OnCollisionEnter(collision);
	}
	
	// Reactors
	protected virtual void Load(Collision collision) 
	{
		food = collision.gameObject;
		foodRigidBody = food.GetComponent<Rigidbody>();

		UpdateEnergy(food.GetComponent<Food>().ConsumeEnergy(0.2f)); // Gather 20% of the food energy
		food.GetComponent<Food>().Transport = true;

		foodRigidBody.useGravity = false;
		foodRigidBody.freezeRotation = true;

		food.transform.position = transform.position + new Vector3(0.0f, 1.5f, 0.0f);

		carryingFood = true;
	}
	
	protected virtual void Unload() 
	{
		//Throw food
		foodRigidBody.useGravity = true;
		foodRigidBody.freezeRotation = false;

		foodRigidBody.AddForce(transform.forward * 400.0f);
		food.GetComponent<Food>().Transport = false;

		food = null;
		foodRigidBody = null;

		carryingFood = false;
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Get active food
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Food").Concat
						   (GameObject.FindGameObjectsWithTag("Ant")).Concat
						   (GameObject.FindGameObjectsWithTag("Enemy")).Concat
				 		   (GameObject.FindGameObjectsWithTag("Labyrinth")).ToArray();

		return Utils.CheckFieldOfView(gameObject, objs, fieldOfView, longViewDistance, closeViewDistance);
	}
}