using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private const float AIR_SPEED_FACTOR = 0.6f;
	private const float ADS_SPEED_FACTOR = 0.5f;
	public Camera playerCamera;
	public GameObject gunCamera;
	[SerializeField] private AudioClip jumpSound;
	[SerializeField] private AudioSource audioSource;
	private CharacterController characterController;
	private Player[] scoreboardData;
	private Vector3 rotationY, verticalMovement, horizontalMovement;
	private float rotationX = 0;
	private Vector3 characterVelocity;
	private float characterSpeed;
	private const float GRAVITY = 19f;
	private const float JUMP_AMOUNT = 6f;
	private float SENSITIVITY = 1.2f;
	private float ADS_SENSITIVITY = 0.7f;
	private float SCOPED_SENSITIVITY = 0.1f;
	private float currentSensitivity;
	private const int RUN_SPEED = 2;
	private float accelerationValue = 85f;
	private float decelerationValue = -70.3f;
	private Vector3 acceleration = new Vector3(0, 0 ,0);
	private Vector3 velocity = new Vector3(0, 0 ,0);
	private float recoilAmount = 0f;
	private float recoilDecreaseAcceleration = 0.003f;
	private IEnumerator recoilCoroutine;
	private int isSprinting;
	private bool isAds;
	private bool isAirborne;
	private bool atCart;
	private bool canMove;
	private bool freeze = false;
	private Teams userTeam;
	private Character player;
	private PlayerGUI gui;
	private GameController gameController;
	private Vector3 groundNormal;
	private bool isSliping = false;
	private bool gravityEnabled = true;
	private Vector3 slipVelocity = new Vector3(0, 0, 0);
	private GameObject weaponContainer;
	Texture2D pixel;
	Color pixelColor;

	void Start() {
		SENSITIVITY = Global.gameSettings.sensitivity;
		ADS_SENSITIVITY = Global.gameSettings.ads_sensitivity;
		SCOPED_SENSITIVITY = Global.gameSettings.scoped_sensitivity;
		characterController = GetComponent<CharacterController>();
		Screen.lockCursor = true;
		Cursor.visible = false;
		characterSpeed = gameObject.GetComponent<Character>().getSpeed();
		player = gameObject.GetComponent<Character> ();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
		pixelColor = Color.black;
		pixelColor.a = 0.5f;
		pixel = new Texture2D (1, 1);
		pixel.SetPixel (0, 0, pixelColor);
		pixel.Apply ();
		currentSensitivity = SENSITIVITY;
		isAds = false;
		canMove = true;
	}

	void Update() {
		isSliping = (Vector3.Angle(Vector3.up, groundNormal) >= 40f && Vector3.Angle(Vector3.up, groundNormal) < 90f && (!isAirborne || characterController.isGrounded));
		rotationY = new Vector3(0f, Input.GetAxisRaw("Mouse X"), 0f) * currentSensitivity * (1f - (60f - playerCamera.fieldOfView) / 60f);
		if (!freeze) {
			rotationX -= (Input.GetAxis ("Mouse Y") * currentSensitivity) * (1f - (60f - playerCamera.fieldOfView) / 60f);
		}
		//verticalMovement = Input.GetAxisRaw("Vertical");// * transform.forward;
		//horizontalMovement = Input.GetAxisRaw("Horizontal");// * transform.right;
		if (Input.GetKey(KeyCode.LeftShift) && !isAds) {
			isSprinting = 1;
			if (velocity.magnitude > 4) {
				//gunCamera.GetComponent<Animator>().SetBool("isSprinting", true);
			} else {
				//gunCamera.GetComponent<Animator>().SetBool("isSprinting", false);
			}
		}	else {
			isSprinting = 0;
			//gunCamera.GetComponent<Animator>().SetBool("isSprinting", false);
			}
		if (!freeze) {
			gameObject.transform.Rotate(rotationY);
		}

		// CHARACTER MOVEMENT ------------------------------------------------------------------------

		// non-directioned values
		acceleration.x = (Input.GetAxisRaw("Vertical")) * accelerationValue;
		acceleration.z = (Input.GetAxisRaw("Horizontal")) * accelerationValue;
		if (acceleration.x == 0 && Mathf.Abs(velocity.x) < 0.4f) {
			velocity.x = 0;
		}
		if (acceleration.z == 0 && Mathf.Abs(velocity.z) < 0.4f) {
			velocity.z = 0;
		}
		if (Mathf.Abs(velocity.x) > 0) {
			acceleration.x += (Mathf.Abs(velocity.x) / velocity.x) * decelerationValue;
		}
		if (Mathf.Abs(velocity.z) > 0) {
			acceleration.z += (Mathf.Abs(velocity.z) / velocity.z) * decelerationValue;
		}
		if (isSliping) {
			//slipVelocity.x += (1f - groundNormal.y) * groundNormal.x * 50f;
			//slipVelocity.z += (1f - groundNormal.y) * groundNormal.z * 50f;
		} else {
			slipVelocity = Vector3.zero;
		}
		// Debug.Log(velocity.x.ToString() + " " + velocity.z.ToString());
		velocity.x += acceleration.x * Time.deltaTime;
		velocity.z += acceleration.z * Time.deltaTime;
		// max velocity
		float maxSpeed = characterSpeed * (isAds ? ADS_SPEED_FACTOR : 1f) + (RUN_SPEED * isSprinting);
		// maxSpeed *= (isAirborne ? 0.6f : 1f);
		float speedToMaxSpeed = velocity.magnitude / maxSpeed;
		if (speedToMaxSpeed > 1) {
			velocity /= speedToMaxSpeed;
		}
		// aligning velocity to direction
		characterVelocity.x = (velocity.x * transform.forward).x + (velocity.z * transform.right).x + slipVelocity.x;
		characterVelocity.z = (velocity.x * transform.forward).z + (velocity.z * transform.right).z + slipVelocity.z;

		// ---------------------------------------------------------------------------------------------
		// Clamp camera angle
 		rotationX = Mathf.Clamp (rotationX, -90.0f, 90.0f);
		Camera.main.transform.localRotation = Quaternion.Euler (rotationX, 0, 0);
		if (!characterController.isGrounded && !isAirborne) {
			characterVelocity.y = 0;
			isAirborne = true;
			StartCoroutine(enableGravity());
		}
		if (characterController.isGrounded) {
			if (isAirborne) {
				isAirborne = false;
				gravityEnabled = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space) && !isSliping && !isAirborne) {
			characterVelocity.y = JUMP_AMOUNT;
			isAirborne = true;
			gravityEnabled = false;
			StartCoroutine(enableGravity());
			audioSource.PlayOneShot(jumpSound);
		}
		else if ((!characterController.isGrounded || isSliping) && gravityEnabled) {
			characterVelocity.y -= GRAVITY * Time.deltaTime;
			if (Mathf.Abs(characterVelocity.y) < 0.05f) {
				characterVelocity.y = -0.1f;
			}
		}
		Vector3 movementVector = characterVelocity * Time.deltaTime;
		if (isSliping) {
			// Only contribute the part of the movement in the downwards direction of the slope
			Vector3 projMovementOntoNormal = new Vector3(0, 0, 0);
			float dot = (groundNormal.x * movementVector.x) + (groundNormal.z * movementVector.z) / ((groundNormal.x * groundNormal.x) + (groundNormal.z * groundNormal.z));
			projMovementOntoNormal.x = Mathf.Max(groundNormal.x * dot, 0f);
			projMovementOntoNormal.z = Mathf.Max(groundNormal.z * dot, 0f);
			movementVector.x = (groundNormal.x * 0.05f) + (movementVector.x - projMovementOntoNormal.x);
			movementVector.z = (groundNormal.z * 0.05f) + (movementVector.z - projMovementOntoNormal.z);
			movementVector.y -= groundNormal.y * 0.05f;
		}
 		if (canMove) {
			characterController.Move(movementVector);
		}
	}

	public void setSensitivity(int state) {
		switch (state) {
		case 0:
			currentSensitivity = SENSITIVITY;
			break;
		case 1:
			currentSensitivity = ADS_SENSITIVITY;
			break;
		case 2:
			currentSensitivity = SCOPED_SENSITIVITY;
			break;
		}
	}

	private void OnGUI() {
		if (Input.GetKey (KeyCode.Tab)){
			userTeam = gameController.getUserTeam (player.getUserId());
			scoreboardData = userTeam.getTeamStats ();
		  float screenWidth = Screen.width;
		  float screenHeight = Screen.height;
			int playerCount = 0;
			string playerCell;
			GUI.DrawTexture (new Rect (screenWidth/8 - screenWidth/100, screenHeight/4, 3*screenWidth/4 + screenWidth/100, screenHeight/2), pixel);
			foreach (Player statLine in scoreboardData) {
				if (statLine == null) {
					playerCell = "";
				} else {
					playerCell = Global.CHARACTER_NAMES [playerCount] + "\n" + statLine.getUsername () + "\nKills: "	+ statLine.getStatLine ().kills + "\nAssists: " + statLine.getStatLine ().assists + "\nDeaths: " + statLine.getStatLine ().deaths + "\nGold Stolen" + statLine.getStatLine ().goldStolen;
				}
				GUI.Label(new Rect ((screenWidth/8) + (3*screenWidth/20 * playerCount), screenHeight/4, 3*screenWidth/20, screenHeight/2), playerCell);
		        playerCount++;
	      	}
	    }
  	}

	public void setSpeed (float speed) {
		characterSpeed = speed;
	}

	public void changeAdsState(bool state) {
		isAds = state;
	}
	private IEnumerator resetAcceleration() {
		decelerationValue = -160f;
		yield return new WaitForSeconds(0.05f);
		decelerationValue = -49.3f;
	}

	public float getSpeed() {
		return characterSpeed;
	}

	public void setAbilityToMove(bool ableToMove) {
  	canMove = ableToMove;
  }
	public void setFreeze (bool isFreeze) {
		freeze = isFreeze;
	}
	public bool getFreeze () { return freeze; }
	public void setRecoil(float recoil) {
		recoilAmount += recoil;
		rotationX -= recoil;
		if (recoilCoroutine != null) {
			StopCoroutine(recoilCoroutine);
		}
		recoilCoroutine = resetRecoil();
		StartCoroutine(recoilCoroutine);
	}
	private IEnumerator resetRecoil() {
		float decreaseAmount = 0.025f;
		while (recoilAmount > 0) {
			if (recoilAmount < decreaseAmount) {
				rotationX += recoilAmount;
				recoilAmount = 0;
			} else if (recoilAmount > 0) {
				rotationX += decreaseAmount;
				recoilAmount -= decreaseAmount;
			}
			yield return new WaitForSeconds(0.005f);
			decreaseAmount += recoilDecreaseAcceleration;
		}
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
		groundNormal = hit.normal;
	}
	private IEnumerator enableGravity() {
		yield return new WaitForSeconds(0.05f);
		gravityEnabled = true;
	}
	public void setArmPivot(GameObject container) {
		weaponContainer = container;
	}
}
