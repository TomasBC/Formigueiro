using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemyBDI : Enemy 
{
	// Beliefs impact
	public int enemyImpact = 2;
	public int friendsImpact = 1;
	public float runDistance = 15f;

	// Beliefs
	private List<GameObject> ants;
	private List<GameObject> friends;
	private List<GameObject> soldiers;
	private List<GameObject> scavengers;

	// Intentions and desires
	private Intention intention;
	private List<Desire> desires;

	private bool run = false;
	private bool attack = false;
	
	// Initialization
	protected override void Start() 
	{ 
		base.Start();

		desires = new List<Desire>();
		soldiers = new List<GameObject>();
		scavengers = new List<GameObject>();

		guiStyle.fontSize = 8;
		guiStyle.normal.textColor = Color.black;
	}
	
	// Called every fixed framerate frame
	protected override void FixedUpdate() 
	{
		base.FixedUpdate();
		EvalBeliefs();
		EvalDesires();
		Move();
	}

	// Called per event
	protected void OnGUI() {
		
		if (intention != null) {
			
			worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
			rect.Set(worldToScreen.x, Screen.height - worldToScreen.y, 80, 20);
			GUI.Label(rect, intention.Type.ToString() + " - " + Mathf.Round(energy), guiStyle);
		}
	}
	
	// Reactors
	protected override void Move() 
	{
		/*
		 * Attack situation:
		 * 
		 * The enemy agent rotates and moves towards an ant 
		 * i.e attacking it. However if the distance between 
		 * is exceeded by a certain amount she drops the chase
		 * 
		 */
		if (attack) {

			if(intention == null || intention.Type == DesireType.Attack || 
			   Vector3.Distance(transform.position, intention.IntentionDest.position) > runDistance) {
				attack = false;
				intention = null;
			}
			else {
				transform.LookAt(intention.IntentionDest.transform.position);
				transform.position = Vector3.MoveTowards(transform.position, intention.IntentionDest.transform.position, 0.5f * this.speed * Time.fixedDeltaTime);
			}
		}
		
		/*
		 *  Collision situation:
		 *
		 *	This situation occurs when the agent collides with some obstacle,
		 *	i.e walls, enemies, ants, and provides a random rotation across
		 *	three different values (90º, -90º, 180º). This allows the agent
		 *	to proceed with its actions.
		 * 
		 */
		else if (collided && !proceed) {	
			Rotate(randomMin);
			collided = false;
		}	
		/*
		* Generic move situation:
		* 
		* This happens when the agent (enemy), doesn't really
		* know its purpose i.e, no beliefs exist. The agent randomly
		* navigates across the map using a randomMax value for rotation.
		* 
		*/ 
		else {			
			Rotate(randomMax);
			base.Move();
		}
		
		/*
		 * Run situation:
		 * 
		 * This situation occurs when the agent spots an enemy. The agent
		 * starts running for his life and drops the food if transporting
		 * any. It runs for a certain distance and eventually leaves the 
		 * run state.
		 * 
		 */
		if (run) {


			if(intention == null || intention.Type != DesireType.Run || 
			   Vector3.Distance(transform.position, intention.IntentionDest.position) > runDistance) {
				
				speedRunning = false;
				speed -= speedRunIncrement;
				
				run = false;
				intention = null;
			}

			// Increment speed
			if(!speedRunning) {
				speedRunning = true;
				speed += speedRunIncrement;
			}
		}
	}
	

	// BDI Evaluators	
	protected void EvalBeliefs()
	{
		// Clear
		desires.Clear();
		soldiers.Clear();
		scavengers.Clear ();
	
		// Gather information about beliefs: Ants and Friends
		Dictionary<string, List<GameObject>> objsInsideCone = CheckFieldOfView();
		objsInsideCone.TryGetValue("Ant", out ants);
		objsInsideCone.TryGetValue("Enemy", out friends);

		// Calculate danger and confidence
		int danger = 0;
		int confidence = 0;

		if (friends != null) {
			confidence = friends.Count * friendsImpact;
		}

		if (ants != null) {

			foreach (GameObject ant in ants) {

				if (ant.name.Contains("soldier")) {
					soldiers.Add(ant);
				} else {
					scavengers.Add(ant);
				}
			}
			danger = soldiers.Count * enemyImpact;
			confidence += scavengers.Count * friendsImpact;

			//Order by proximity
			ants.OrderBy(ant => (ant.transform.position - transform.position).sqrMagnitude);
			desires.Add(new Desire(DesireType.Attack, ants[0].transform, danger, confidence, DesirePriorities.ATTACK_PRIORITY));
			desires.Add(new Desire(DesireType.Run, ants[0].transform, confidence, danger, DesirePriorities.RUN_PRIORITY)); 
		}
		// Default beliefs (Exit or FindFood)  
		if (desires.Count == 0) {
			desires.Add(new Desire(DesireType.Patrol, null, danger, confidence, DesirePriorities.PATROL_PRIORITY));
		}
	}
	
	private void EvalDesires()
	{
		float desireValue = -1000f;
		
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
			
			futureIntention = new Intention (bestDesire);
			
			if (intention == null || futureIntention.IntentionValue > intention.IntentionValue || futureIntention.Type == intention.Type) {
				
				//Update current intention
				intention = futureIntention;
				
				switch (intention.Type) {
				case DesireType.Attack:
					attack = true;
					break;
				case DesireType.Patrol:
					//Just do stuff
					break;
				case DesireType.FindFood:
					//Just do stuff
					break;
				case DesireType.Run:
					run = true;
					break;
				}
			}
		}
	}
}