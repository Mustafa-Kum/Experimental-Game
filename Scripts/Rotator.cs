using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 80f;

    void Update()
    {
        transform.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f), Space.World);
    }
}
