using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public enum ThirdPersonCameraType
    {
        Track,
        Follow,
        Follow_TrackRotation,
        Follow_IndependentRotation,
        TopDown
    }
    public ThirdPersonCameraType mThirdPersonCameraType =
      ThirdPersonCameraType.Track;
    public Transform mPlayer;

    public Vector3 mPositionOffset = new Vector3(0.0f, 2.0f, -2.5f);
    public Vector3 mAngleOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Tooltip("The damping factor to smooth the changes " +
      "in position and rotation of the camera.")]
    public float mDamping = 1.0f;

    [Header("Follow Independent Rotation")]
    public float mMinPitch = -30.0f;
    public float mMaxPitch = 30.0f;
    public float mRotationSpeed = 5.0f;
    private float angleX = 0.0f;

    void Start()
    {
    }

    void Update()
    {
    }

    void LateUpdate()
    {
        switch (mThirdPersonCameraType)
        {
            case ThirdPersonCameraType.Track:
                {
                    CameraMove_Track();
                    break;
                }
            case ThirdPersonCameraType.Follow:
                {
                    CameraMove_Follow();
                    break;
                }
            case ThirdPersonCameraType.Follow_TrackRotation:
                {
                    // refactored to not allow rotational tracking.
                    CameraMove_Follow(true);
                    break;
                }
            case ThirdPersonCameraType.Follow_IndependentRotation:
                {
                    // refactored to not allow rotational tracking.
                    Follow_IndependentRotation();
                    break;
                }
            case ThirdPersonCameraType.TopDown:
                {
                    CameraMove_TopDown();
                    break;
                }
        }
    }

    void CameraMove_Track()
    {
        Vector3 targetPos = mPlayer.transform.position;
        // We removed mPlayerHeight and replaced with the mPositionOffset.y
        targetPos.y += mPositionOffset.y;
        transform.LookAt(targetPos);
    }

    // Follow camera implementation.
    // Refactored to allow only positional tracking or 
    // positional tracking and rotational tracking. For this we
    // added a bool parameter called allowRotationTracking.
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

    void Follow_IndependentRotation()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        // We apply the initial rotation to the camera.
        Quaternion initialRotation = Quaternion.Euler(mAngleOffset);

        Vector3 eu = transform.rotation.eulerAngles;

        angleX -= my * mRotationSpeed;

        // We clamp the angle along the X axis to be between 
        // the min and max pitch.
        angleX = Mathf.Clamp(angleX, mMinPitch, mMaxPitch);

        eu.y += mx * mRotationSpeed;
        Quaternion newRot = Quaternion.Euler(angleX, eu.y, 0.0f) *
          initialRotation;

        transform.rotation = newRot;

        Vector3 forward = transform.rotation * Vector3.forward;
        Vector3 right = transform.rotation * Vector3.right;
        Vector3 up = transform.rotation * Vector3.up;

        Vector3 targetPos = mPlayer.position;
        Vector3 desiredPosition = targetPos
            + forward * mPositionOffset.z
            + right * mPositionOffset.x
            + up * mPositionOffset.y;

        Vector3 position = Vector3.Lerp(transform.position,
            desiredPosition,
            Time.deltaTime * mDamping);
        transform.position = position;
    }

    void CameraMove_TopDown()
    {
        // For topdown camera we do not use the x and z offsets.
        Vector3 targetPos = mPlayer.position;
        targetPos.y += mPositionOffset.y;
        Vector3 position = Vector3.Lerp(
          transform.position,
          targetPos,
          Time.deltaTime * mDamping);
        transform.position = position;

        transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }
}
