using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerJoystickController : MonoBehaviour
{
    [SerializeField]
    private float speedForce = 5;
    [SerializeField]
    private VariableJoystick variableJoystick;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float jumpForce = 6f;
    [SerializeField]
    private float addtlJump = 1;
    [SerializeField]
    private float rotationSpeed = 1;

    private float movementX;
    private float movementY;
    private Animator anim;
    private string WALK_ANIMATION = "Walk";
    private string JUMP_ANIMATION = "Jump";

    private Rigidbody myRB;
    private bool isGrounded = true;
    private string GROUND_TAG = "Ground";
    private bool isMidAir = false;
    private float addtlJumpLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        WalkAnimation();
        PlayerMovement();
        PlayerJump();
    }

    /* working but player keeps sliding due to additional force
    public void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        rb.AddForce(direction * speedForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    */

    void PlayerMovement()
    {
        movementX = variableJoystick.Horizontal;
        movementY = variableJoystick.Vertical;

        Vector3 movementDirection = new Vector3(movementX, 0, movementY);
        movementDirection.Normalize();

        transform.Translate(movementDirection * speedForce * Time.deltaTime, Space.World);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }
    void PlayerJump()
    {
        //for jump
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            JumpAnimation(false);
            if (isGrounded)
            {
                isGrounded = false;
                isMidAir = true;
                //myBody.AddForce(new Vector2(0f, jumpForce), ForceMode.Impulse); //works here also but originally from 2D
                StartCoroutine(PlayerJumpMovement()); // myRB.velocity = new Vector3(0f, jumpForce, 0f); //for 3D

                JumpAnimation(true);
            }
            else if (isMidAir)
            {
                if (addtlJumpLeft > 0)
                {
                    myRB.velocity = new Vector3(0f, jumpForce, 0f); //for 3D
                    addtlJumpLeft = addtlJumpLeft - 1;
                    Debug.Log("Jump left: " + addtlJumpLeft);

                    JumpAnimation(true);
                }
                else
                {
                    isMidAir = false;
                    addtlJumpLeft = addtlJump;

                    JumpAnimation(false);
                }
            }
            else
            {
                JumpAnimation(false);
            }
        }
    }
    //Animations
    public void WalkAnimation()
    {
        movementX = variableJoystick.Horizontal;
        movementY = variableJoystick.Vertical;

        //Debug.Log("Horizontal: " + movementX + "| Vertical: " + movementY);
        if (movementX != 0)
        {
            anim.SetBool(WALK_ANIMATION, true);
        }
        else if (movementY != 0)
        {
            anim.SetBool(WALK_ANIMATION, true);
        }
        else
        {
            anim.SetBool(WALK_ANIMATION, false);
        }

    }
    private void JumpAnimation(bool jump)
    {
        //Debug.Log("isJump : " + jump);

        anim.SetBool(JUMP_ANIMATION, jump);
    }
    private IEnumerator PlayerJumpMovement()
    {
        yield return new WaitForSeconds(0.6f);
        myRB.velocity = new Vector3(0f, jumpForce, 0f);

        JumpAnimation(false);
    }


    /// Collisions Triggers
    /// 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            //Debug.Log("Landed on ground");
            isGrounded = true;
            isMidAir = false;
            JumpAnimation(false);
        }
    }
}
