using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public CharacterController mCharacterController;
    //public Animator mAnimator;
    [SerializeField] VariableJoystick variableJoystick;

    [SerializeField] float mWalkSpeed = 5.5f;
    [SerializeField] float mRunSpeed = 10.0f;
    [SerializeField] float mRotationSpeed = 200.0f;
    //[SerializeField] float mGravity = -20.0f;

    [SerializeField] float jumpForce = 6f;
    [SerializeField] float addtlJump = 1;

    //private Vector3 mVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Animator anim;

    float h;
    float v;
    string WALK_ANIMATION = "Walk";

    string JUMP_ANIMATION = "Jump";
    private Rigidbody myRB;
    private bool isGrounded = true;
    private string GROUND_TAG = "Ground";
    private bool isMidAir = false;
    private float addtlJumpLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        WalkAnimation();
        PlayerJump();
    }

    public void Move()
    {
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        h = variableJoystick.Horizontal;
        v = variableJoystick.Vertical;

        float speed = mWalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = mRunSpeed;
        }

        mCharacterController.Move(transform.forward * v * speed * Time.deltaTime);
        transform.Rotate(0.0f, h * mRotationSpeed * Time.deltaTime, 0.0f);
        //if (mAnimator != null)
        //{
        //    mAnimator.SetFloat("PosZ", v * speed / mRunSpeed);
        //}

        //// apply gravity.
        //mVelocity.y += mGravity * Time.deltaTime;
        //mCharacterController.Move(mVelocity * Time.deltaTime);

        //if (mCharacterController.isGrounded && mVelocity.y < 0)
        //{
        //    mVelocity.y = 0f;
        //}
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
                StartCoroutine(PlayerJumpMovement());

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
        h = variableJoystick.Horizontal;
        v = variableJoystick.Vertical;

        //Debug.Log("Horizontal: " + movementX + "| Vertical: " + movementY);
        if (h != 0)
        {
            anim.SetBool(WALK_ANIMATION, true);
        }
        else if (v != 0)
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
            Debug.Log("Landed on ground");
            isGrounded = true;
            isMidAir = false;
            JumpAnimation(false);
        }
    }
}
