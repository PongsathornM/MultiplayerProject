using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    /*
    private CharacterController controller;
    private float speed = 10f;
    
    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.1f;
    */

    public float speed;

    private Vector2 move;

    //[Header("Settings")]
    //[SerializeField]

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)* Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            
            controller.Move(direction * speed * Time.deltaTime);
        }
        */
        movePlayer();
    }

    public void movePlayer()
    {
        var newMove = transform.rotation; 
        if (!IsOwner) return;
        {
            Vector3 movement = new Vector3(move.x, 0f, move.y);
        
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
        }
    }
}
