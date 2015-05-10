using UnityEngine;
using System.Collections;

public class Intention {
	private GameObject intentionObject;
	public GameObject IntentionObject{
		get{return intentionObject;}
	}

	private BeliefsTypes type;

	public BeliefsTypes Type {
		get {
			return type;
		}
	}

	private Desire desire;

	public Desire Desire {
		get {return desire;}
	}

	private float intentionValue;

	public float IntentionValue {
		get {return intentionValue;}
	}

	public Intention(Desire desire){
		intentionObject = desire.Belief.BeliefObject;
		this.desire = desire;
		type = desire.Belief.Type;
		intentionValue = desire.DesireValue;
	}


}
