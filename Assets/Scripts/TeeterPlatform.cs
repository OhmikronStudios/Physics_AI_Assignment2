using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeeterPlatform : MonoBehaviour
{
    //Rotation Parameters
    Rigidbody rb;
    float startingZRotation;
    float targetZRotation = 0;
    float maxRotSpeed = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startingZRotation = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        Stabilize();
    }

    public void Rotate(Vector3 distanceFromCenter)
    {
        Debug.Log("rotation started");
        float rotSpeed = maxRotSpeed * Vector3.Distance(distanceFromCenter, transform.position);
        targetZRotation += rotSpeed;
        rb.MoveRotation(new Quaternion(0,0,targetZRotation,rotSpeed).normalized);
    }
    
    public void Stabilize()
    {
        Debug.Log("stabilizing started");
        
        rb.MoveRotation(new Quaternion(0,0,startingZRotation,maxRotSpeed/10).normalized);
    }


    



}
