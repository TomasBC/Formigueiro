using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScavengerAntBDI : ScavengerAnt 
{
	public float desireValue = -1000f;

	public int enemyImpact = 10;
	public int friendsImpact = 5;

	private List<GameObject> foood;
	private List<GameObject> friends;
	private List<GameObject> enemies;

	private Intention intention;
	private List<Belief> beliefs;
	private List<Desire> desires;

	// Initialization
	protected override void Start() 
	{
		base.Start();
		beliefs = new List<Belief>();
		desires = new List<Desire>();
	}

	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvaluateFieldOfView(); //Evaluate view cone
		EvalBeliefs();
		EvalDesires();
		Move();
	}

	// Reactors
	protected override void Move() 
	{
		if (!collided && !proceed) {
			base.Move(); //Move forward and rotate max
			Rotate(randomMax);
		} else if (!collided && proceed) {
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

		// Clear beliefs and desires
		beliefs.Clear();
		desires.Clear();

		// Gather information about: Food, Ants (Friends) and Enemies
		objsInsideCone.TryGetValue("Food", out foood);
		objsInsideCone.TryGetValue("Ant", out friends);
		objsInsideCone.TryGetValue("Enemy", out enemies);

		// If we see some food and we are not carrying any
		if (foood != null && !CarryingFood()) {

			foreach (GameObject obj in foood) {
				//If food is not being transported
				if (!obj.GetComponent<Food>().Transport) {
					beliefs.Add(new Belief(obj, BeliefType.CatchFood)); //Add catch food belief
				}
			}
		}

		if (enemies != null) { //Nearby enemies

			foreach (GameObject obj in enemies) {
				beliefs.Add(new Belief(obj, BeliefType.Run)); //Add run belief
			}
		}

		// Check if we have any beliefs		         
		if (beliefs.Count == 0) {
			intention = new Intention(new Desire(new Belief(null, BeliefType.FindFood), 0, 0));

			//TODO: Add reactive agent behaviour in this situation? 
		}
	}

	private void EvalBeliefs()
	{
		float danger = 0f;
		float confidence = 0f;
	
		foreach (Belief belief in beliefs) {

			if(enemies != null && friends != null) {
				danger = enemies.Count * enemyImpact;
				confidence = friends.Count * friendsImpact;
			}
			desires.Add(new Desire(belief, danger, confidence));
		}
	}
	
	private void EvalDesires()
	{
		Desire bestDesire = null;
		Intention futureIntention = null;

		//Get best desire
		foreach (Desire desire in desires) {

			if (desire.DesireValue > desireValue) {
				bestDesire = desire;
				desireValue = desire.DesireValue;
			}
		}

		if (bestDesire != null) {
			futureIntention = new Intention(bestDesire);
		}
		
		if (futureIntention != null && (futureIntention.IntentionValue >= intention.IntentionValue)) {

			//Update current intention
			intention = futureIntention;

			switch (intention.Type) {
			case BeliefType.CatchFood:
				RotateTowards(intention.IntentionObject);
				proceed = true;
				break;
			case BeliefType.DropFood:
				proceed = true;
				break;
			case BeliefType.Run:
				run = true;
				break;
			}
		}
	}
}