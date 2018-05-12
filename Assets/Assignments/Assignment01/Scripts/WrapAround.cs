using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAround : MonoBehaviour {

    private void OnTriggerExit(Collider other)
    {
        //A boid exited the trigger
        if(other.gameObject.tag == "Boid")
        {
            other.gameObject.transform.position = new Vector3(-other.gameObject.transform.position.x,other.transform.position.y, -other.gameObject.transform.position.z);
        }

    }
}
