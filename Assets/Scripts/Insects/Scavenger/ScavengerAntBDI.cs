using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAntReactive {
	// Initialization
	private List<Belief> beliefs;
	private List<Desire> desires;
	private Intention intention;

	protected override void Start() 
	{
		base.Start();
		beliefs = new List<Belief> ();
		desires = new List<Desire> ();
	}

	// Reactors
	protected override void Move() 
	{
		if (intention != null) {
			RotateTowards (intention.IntentionObject);
		} else {
			if (!collided && !proceed) {
				base.Move (); //Move forward and rotate max
				Rotate (randomMax);
			} else if (!collided && proceed) {
				base.Move (); //Move forward
				proceed = false;
			} else {
				Rotate (randomMin); //Rotate min
				collided = false;
			}
		}
	}


	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvaluateFieldOfView(); //Evaluate view cone
		evalBeliefs ();
		Move();
	}

	protected override void EvaluateFieldOfView()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView ();
		List<GameObject> listAux;

		//If we find any sort of food and we are not carrying any, we rotate towards the object

		if (objsInsideCone ["Food"].Count != 0) {
			if (CarryingFood ())
				foreach (GameObject obj in objsInsideCone ["Food"])
					beliefs.Add (new Belief (obj, BeliefsTypes.CatchFood, objsInsideCone ["Enemy"].Count, objsInsideCone ["Ant"].Count));
			else {
				foreach (GameObject obj in objsInsideCone ["Food"])
					beliefs.Add (new Belief (obj, BeliefsTypes.CatchFood, objsInsideCone ["Enemy"].Count, objsInsideCone ["Ant"].Count));
			}
			/*if (objsInsideCone ["Enemy"].Count != 0) {
			foreach(GameObject obj in objsInsideCone ["Enemy"]){
				belief.Add(new Belief(obj, BeliefsTypes.Attack));
				belief.Add(new Belief(obj, BeliefsTypes.Run));
			}
		}*/
			proceed = true;
		}
	}

	private void evalBeliefs(){
		foreach (Belief b in beliefs)
			desires.Add(new Desire(b));
	}

}
