using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rollAngle = transform.rotation.eulerAngles.z;

        Debug.Log("Roll Angle: " + rollAngle);
    }
}
