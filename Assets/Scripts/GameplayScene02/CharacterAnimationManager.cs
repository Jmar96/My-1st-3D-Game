using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterAnimationManager : MonoBehaviour
{
    [SerializeField] VariableJoystick variableJoystick;
    [SerializeField] OnCollisionJump oCollJump;

    private Animator anim;
    private string WALK_ANIMATION = "Walk";
    private string JUMP_ANIMATION = "Jump";
    //private string GROUND_TAG = "Ground";
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        PlayerWalking();
        PlayerJumping();
    }

    void PlayerWalking()
    {
        float hJoystick = variableJoystick.Horizontal;
        float vJoystick = variableJoystick.Vertical;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (hJoystick != 0)
        {
            h = hJoystick;
        }
        if (vJoystick != 0)
        {
            v = vJoystick;
        }

        //Debug.Log("H: " + h + "| V: " + v);
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
    void PlayerJumping()
    {
        bool isGrounded = oCollJump.isGrounded;
        Debug.Log("isGround: " + isGrounded);
        if ((Input.GetKeyDown("space") || Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Jump")) && isGrounded)
        {
            anim.SetBool(JUMP_ANIMATION, true);
        }
        //else
        //{
        //    anim.SetBool(JUMP_ANIMATION, false);
        //}
        anim.SetBool(JUMP_ANIMATION, false);

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    anim.SetBool(JUMP_ANIMATION, false);

    //}

}
