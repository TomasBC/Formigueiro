using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beliefs {
	protected BeliefsTypes type;
	protected List<GameObject> enemies;
	protected List<GameObject> friends;
	protected List<GameObject> food;
	protected int enemiesCount;
	public int EnemiesCount{
		get{return enemiesCount;}
	}
	public int FriendsCount{
		get{return friendsCount;}
	}
	protected int friendsCount;


	public Beliefs(){}

	public void addEnemie(GameObject enemy){
		enemies.Add (enemy);
		enemiesCount = enemies.Count;
	}

	public void addFriends(GameObject friend){
		friends.Add (friend);
		friendsCount = friends.Count;
	}

	public void addFood(GameObject foodElement){
		food.Add (foodElement);
	}
}
