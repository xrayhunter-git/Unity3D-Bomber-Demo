using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyController : MonoBehaviour {

    public GameObject com;
    private Rigidbody body;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();

        if (com != null)
            body.centerOfMass = com.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
