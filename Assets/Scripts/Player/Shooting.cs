using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {
	public Camera playerCamera;
	private float shootTime = 10f;
    private const float SHOT_DELAY = 0.5f;
	private RaycastHit hit;
	private Ray ray;
	private Vector3 endpoint;
	private float distance;
    private bool canshoot = true;
	private Vector3 ads, hip;
	private PlayerGUI gui;
	[SerializeField] private GameObject armPivot;
	[SerializeField] private GameObject gunContainer;
	[SerializeField] private GameObject revolver;
	[SerializeField] private GameObject sniper;
	[SerializeField] private GameObject semiAuto;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private GameObject tipOfGun;
	[Header("Sounds")]
	[SerializeField] private AudioClip revolverSound;
	[SerializeField] private AudioClip sniperSound;
	[SerializeField] private AudioClip semiAutoSound;
	private Animator armPivotAnimator;
	private GameObject currentGun;
	private PhotonView photonView;
	LayerMask ignoreRayCastLayer;
	Character player;

	void Start () {
		currentGun = revolver;
		hip = new Vector3(0, 0, 0);
		ads = new Vector3(-0.24f, 0.09f, -0.18f);
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
		endpoint = new Vector3(0,0,0);
		distance = 0;
		photonView = gameObject.GetComponent<PhotonView>();
		armPivotAnimator = armPivot.GetComponent<Animator>();
		player = gameObject.GetComponent<Character> ();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		// all layers except 2nd which is Ignore Raycast
		ignoreRayCastLayer = ~(1 << 2);
	}

	void Update () {
		if (!photonView.isMine) { return; }


		if (Input.GetKeyDown (KeyCode.Alpha1) && currentGun != revolver) {
			swapGuns (revolver);
		}

		else if (Input.GetKeyDown (KeyCode.Alpha2) && currentGun != sniper) {
			swapGuns(sniper);
		}

		else if ((Input.GetKeyDown (KeyCode.Alpha3) && currentGun != semiAuto)) {
			swapGuns (semiAuto);
		}

		if (Input.GetButtonDown ("Fire1") && canshoot == true) {
      		//Get Point where bullet will hit
      		StartCoroutine(delayedShooting());
     		armPivotAnimator.SetTrigger("shooting");
			ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward*100);
			if (Physics.Raycast(ray ,out hit, Mathf.Infinity, ignoreRayCastLayer)) {
				endpoint = ray.GetPoint(hit.distance);
			} else {
			endpoint = ray.GetPoint(1000);
			}

			gameObject.GetComponent<PhotonView>().RPC("shoot",PhotonTargets.All, tipOfGun.transform.position,endpoint, player.getUserId());
		}
		if (Input.GetButtonDown("Fire2")) {
			gunContainer.transform.localPosition = ads;
			gui.toggleCrosshair();

		} else if (Input.GetButtonUp("Fire2")) {
			gunContainer.transform.localPosition = hip;
			gui.toggleCrosshair();
		}
	}
	[PunRPC]
	private void shoot(Vector3 start, Vector3 end, int userId) {
		if (revolver.activeSelf) {
			audioSource.PlayOneShot (revolverSound);
		} else if (sniper.activeSelf) {
			audioSource.PlayOneShot (sniperSound);
		} else if (semiAuto.activeSelf) {
			audioSource.PlayOneShot (semiAutoSound);
		}
		if(!PhotonNetwork.isMasterClient) { return; }
		//create the bullet at tip of gun
		PhotonNetwork.Instantiate ("Bullet", start ,Quaternion.LookRotation(Vector3.Normalize(end-start)), 0, new object[] {userId, Vector3.Normalize(end-start)*300, photonView.viewID});
		//shot.GetComponent<Rigidbody>().velocity = Vector3.Normalize(end-start)*300;
		//shot.GetComponent<Bullet> ().setUserId (userId);
	}

    private IEnumerator delayedShooting(){
        canshoot = false;
        yield return new WaitForSeconds(SHOT_DELAY);
        canshoot = true;
    }

	private void swapGuns(GameObject newGun) {
		currentGun.SetActive (false);
		newGun.SetActive (true);
		currentGun = newGun;
	}
}
