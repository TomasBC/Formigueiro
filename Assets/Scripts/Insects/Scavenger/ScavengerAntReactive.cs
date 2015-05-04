using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntReactive : ScavengerAnt 
{	
	// Initialization
	protected override void Start() 
	{
		base.Start();
	}

	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvaluateFieldOfView(); //Evaluate view cone
		Move();
	}

	// Reactors
	protected override void Move() 
	{
		if(!collided && !proceed) {
			base.Move(); //Move forward and rotate max
			Rotate(randomMax);
		} else if(!collided && proceed) {
			base.Move(); //Move forward
			proceed = false;
		} else {
			Rotate(randomMin); //Rotate min
			collided = false;
		}
	}

	protected override void EvaluateFieldOfView()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		List<GameObject> listAux;

		//If we find any sort of food and we are not carrying any, we rotate towards the object
		if(objsInsideCone.TryGetValue("Food", out listAux) && !CarryingFood()) {
				
			RotateTowards(listAux[Random.Range(0, listAux.Count)]); //Randomly pick a food object
			proceed = true;
		}
	}
}