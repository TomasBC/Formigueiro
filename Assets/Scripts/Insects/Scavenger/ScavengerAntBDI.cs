using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAnt {
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
		/*if (intention != null && !(intention.Type == BeliefsTypes.FindFood)) {
			Debug.Log("Entrei");
			RotateTowards (intention.IntentionObject);*/
		//} else {
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
		//}
	}


	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvaluateFieldOfView(); //Evaluate view cone
		evalBeliefs ();
		evalDesires ();
		Move();
	}

	protected override void EvaluateFieldOfView()
	{
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView ();
		List<GameObject> food, enemies, friends;

		beliefs.Clear();
		desires.Clear();

		objsInsideCone.TryGetValue ("Food", out food);
		objsInsideCone.TryGetValue ("Enemy", out enemies);
		objsInsideCone.TryGetValue ("Ant", out friends);

		if (CarryingFood ()) {
			beliefs.Add(new Belief(GameObject.Find("wall_10"), BeliefsTypes.DropFood, 0, 100000));
		}

		if (food != null && !CarryingFood()) { //Procura Comida
			foreach (GameObject obj in food){
				if(!obj.GetComponent<Food>().GetTransport()){
					if(enemies != null){
							if(friends != null)
								beliefs.Add (new Belief (obj, BeliefsTypes.CatchFood, enemies.Count, friends.Count));
							else
								beliefs.Add (new Belief (obj, BeliefsTypes.CatchFood, enemies.Count, 0));
					}else
						beliefs.Add (new Belief (obj, BeliefsTypes.CatchFood, 0, 0));
				}
			}
		}

		if (enemies != null) { //Inimigos perto
			foreach (GameObject obj in enemies){
//				if(friends != null)
//					beliefs.Add (new Belief (obj, BeliefsTypes.Run, enemies.Count, friends.Count));
//				else
					beliefs.Add (new Belief (obj, BeliefsTypes.Run, enemies.Count, 0));
					break;
			}
		}

		if (beliefs.Count == 0) {
			intention = new Intention(new Desire(new Belief(null, BeliefsTypes.FindFood, 0,0)));
		}


		/*if (objsInsideCone ["Enemy"].Count != 0) {
			foreach(GameObject obj in objsInsideCone ["Enemy"]){
				belief.Add(new Belief(obj, BeliefsTypes.Attack));
				belief.Add(new Belief(obj, BeliefsTypes.Run));
			}
		}*/
	}

	private void evalBeliefs(){
		foreach (Belief b in beliefs)
			desires.Add(new Desire(b));
	}
	private void evalDesires(){
		float desireValue = -1000;
		Intention futureIntention = null;
		Desire maxDesire = null;
		foreach (Desire d in desires) {
			if (d.DesireValue > desireValue) {
				maxDesire = d;
				desireValue = d.DesireValue;
			}
		}
		if (maxDesire != null)
			futureIntention = new Intention (maxDesire);

		if (futureIntention != null && (futureIntention.IntentionValue >= intention.IntentionValue)) {
			intention = futureIntention;
			if(intention.Type == BeliefsTypes.CatchFood)
				proceed = true;
			if(intention.Type == BeliefsTypes.CatchFood || intention.Type == BeliefsTypes.DropFood){
				RotateTowards(intention.IntentionObject);
				proceed = true;
			}
			if(intention.Type == BeliefsTypes.Run){
				proceed = true;
			}
		}
			
	}

}
