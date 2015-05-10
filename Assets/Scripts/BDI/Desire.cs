using UnityEngine;
using System.Collections;

public class Desire {
	private float danger, confidence;
	private Belief belief;

	public Belief Belief {
		get {
			return belief;
		}
	}

	private float desireValue;

	public float DesireValue {
		get {
			return desireValue;
		}
	}

	public Desire(Belief belief){
		this.belief = belief;
		danger = belief.EnemiesCount * 10;
		confidence = belief.FriendsCount * 5;
		desireValue = confidence - danger;
	}


}
