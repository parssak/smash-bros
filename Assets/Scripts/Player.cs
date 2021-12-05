using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6.0f;
    public float gravity = -600.0f;
    public float jumpSpeed = 8.0f;
    public float jumpDuration = 0.5f;
    public AnimationCurve jumpCurve;
    private bool isJumping = false;
    private int jumpCount = 0;

    private Vector3 spawnPoint = new Vector3(0, 30, 0);
    private float deathDistance = 100f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        Vector3 moveDirection = ReceiveInput();
        if (controller.isGrounded) {
            jumpCount = 0;
        }
        // check if distance from spawnPoint is greater than deathDistance
        if (Vector3.Distance(spawnPoint, transform.position) > deathDistance) {
            Die();
            return;
        }
        HandleMovement(moveDirection);
    }

    void Die() {
        Debug.Log("Died");
        transform.position = spawnPoint;
    }

    Vector3 ReceiveInput() {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = Input.GetAxis("Horizontal") * speed;
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Vertical"))  {
            moveDirection.y = 1;
        }
        return moveDirection;
    }

    bool CanJump() {
        return jumpCount < 2;

    }

    void HandleMovement(Vector3 moveDirection) {
        if (moveDirection.y > 0.0f) {
            if (CanJump()) HandleJump();
            // else if (!isJumping) Gap(CanJump, HandleJump, 0.1f);
        }
        if (!isJumping) {
            moveDirection.y += gravity;
        }
        if (controller.isGrounded) {
            moveDirection.y = 0;
        }
        controller.Move(moveDirection * Time.deltaTime);
    }

    // pass in function that returns boolean, and function to call if it returns true
    void Gap(System.Func<bool> condition, System.Action action, float duration) {
        StartCoroutine(GapCoroutine(condition, action, duration));
    }
    
    IEnumerator GapCoroutine(System.Func<bool> condition, System.Action action, float duration) {
        float startTime = Time.time;
        while (Time.time < startTime + duration) {
            if (condition()) {
                action();
                yield break;
            }
            yield return null;
        }
    }

    void HandleJump() {
        jumpCount++;
        if (isJumping) {
            StopCoroutine(JumpCoroutine());
        }
        StartCoroutine(JumpCoroutine());
    }  

    IEnumerator JumpCoroutine() {
        float jumpTime = 0.0f;
        float jumpingSpeed = jumpSpeed * jumpCount;
        isJumping = true;
        while (jumpTime < jumpDuration) {
            jumpTime += Time.deltaTime;
            float percent = jumpTime / jumpDuration;
            jumpingSpeed = jumpSpeed * jumpCurve.Evaluate(percent);
            controller.Move(Vector3.up * jumpingSpeed * Time.deltaTime);
            yield return null;
        }
        isJumping = false;
    }
}
