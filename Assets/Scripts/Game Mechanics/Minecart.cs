﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour {
	private const int STARTING_GOLD = 500;
	private int goldCount;
	[SerializeField] private int teamid;
	private float cartSpeed;
	private bool isStarted = false;

	void Start () {
		goldCount = STARTING_GOLD;
	}

	void FixedUpdate() {
		if (isStarted) {
			cartSpeed = (float)goldCount / STARTING_GOLD * Time.deltaTime;
			gameObject.transform.Translate (Vector3.forward * cartSpeed);
		}
	}

	public int getTeamId(){
		return teamid;
	}

	public void startCarts(){
		isStarted = true;
	}

	public int getGold(){
		return goldCount;
	}

	public void setCartGold(int gold) {
		goldCount += gold;
	}

}
