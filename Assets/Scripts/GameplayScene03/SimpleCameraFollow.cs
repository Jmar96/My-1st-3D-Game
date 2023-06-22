using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    Transform mPlayer;
    // Start is called before the first frame update
    void Start()
    {

        mPlayer = GameObject.FindWithTag("TestPlayer3").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        CameraFollow();
    }
    void CameraFollow()
    {
        transform.position = mPlayer.transform.position + new Vector3(0, 2, -5);
    }
}
