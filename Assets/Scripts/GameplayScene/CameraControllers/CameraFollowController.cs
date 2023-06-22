using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [SerializeField] Vector3 mPositionOffset = new Vector3(0.0f, 2.0f, -5f);
    [SerializeField] Vector3 mAngleOffset = new Vector3(0.0f, 0.0f, 0.0f);

    [Tooltip("The damping factor to smooth the changes in position and rotation of the camera.")]
    [SerializeField] float mDamping = 50.0f;

    [Header("Follow Independent Rotation")]
    [SerializeField] float mMinPitch = -30.0f;
    [SerializeField] float mMaxPitch = 30.0f;
    [SerializeField] float mRotationSpeed = 10.0f;
    [SerializeField] float angleX = 0.0f;
    [SerializeField] FixedTouchField mTouchField;

    Transform mPlayer;
    private Vector3 tempPos;
    // Start is called before the first frame update
    void Start()
    {
        mPlayer = GameObject.FindWithTag("TestPlayer2").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        //tempPos = transform.position;
        //tempPos.x = player.position.x;
        //tempPos.y = player.position.y;

        //CameraFollow();
        //Follow_IndependentRotation(); 
        CameraMove_Follow(true);
    }

    void CameraFollow()
    {
        transform.position = mPlayer.transform.position + new Vector3(0, 2, -5);
    }

    void Follow_IndependentRotation()
    {
        float mx, my;
        mx = mTouchField.TouchDist.x * Time.deltaTime;
        my = mTouchField.TouchDist.y * Time.deltaTime;

        // We apply the initial rotation to the camera.
        Quaternion initialRotation = Quaternion.Euler(mAngleOffset);

        Vector3 eu = transform.rotation.eulerAngles;

        angleX -= my * mRotationSpeed;

        // We clamp the angle along the X axis to be between 
        // the min and max pitch.
        angleX = Mathf.Clamp(angleX, mMinPitch, mMaxPitch);

        eu.y += mx * mRotationSpeed;
        Quaternion newRot = Quaternion.Euler(angleX, eu.y, 0.0f) * initialRotation;

        transform.rotation = newRot;

        Vector3 forward = transform.rotation * Vector3.forward;
        Vector3 right = transform.rotation * Vector3.right;
        Vector3 up = transform.rotation * Vector3.up;

        Vector3 targetPos = mPlayer.position;
        Vector3 desiredPosition = targetPos
            + forward * mPositionOffset.z
            + right * mPositionOffset.x
            + up * mPositionOffset.y;

        Vector3 position = Vector3.Lerp(transform.position,desiredPosition,Time.deltaTime * mDamping);

        transform.position = position;
    }
    void CameraMove_Follow(bool allowRotationTracking = false)
    {
        // We apply the initial rotation to the camera.
        Quaternion initialRotation = Quaternion.Euler(mAngleOffset);

        // added the following code to allow rotation tracking of the player
        // so that our camera rotates when the player rotates and at the same
        // time maintain the initial rotation offset.
        if (allowRotationTracking)
        {
            Quaternion rot = Quaternion.Lerp(transform.rotation,
                mPlayer.rotation * initialRotation,
                Time.deltaTime * mDamping);

            transform.rotation = rot;
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(
              transform.rotation,
              initialRotation,
              mDamping * Time.deltaTime);
        }

        // Now we calculate the camera transformed axes.
        Vector3 forward = transform.rotation * Vector3.forward;
        Vector3 right = transform.rotation * Vector3.right;
        Vector3 up = transform.rotation * Vector3.up;

        // We then calculate the offset in the 
        // camera's coordinate frame.
        Vector3 targetPos = mPlayer.position;
        Vector3 desiredPosition = targetPos
            + forward * mPositionOffset.z
            + right * mPositionOffset.x
            + up * mPositionOffset.y;

        // Finally, we change the position of the camera, 
        // not directly, but by applying Lerp.
        Vector3 position = Vector3.Lerp(transform.position,
            desiredPosition,
            Time.deltaTime * mDamping);

        transform.position = position;
    }
}
