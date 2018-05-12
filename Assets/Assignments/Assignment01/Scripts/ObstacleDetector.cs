using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour {

    [SerializeField] private DynoWander wander;
    [SerializeField] private GoalNamespace.GoalSwitcher goalSwitcher;
    [SerializeField] private GameObject safeZone1;
    [SerializeField] private GameObject safeZone2;
    [SerializeField] private GameObject dummyObject;

    private float maxTimeSinceLeftWander = 1.0f;
    private float currentTimeOutOfWander = 0.0f;

    public enum AvoidanceStrategies
    {
        BounceOff,
        SafeZoneNavigation
    };

    public AvoidanceStrategies AvoidanceStrategy;

    private void Update()
    {
        if (!wander.getWanderState())
        {
            currentTimeOutOfWander += Time.deltaTime;

            if (currentTimeOutOfWander > maxTimeSinceLeftWander)
            {
                goalSwitcher.SetGoal(GetComponent<DynoWander>().goal.gameObject);
                wander.setWanderState(true);
                currentTimeOutOfWander = 0.0f;
            }
        }
        
    }

    void FixedUpdate()
    {
        Ray ray = new Ray();
        RaycastHit hit;

        ray.origin = transform.position;
        ray.direction = transform.right;

        if (Physics.Raycast(ray, out hit, 3.0f, LayerMask.GetMask("Obstacle")))
        {
            //Debug.Log("Found wall!");

            GameObject obstacle = hit.transform.gameObject;

            //Go to safe zone
            wander.setWanderState(false);

            if (AvoidanceStrategy == AvoidanceStrategies.SafeZoneNavigation)
            {
                //Do dot product to see on which side of the wall the boid is on
                goalSwitcher.SetGoal(SafeZone(obstacle));
            }
            else if (AvoidanceStrategy == AvoidanceStrategies.BounceOff)
            {
                //Get the reflection
                Reflection(hit);
            }
            

        }
    }

    private GameObject SafeZone(GameObject obstacle)
    {
        //Debug.Log(Vector3.Dot(obstacle.transform.forward, transform.right));

        if (Vector3.Dot(obstacle.transform.forward, transform.right) < 0.0f)
        {
            return safeZone1;
        }
        else
        {
            return safeZone2;
        }

    }
    private void Reflection(RaycastHit hit)
    {
        Vector3 incomingVec = hit.point - transform.position;
        Vector3 reflectVec = Vector3.Reflect(incomingVec.normalized, hit.normal);

        dummyObject.transform.position = hit.point + reflectVec.normalized * 10.0f;

        goalSwitcher.SetGoal(dummyObject);
    }


}
