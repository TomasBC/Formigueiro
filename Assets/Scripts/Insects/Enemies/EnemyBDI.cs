using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemyBDI : Insect {
	private List<Belief> beliefs;
	public float attackPower = 5.0f;

	public float fieldOfView = 90f;
	public float longViewDistance = 20f; 
	public float closeViewDistance = 5f;

	// Initialization
	protected override void Start() 
	{
		base.Start();
	//	beliefs = new List<string> ();
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
			collision.gameObject.GetComponent<Insect>().UpdateEnergy(-attackPower); //Attack
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

	protected override Dictionary<string, List<GameObject>> CheckFieldOfView() 
	{
		//Concat Food and Enemies for checking
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Food").Concat(GameObject.FindGameObjectsWithTag("Ant")).Concat(GameObject.FindGameObjectsWithTag("Enemy")).ToArray();
		Dictionary<string, List<GameObject>> result;
		
		result = ViewConeController.CheckFieldOfView(this.gameObject, objs, fieldOfView, longViewDistance, closeViewDistance);
		
		return result;
	}

	/*protected override void EvaluateFieldOfView()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		List<GameObject> listAux;

		foreach (GameObject f in objsInsideCone ["Ant"]) {
			be
		}
		Belief b = new Belief (objsInsideCone ["Ant"], objsInsideCone ["Enemy"], objsInsideCone ["Food"]);
		//If we find any sort of food and we are not carrying any, we rotate towards the object
		if(objsInsideCone.TryGetValue("Food", out listAux)) {
			RotateTowards(listAux[Random.Range(0, listAux.Count)]); //Randomly pick a food object
			proceed = true;
		}
	}*/

}
