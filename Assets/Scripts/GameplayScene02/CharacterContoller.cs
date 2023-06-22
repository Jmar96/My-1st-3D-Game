using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterContoller : MonoBehaviour
{
	[SerializeField] float speed = 7.0f;
	[SerializeField] float airVelocity = 7f;
	[SerializeField] float gravity = 40.0f;
	[SerializeField] float maxVelocityChange = 10.0f;
	[SerializeField] float jumpHeight = 2.0f;
	[SerializeField] float maxFallSpeed = 20.0f;
	[SerializeField] float rotateSpeed = 25f; //Speed the player rotate
	[SerializeField] GameObject cam;
	[SerializeField] Vector3 checkPoint;
	[SerializeField] VariableJoystick variableJoystick;

	private Vector3 moveDir;
	private Rigidbody rb;

	private float distToGround;

	private bool canMove = true; //If player is not hitted
	private bool isStuned = false;
	private bool wasStuned = false; //If player was stunned before get stunned another time
	private float pushForce;
	private Vector3 pushDir;

	private bool slide = false;

	private Animator anim;
	private string WALK_ANIMATION = "Walk";

	// Start is called before the first frame update
	void Start()
	{
		// get the distance to ground
		distToGround = GetComponent<Collider>().bounds.extents.y;

		anim = GetComponent<Animator>();
	}

	bool IsGrounded()
	{
        float dtg = distToGround + 0.1f;
        //Debug.Log("Distance to ground : " + distToGround);
        //Debug.Log("IsGround() : " + Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f)
        //    + " |origin : " + transform.position
        //    + " |direction : " + -Vector3.up
        //    + " |maxDistance : " + dtg
        //    );
        return Physics.Raycast(transform.position, -Vector3.up, dtg);
	}

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;

		checkPoint = transform.position;
		Cursor.visible = false;
	}

	void FixedUpdate()
	{
		if (canMove)
		{
			if (moveDir.x != 0 || moveDir.z != 0)
			{
				Vector3 targetDir = moveDir; //Direction of the character

				targetDir.y = 0;
				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
				transform.rotation = targetRotation;
			}

			if (IsGrounded())
			{
				// Calculate how fast we should be moving
				Vector3 targetVelocity = moveDir;
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
				{
					targetVelocity = velocity;
					rb.velocity /= 1.1f;
				}
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				if (!slide)
				{
					if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
						rb.AddForce(velocityChange, ForceMode.VelocityChange);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					//Debug.Log(rb.velocity.magnitude);
				}

				// Jump
				//not working because Physics.Raycast always returns false, can't fix it
				//if (IsGrounded() && Input.GetButton("Jump"))
				//{
				//	rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				//}
			}
			else
			{
				if (!slide)
				{
					Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
					Vector3 velocity = rb.velocity;
					Vector3 velocityChange = (targetVelocity - velocity);
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					rb.AddForce(velocityChange, ForceMode.VelocityChange);
					if (velocity.y < -maxFallSpeed)
						rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
				}
			}
		}
		else
		{
			rb.velocity = pushDir * pushForce;
		}
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}

	private void Update()
	{
		PlayerMovement();
        //Debug.Log("Cal: " + CalculateJumpVerticalSpeed());
    }
	
	void PlayerMovement()
	{
		float hJoystick = variableJoystick.Horizontal;
		float vJoystick = variableJoystick.Vertical;
		float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

		if(hJoystick != 0)
        {
			h = hJoystick;
		}
		if (vJoystick != 0)
		{
			v = vJoystick;
		}

		PlayerWalkingAnimation(h, v);

		Vector3 v2 = v * cam.transform.forward; //Vertical axis to which I want to move with respect to the camera
		Vector3 h2 = h * cam.transform.right; //Horizontal axis to which I want to move with respect to the camera
		moveDir = (v2 + h2).normalized; //Global position to which I want to move in magnitude 1

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
		{
			if (hit.transform.tag == "Slide")
			{
				slide = true;
			}
			else
			{
				slide = false;
			}
		}
	}
	float CalculateJumpVerticalSpeed()
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void HitPlayer(Vector3 velocityF, float time)
	{
		rb.velocity = velocityF;

		pushForce = velocityF.magnitude;
		pushDir = Vector3.Normalize(velocityF);
		StartCoroutine(Decrease(velocityF.magnitude, time));
	}

	public void LoadCheckPoint()
	{
		transform.position = checkPoint;
	}

	private IEnumerator Decrease(float value, float duration)
	{
		if (isStuned)
		{
			wasStuned = true;
		}
		isStuned = true;
		canMove = false;

		float delta = 0;
		delta = value / duration;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			yield return null;
			if (!slide) //Reduce the force if the ground isnt slide
			{
				pushForce = pushForce - Time.deltaTime * delta;
				pushForce = pushForce < 0 ? 0 : pushForce;
				//Debug.Log(pushForce);
			}
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
		}

		if (wasStuned)
		{
			wasStuned = false;
		}
		else
		{
			isStuned = false;
			canMove = true;
		}
	}

	//Animations
	void PlayerWalkingAnimation(float h, float v)
    {
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
}
