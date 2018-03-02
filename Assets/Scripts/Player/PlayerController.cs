usingSystem.Collections;
usingSystem.Collections.Generic;
usingUnityEngine;

publicclassPlayerController:MonoBehaviour{
	publicCameraplayerCamera;
	[SerializeField]privateAudioClipjumpSound;
	[SerializeField]privateAudioSourceaudioSource;
	privateCharacterControllercharacterController;
	privateVector3rotationX,rotationY,verticalMovement,horizontalMovement;
	privateVector3characterVelocity;
	privateconstfloatGRAVITY=12f;
	privateconstfloatCHARACTER_SPEED=5f;
	privateconstfloatJUMP_AMOUNT=4f;
	privateconstfloatSENSITIVITY=1.2f;
	privateintisSprinting;
	voidStart(){
		characterController=GetComponent<CharacterController>();
		Screen.lockCursor=true;
		Cursor.visible=false;
	}

	voidUpdate(){
		rotationY=newVector3(0f,Input.GetAxisRaw("MouseX"),0f)*SENSITIVITY;
		rotationX=newVector3(Input.GetAxisRaw("MouseY"),0f,0f)*-SENSITIVITY;
		verticalMovement=Input.GetAxisRaw("Vertical")*transform.forward;
		horizontalMovement=Input.GetAxisRaw("Horizontal")*transform.right;
		if(Input.GetKey(KeyCode.LeftShift)){
			isSprinting=1;
		}else{isSprinting=0;}

		playerCamera.transform.Rotate(rotationX);
		gameObject.transform.Rotate(rotationY);

		characterVelocity.x=(verticalMovement.x+horizontalMovement.x)*CHARACTER_SPEED+isSprinting*3;
		characterVelocity.z=(verticalMovement.z+horizontalMovement.z)*CHARACTER_SPEED+isSprinting*3;


			
		if(characterController.isGrounded){
			characterVelocity.y=0;
		}

		if(Input.GetKeyDown(KeyCode.Space)&&characterController.isGrounded){
				characterVelocity.y=JUMP_AMOUNT;
				audioSource.PlayOneShot(jumpSound);
			}
		else{
			characterVelocity.y-=GRAVITY*Time.deltaTime;
		}

		characterController.Move(characterVelocity*Time.deltaTime);
	}
	voidFixedUpdate(){

	}
}
