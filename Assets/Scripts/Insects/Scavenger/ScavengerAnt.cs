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
		if (collision.gameObject.tag.Equals("Food")) {
			Load(collision);
			collided = true;
		}

		//QueenWall?
		if (collision.gameObject.name.Equals("queen_wall")) {
			Unload();
			collided = true;
		}

		//Enemy?
		if(collision.gameObject.tag.Equals("Enemy")) {
			GameObject enemy = collision.gameObject;
			UpdateEnergy(-(enemy.GetComponent<Enemy>().attackPower)); //Lose health
		}
		//SameWall?
		if (collision.gameObject.tag.Equals("Wall")) {
			collided = true;
		}
	}

	protected virtual void OnTriggerEnter(Collider collider) 
	{
		//LabyrinthDoor?
		if (collider.gameObject.tag.Equals("Labyrinth")) {
			
			//LabyrinthExit?
			if(collider.gameObject.name.Equals("labyrinth_exit")) {
				insideLabyrinth = false;
	
			} //LabyrinthEntrance?
			else { 
				insideLabyrinth = true;
			}
		}
	}
	
	// Reactors
	protected override void Move() {

		base.Move();

		if (carryingFood) {
			food.transform.position = transform.position + new Vector3(0.0f, 1.5f, 0.0f);
		}
	}
	
	protected virtual void Load(Collision collision) 
	{
		if (!carryingFood) {

			food = collision.gameObject;
			foodRigidBody = food.GetComponent<Rigidbody>();

			food.GetComponent<Food>().Transport = true;
			UpdateEnergy(food.GetComponent<Food>().ConsumeEnergy(0.2f)); // Gather 20% of the food energy

			foodRigidBody.useGravity = false;
			foodRigidBody.freezeRotation = true;

			carryingFood = true;
		}
	}
	
	protected virtual void Unload() 
	{
		//Throw food
		if (carryingFood) {

			foodRigidBody.useGravity = true;
			foodRigidBody.freezeRotation = false;

			food.GetComponent<Food>().Transport = false;
			foodRigidBody.AddForce(transform.forward * 400.0f);
	
			food = null;
			foodRigidBody = null;

			carryingFood = false;
		}
	}

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Get active food
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Food").Concat
						   (GameObject.FindGameObjectsWithTag("Ant")).Concat
						   (GameObject.FindGameObjectsWithTag("Enemy")).Concat
				 		   (GameObject.FindGameObjectsWithTag("Labyrinth")).ToArray();

		return Utils.CheckFieldOfView(gameObject, objs, fieldOfView, longViewDistance);
	}
}