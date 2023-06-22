using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField]
    //private Rigidbody myBody;
    [SerializeField]
    private float jumpForce = 11f;
    [SerializeField]
    private float speedForce = 5;
    [SerializeField]
    private float addtlJump = 1000;

    private Rigidbody myRB;
    private bool isGrounded = true;
    private string GROUND_TAG = "Ground";
    private bool isMidAir = false;
    private float addtlJumpLeft = 0;


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        addtlJumpLeft = addtlJump;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerJump();
        PlayerMovement();
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown("space"))
        {
            if (isGrounded) {
                isGrounded = false;
                isMidAir = true;
                //myBody.AddForce(new Vector2(0f, jumpForce), ForceMode.Impulse); //works here also but originally from 2D
                myRB.velocity = new Vector3(0f, jumpForce, 0f); //for 3D
            } 
            else if (isMidAir)
            {
                if(addtlJumpLeft > 0)
                {
                    myRB.velocity = new Vector3(0f, jumpForce, 0f); //for 3D
                    addtlJumpLeft = addtlJumpLeft - 1;
                    Debug.Log("Jump left: " + addtlJumpLeft);
                }
                else
                {
                    isMidAir = false;
                    addtlJumpLeft = addtlJump;
                }
            }
        }
    }

    void PlayerMovement()
    {
        //
        if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
        {
            //myRB.velocity = new Vector3(0, 0, speedForce); //for 3D
            transform.position += Vector3.forward * speedForce * Time.deltaTime;
        } 
        if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            //myRB.velocity = new Vector3(0, 0, speedForce * -1); //for 3D
            transform.position += Vector3.back * speedForce * Time.deltaTime;
        }
        //
        if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
        {
            //myRB.velocity = new Vector3(speedForce, 0, 0); //for 3D
            transform.position += Vector3.right * speedForce * Time.deltaTime;
        }
        if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
        {
            //myRB.velocity = new Vector3(speedForce * -1, 0, 0); //for 3D
            transform.position += Vector3.left * speedForce * Time.deltaTime;
        }
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
        }
    }
}
