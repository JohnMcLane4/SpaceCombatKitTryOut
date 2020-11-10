using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour {

    public Transform shakenObject;

    public bool localSpace;

    public bool shakeXAxis = true;
    public bool shakeYAxis = true;
    public bool shakeZAxis = true;

    public float maxShakeVectorLength = 0.01f;

    bool shaken = false;

    public void Shake(float level)
    {

        if (Time.timeScale < 0.0001f)
        {
            if (localSpace)
            {
                shakenObject.localRotation = Quaternion.identity;
            }
            return;
        }

        // Do a single shake
        // Get a random vector on the xy plane
        Vector3 shakeVector = new Vector3(shakeXAxis ? UnityEngine.Random.Range(-1, 1) : 0, 
                                            shakeYAxis ? UnityEngine.Random.Range(-1, 1) : 0, 
                                            shakeZAxis ? UnityEngine.Random.Range(-1, 1) : 0).normalized;

        // Scale according to desired shake magnitude
        shakeVector *= level * maxShakeVectorLength;

        Vector3 lookDirection, upVector;
        if (localSpace)
        {
            // Look at shake vector
            lookDirection = (shakenObject.parent.TransformDirection(Vector3.forward).normalized + shakeVector).normalized;
            upVector = shakenObject.parent.TransformDirection(Vector3.up);
        }
        else
        {
            // Look at shake vector
            lookDirection = (shakenObject.forward + shakeVector).normalized;
            upVector = shakenObject.up;
            
        }
        
        shakenObject.rotation = Quaternion.LookRotation(lookDirection, upVector);

        shaken = true;

    }

    private void LateUpdate()
    {
        if (localSpace)
        {
            if (shaken)
            {
                shaken = false;
            }
            else
            {
                shakenObject.localRotation = Quaternion.identity;
            } 
        }
    }
}
