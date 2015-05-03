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
		GameObject[] objsInsideCone = CheckFieldOfView();

		for(int i = 0; i < objsInsideCone.Length; i++) {

			//If we find any sort of food and we are not carrying any, we rotate towards the object
			if(objsInsideCone[i].tag.Equals("Food") && !CarryingFood()) {
				RotateTowards(objsInsideCone[i]);
				proceed = true;
				break;
			}
		}
	}
}