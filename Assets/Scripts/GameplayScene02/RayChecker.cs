using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayChecker : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(transform.position, transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForColliders();
        RaycastCHeck();
        RaycastCheck2();
    }
    void CheckForColliders()
    {
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject.name + " was hit1!");
        }
    }
    void RaycastCHeck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.0f))
        {
            Debug.Log("was hit2!");
        }
    }
    void RaycastCheck2()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log("Hit3");
        }

        Debug.Log("unHit3");
    }
}
