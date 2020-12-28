using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiTourne : MonoBehaviour
{
    public float angularSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(transform.forward, angularSpeed);
    }
}
