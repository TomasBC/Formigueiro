using UnityEngine;
using System.Collections;

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
		Move();
	}

	// Reactors
	protected override void Move() 
	{
		if (!collided) {
			base.Move(); //Move forward
			Rotate(randomMax);
		} else {
			Rotate(randomMin);
			collided = false;
		}
	}
}