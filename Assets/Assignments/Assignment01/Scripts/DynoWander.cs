using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynoWander : MonoBehaviour {

    //Holds the radius and forward offset of the wander circle. 
    public float wanderOffset;
    public float wanderRadius;

    //Holds the current orientation of the wander target
    public float wanderOrientation;

    //Holds the maximum rate at which the wander orientation can change 
    public float wanderRate;
    private Kinematic charRigidBody;
    private float targetOrientation;
    public Transform goal;

    private bool wanderState = true;

    public void setWanderState(bool newWanderState)
    {
        wanderState = newWanderState;
    }

    public bool getWanderState()
    {
        return wanderState;
    }

    public void Start()
    {
        charRigidBody = GetComponent<Kinematic>();
    }

    public void Update()
    {
        if (wanderState)
        {
            // Update the wander orientation
            wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;

            // Calculate the combined target orientation
            targetOrientation = wanderOrientation + charRigidBody.getOrientation();

            // Calculate the center of the wander circle 
            Vector3 orientationAsVector = new Vector3(-Mathf.Sin(charRigidBody.getOrientation()), 0.0f, Mathf.Cos(charRigidBody.getOrientation()));

            goal.position = transform.position + wanderOffset * orientationAsVector.normalized;

            //Calculate the target location
            goal.position += wanderRadius * new Vector3(-Mathf.Sin(targetOrientation), 0.0f, Mathf.Cos(targetOrientation)).normalized;
        }
        
    }
}
