using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadCrumb : MonoBehaviour {

    private float disappearanceRate;
    private float currentRate = 0.0f;
    private bool rateSet = false;

    public void SetDisappearanceRate(float newDisappearanceRate)
    {
        disappearanceRate = newDisappearanceRate;
        rateSet = true;
        currentRate = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {


        currentRate += Time.deltaTime;

        if (rateSet && currentRate >= disappearanceRate)
        {
            Destroy(gameObject);
        }

	}
}
