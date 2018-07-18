using UnityEngine;
using System.Collections;

public class SpinningClouds : MonoBehaviour
{
    public float speed = 0.05f;


    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}