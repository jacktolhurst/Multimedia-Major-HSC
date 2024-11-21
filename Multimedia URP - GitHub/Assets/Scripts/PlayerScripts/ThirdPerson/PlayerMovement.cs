using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    // * numbers
    public float speed;
    public float sprintSpeed;
    public float turnSmooth;
    public float jumpStrength;
    public float gravity;
    public float rayDistance;
    float turnSmoothVelocity;
    float velocity;

    // * vector3s
    Vector3 totalJumpMovement;

    // * transform
    public Transform cam;

    // * controllers
    public CharacterController controller;


    void Start(){
        Debug.Log(Application.persistentDataPath);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        controller.detectCollisions = true;
    }

    void Update(){
        Move();
        Jump();
    }

    void Move(){
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float sprint = Input.GetAxis("Sprint");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f){
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmooth);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            controller.Move(moveDirection * (speed + (sprint * sprintSpeed * Convert.ToInt32(CheckGround()))) * Time.deltaTime);
        }
    }

    void Jump(){
        if(Input.GetButtonDown("Jump")){
            if(CheckGround()){
                totalJumpMovement.y =  Mathf.Sqrt(jumpStrength * -3.0f * gravity);
            }  
        }

        totalJumpMovement.y += gravity * Time.deltaTime;

        controller.Move(totalJumpMovement * Time.deltaTime);
    }

    bool CheckGround(){
        return Physics.Raycast(transform.position, Vector3.down, rayDistance);
    }

    public void LoadData(GameData data){
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data){
        data.playerPosition = this.transform.position;
    }
}
