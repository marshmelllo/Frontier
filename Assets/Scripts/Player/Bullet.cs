﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private const int LIFE_SPAN = 3;
	private Vector3 currentPosition, lastPosition;
	private float positionDifference;
	private RaycastHit hit;
	private Ray ray;
	private Rigidbody rigidbod;
	private int userId;
	[SerializeField] GameObject dirtImpactParticles;
	void Start () {
		if(!PhotonNetwork.isMasterClient) { enabled = false; }
		object[] data = GetComponent<PhotonView>().instantiationData;
		setUserId ((int)data[0]);
		currentPosition = gameObject.transform.position;
		lastPosition = gameObject.transform.position;
		rigidbod = gameObject.GetComponent<Rigidbody>();
		rigidbod.detectCollisions = false;
		rigidbod.velocity = (Vector3)data[1];
		StartCoroutine(setTimeOutDestroy());

	}

	public void setUserId(int id) {
		userId = id;
	}

	void FixedUpdate() {
		gameObject.transform.rotation = Quaternion.LookRotation(rigidbod.velocity.normalized);
		currentPosition = gameObject.transform.position;
		positionDifference = (currentPosition - lastPosition).magnitude;
		// check if there was a collision in the last 0.1 units
		if (positionDifference > 0.1f) {
			ray = new Ray(lastPosition,rigidbod.velocity.normalized);
			if (Physics.Raycast(ray, out hit, positionDifference)) {
				if (hit.collider.gameObject.tag == "Player") {
					hit.collider.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -25, userId);
				} else {
					PhotonNetwork.Instantiate ("WFX_BImpact Sand", ray.GetPoint(hit.distance), Quaternion.EulerAngles(new Vector3(-90, 0, 0)), 0);
				}
				PhotonNetwork.Destroy(gameObject);
			}
			lastPosition = currentPosition;
		}
	}
	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		PhotonNetwork.Destroy(gameObject);
	}

}
