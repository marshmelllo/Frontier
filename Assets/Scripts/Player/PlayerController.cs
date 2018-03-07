using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
 
public class PlayerController : MonoBehaviour { 
	public Camera playerCamera; 
	[SerializeField] private AudioClip jumpSound; 
	[SerializeField] private AudioSource audioSource; 
	private CharacterController characterController; 
	private Vector3 rotationX, rotationY, verticalMovement, horizontalMovement; 
	private Vector3 characterVelocity; 
	private float characterSpeed;
	private const float GRAVITY = 12f; 
	private const float JUMP_AMOUNT = 4f; 
	private const float SENSITIVITY = 1.2f; 
	private const int RUN_SPEED = 3;
	private int isSprinting; 
	private bool atCart;
	private Character player;
	private PlayerGUI gui;
	private GameController gameController;
	void Start() { 
		characterController = GetComponent<CharacterController>(); 
		Screen.lockCursor = true; 
		Cursor.visible = false;
		characterSpeed = gameObject.GetComponent<Character>().getSpeed();
		player = gameObject.GetComponent<Character> ();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
	}
 
	void Update() { 
		rotationY = new Vector3(0f, Input.GetAxisRaw("Mouse X"), 0f) * SENSITIVITY; 
		rotationX = new Vector3(Input.GetAxisRaw("Mouse Y"), 0f, 0f) * -SENSITIVITY; 
		verticalMovement = Input.GetAxisRaw("Vertical") * transform.forward; 
		horizontalMovement = Input.GetAxisRaw("Horizontal") * transform.right; 
		if (Input.GetKey(KeyCode.LeftShift)) { 
			isSprinting = 1; 
		}
    	else { isSprinting = 0; } 
 
		playerCamera.transform.Rotate(rotationX); 
		gameObject.transform.Rotate(rotationY); 
		characterVelocity.x = (verticalMovement.x + horizontalMovement.x) * (characterSpeed + isSprinting * RUN_SPEED); 
		characterVelocity.z = (verticalMovement.z + horizontalMovement.z) * (characterSpeed + isSprinting * RUN_SPEED); 
 
 
			 
		if (characterController.isGrounded) { 
			characterVelocity.y = 0; 
		} 
 
		if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) { 
				characterVelocity.y = JUMP_AMOUNT; 
				audioSource.PlayOneShot(jumpSound); 
		} 
		else { 
			characterVelocity.y -= GRAVITY * Time.deltaTime; 
		} 
 
		characterController.Move(characterVelocity * Time.deltaTime); 

	}	
} 
