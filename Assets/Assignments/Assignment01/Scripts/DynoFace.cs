using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynoFace : MonoBehaviour
{
    public Transform goal;
    private DynoSteering ds;
    private DynoAlign align;
    private SteeringParams sp;
    private Kinematic charRigidBody;
    private float targetOrientation;
    private Vector3 target;

    // Use this for initialization
    void Start()
    {
        sp = GetComponent<SteeringParams>();
        charRigidBody = GetComponent<Kinematic>();
        align = GetComponent<DynoAlign>();
        goal = GetComponent<GoalNamespace.Goal>().getGoal();
    }

    public DynoSteering getSteering()
    {
        Vector3 direction = goal.position - transform.position;

        if (direction.magnitude != 0.0f)
        {
            align.goal = goal;
            align.targetOrientation = Mathf.Atan2(-direction.x, direction.z);
        }
        

        return align.getSteering();
    }
}
