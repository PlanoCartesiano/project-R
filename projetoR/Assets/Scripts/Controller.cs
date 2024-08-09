using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	float movementSpeed = 2.0f;

	public void Update(){
		float rTriggerFloat = Input.GetAxis("Right Trigger");
		float lTriggerFloat = Input.GetAxis("Left Trigger");
		bool leftBumper = Input.GetButton("Left Bumper");
		bool rightBumper = Input.GetButton("Right Bumper");
		bool selectButton = Input.GetButton("Select");
		bool startButton = Input.GetButton("Start");
		bool aButton = Input.GetButton("A Button");
		bool bButton = Input.GetButton("B Button");
		bool xButton = Input.GetButton("X Button");
		bool yButton = Input.GetButton("Y Button");
		float dpadHorizontal = Input.GetAxis("Dpad Horizontal") * movementSpeed;
		float dpadVertical = Input.GetAxis("Dpad Vertical") * movementSpeed;
		float moveHL = Input.GetAxis("Horizontal") * movementSpeed;
		float moveVL = Input.GetAxis("Vertical") * movementSpeed;
		float moveHR = Input.GetAxis("Mouse X") * movementSpeed;
		float moveVR = Input.GetAxis("Mouse Y") * movementSpeed;

		moveHL *= Time.deltaTime;
		moveVL *= Time.deltaTime;
		moveHR *= Time.deltaTime;
		moveVR *= Time.deltaTime;
	}
}
