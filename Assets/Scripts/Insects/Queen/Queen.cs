using UnityEngine;
using System.Collections;

public class Queen : Insect {
	
	// Initialization
	protected override void Start()
	{
		base.Start();
	}
	
	//Enemy cycle
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		Move();
	}
	 
	protected override void OnCollisionEnter(Collision collision) 
	{
		base.OnCollisionEnter(collision);

		//Food?
		if (collision.gameObject.tag.Equals("Food")) {
		
			UpdateEnergy(collision.gameObject.GetComponent<Food>().ConsumeEnergy(1)); // Gather all the remaining energy
			Destroy(collision.gameObject);
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