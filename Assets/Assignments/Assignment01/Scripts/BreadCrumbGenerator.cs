using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadCrumbGenerator : MonoBehaviour {

    [SerializeField] private GameObject crumbPrefab;
    [SerializeField][Range(0.1f,5.0f)] private float appearanceRate;
    [SerializeField] [Range(0.1f, 5.0f)] private float disappearanceRate;

    private float currentAppearanceRate = 0.0f;

    // Update is called once per frame
    void Update ()
    {
        currentAppearanceRate += Time.deltaTime;

        if (currentAppearanceRate > appearanceRate)
        {
            GameObject newCrumb = Instantiate(crumbPrefab, transform.position, Quaternion.identity);
            newCrumb.AddComponent<BreadCrumb>();
            newCrumb.GetComponent<BreadCrumb>().SetDisappearanceRate(disappearanceRate);
            currentAppearanceRate = 0.0f;
        }
	}
}
