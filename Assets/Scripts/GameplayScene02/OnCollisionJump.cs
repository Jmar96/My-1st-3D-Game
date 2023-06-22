using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class OnCollisionJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 15f;


    [HideInInspector] public bool isGrounded = true;
    Rigidbody myRB;


    private Animator anim;
    private string JUMP_ANIMATION = "Jump";

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        PlayerJump01();
    }
    void PlayerJump01()
    {
        if ((Input.GetKeyDown("space") || Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Jump")) && isGrounded)
        {
            //GetComponent<Rigidbody>().AddForce(transform.up *3, ForceMode.Impulse); //not working
            JumpAnimation(true); 
            isGrounded = false;
            StartCoroutine(DelayPlayerJump()); //myRB.velocity = new Vector3(0f, jumpForce, 0f);
        }
    }
    //Animations
    private void JumpAnimation(bool jump)
    {
        //Debug.Log("isJump : " + jump);
        anim.SetBool(JUMP_ANIMATION, jump);
    }

    private IEnumerator DelayPlayerJump()
    {
        yield return new WaitForSeconds(0.6f);
        myRB.velocity = new Vector3(0f, jumpForce, 0f);

        JumpAnimation(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        Debug.Log("isGrounded True");

    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        Debug.Log("isGrounded False");
    }

}
