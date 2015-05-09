using UnityEngine;
using System.Collections;

public class Desire {
	private float danger, confidence;
	private Belief belief;

	public Desire(Belief belief){
		this.belief = belief;
		danger = belief.EnemiesCount * 10;
		confidence = belief.FriendsCount * 5;
	}

}
