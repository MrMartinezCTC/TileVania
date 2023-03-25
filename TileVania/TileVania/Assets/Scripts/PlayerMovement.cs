using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float startingGravity = 5f;

    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator animator;
    Collider2D myCapsuleCollider;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<Collider2D>();

        myRigidBody.gravityScale = startingGravity;
    }

    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        bool playerIsGrounded = myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")); // Prevent double jump
        bool playerIsOnLadder = myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        Debug.Log(value.isPressed && (playerIsGrounded || playerIsOnLadder));
        if (value.isPressed && (playerIsGrounded || playerIsOnLadder))
        {
            myRigidBody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * playerSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        animator.SetBool("isRunning", PlayerIsMovingHorizontally());
    }


    void FlipSprite()
    {
        if (PlayerIsMovingHorizontally())
        {
            transform.localScale = new Vector2(Mathf.Sign(moveInput.x), 1f);
        }
    }

    bool PlayerIsMovingHorizontally()
    {
        return Mathf.Abs(moveInput.x) > Mathf.Epsilon;
    }

    void ClimbLadder()
    {
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myRigidBody.gravityScale = startingGravity;
            animator.SetBool("isClimbing", false);
            return;
        }

        myRigidBody.gravityScale = 0;
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
        myRigidBody.velocity = climbVelocity;

        animator.SetBool("isClimbing", Math.Abs(moveInput.y) > 0);
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
    //    {
    //    }
    //}
}
