using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMainController : MonoBehaviour
{

    public float movementSpeed = 1f;
    CharacterAnimationController characterAnimationController;
    Rigidbody2D rBody;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        characterAnimationController = GetComponentInChildren<CharacterAnimationController>();
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = rBody.position;
        float horisontalInput   = Input.GetAxis("Horizontal");
        float verticalInput     = Input.GetAxis("Vertical");
        Vector2 inputVector     = new Vector2(horisontalInput, verticalInput);
        inputVector             = Vector2.ClampMagnitude(inputVector, 1);
        Vector2 movement        = inputVector * movementSpeed;
        Vector2 newPosition     = currentPosition + movement * Time.deltaTime;
        characterAnimationController.trowCharacterCurrentMovement(movement);

        rBody.MovePosition(newPosition);
    }
}

