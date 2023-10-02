using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // [SerializeField] TMP_Text timerTextUp;
    // [SerializeField] TMP_Text timerTextDown;

    Rigidbody rb;
    Animator anim;
    public float jumpHeightApex = 2f;
    public float jumpDuration = 1f;
    public float downwardsGravityMultiplier = 1f;

    public float speed = 1.0f;
    public float maxSpeed = 5.0f;

    public bool isJumping = false;
    public bool isGrounded;
    public bool allowDoubleJump = false;

    bool doubleJumped = false;

    float gravity;
    float initialJumpVelocity;
    float jumpStartTime;
    Vector3 targetVelocity;
    public float groundDistance = 1f;

    float timerUp = 0f;
    float timerDown = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // displacement = initial velocity * time + 0.5 * acceleration due to gravity * time^2
        // solving for acceleration: acceleration = -2 * displacement / (time^2)
        gravity = -2 * jumpHeightApex / (jumpDuration * jumpDuration);

        // initialVelocity = acceleration * time
        initialJumpVelocity = Mathf.Abs(gravity) * jumpDuration;
    }

    private void Update()
    {
        // RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, groundDistance + 0.1f);
        // visual aid for raycast
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), Vector3.down * (groundDistance + 0.1f), Color.red);

        anim.SetFloat("PosX", rb.velocity.x);
        anim.SetFloat("PosY", rb.velocity.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGrounded && allowDoubleJump && !doubleJumped)
            {
                doubleJumped = true;
                anim.SetBool("isJumping", true);
                StartJump();
            }
            else if (isGrounded)
            {
                doubleJumped = false;
                anim.SetBool("isJumping", true);
                StartJump();
            }
        }

        if (isGrounded && !isJumping)
        {
            anim.SetBool("isJumping", false);
        }

        // // Move forward and backward with force
        // if (Input.GetKey(KeyCode.W))
        // {
        //     rb.AddForce(Vector3.forward * 1f, ForceMode.Impulse);
        // }
        // else if (Input.GetKey(KeyCode.S))
        // {
        //     rb.AddForce(Vector3.back * 1f, ForceMode.Impulse);
        // }

        // if (Input.GetKey(KeyCode.A))
        // {
        //     rb.AddForce(Vector3.left * 1f, ForceMode.Impulse);
        // }
        // else if (Input.GetKey(KeyCode.D))
        // {
        //     rb.AddForce(Vector3.right * 1f, ForceMode.Impulse);
        // }

        targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        // UpdateTimers();
    }

    void FixedUpdate()
    {

        rb.AddForce(targetVelocity * speed, ForceMode.Acceleration);

        // Cap x and z acceleration

        if (rb.velocity.x > maxSpeed)
        {
            rb.velocity = new Vector3(maxSpeed, rb.velocity.y, rb.velocity.z);
        }
        else if (rb.velocity.x < -maxSpeed)
        {
            rb.velocity = new Vector3(-maxSpeed, rb.velocity.y, rb.velocity.z);
        }

        if (rb.velocity.z > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSpeed);
        }
        else if (rb.velocity.z < -maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -maxSpeed);
        }

        // Jumping

        if (isJumping)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);

            if (Time.time - jumpStartTime >= jumpDuration)
            {
                isJumping = false;
            }
        }
        else
        {
            rb.AddForce(Vector3.down * -gravity * downwardsGravityMultiplier, ForceMode.Acceleration);
        }
    }

    void StartJump()
    {
        // Recalculate gravity and initial velocity in case they were changed in the inspector
        gravity = -2 * jumpHeightApex / (jumpDuration * jumpDuration);
        initialJumpVelocity = Mathf.Abs(gravity) * jumpDuration;

        timerUp = 0f;
        timerDown = 0f;

        isJumping = true;
        jumpStartTime = Time.time;
        rb.velocity = Vector3.up * initialJumpVelocity;
    }

    // void UpdateTimers()
    // {
    //     if (rb.velocity.y > 0f)
    //     {
    //         timerUp += Time.deltaTime;
    //     }
    //     else if (rb.velocity.y < 0f)
    //     {
    //         timerDown += Time.deltaTime;
    //     }

    //     timerTextUp.text = "Up: " + timerUp.ToString("F2");
    //     timerTextDown.text = "Down: " + timerDown.ToString("F2");
    // }
}