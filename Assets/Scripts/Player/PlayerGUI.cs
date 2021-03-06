﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {
	[SerializeField] private Text interactText;
	[SerializeField] private Text healthText;
	[SerializeField] private Text goldText;
	[SerializeField] private Text ammoText;
	[SerializeField] private RawImage crosshair;
	[SerializeField] private Image scope;
	[SerializeField] private RawImage hitMarker;
	void Start() {
		hitMarker.enabled = false;
	}

	public void setHealth(int health){
		healthText.text = "Health: " + health;
	}

	public void setGold(int gold, int max) {
		goldText.text = "Gold: " + gold + "/" + max;
	}

	public void setInteract(string message) {
		interactText.text = message;
	}

	public void setCrosshairEnabled (bool enabled) {
		crosshair.enabled = enabled;
	}
	public void setScopeEnabled (bool enabled) {
		scope.enabled = enabled;
	}
	public void setAmmoCounter(int current, int max) {
		ammoText.text = current + "/" + max;
	}

	public void hitMarked() {
		hitMarker.color = Color.white;
		hitMarker.enabled = true;
		StartCoroutine (hitMarkerTime());
	}

	public void killMarked() {
		hitMarker.color = Color.red;
		hitMarker.enabled = true;
		StartCoroutine (hitMarkerTime());
	}

	private IEnumerator hitMarkerTime() {
		yield return new WaitForSeconds(0.5f);
		hitMarker.enabled = false;
	}
}
