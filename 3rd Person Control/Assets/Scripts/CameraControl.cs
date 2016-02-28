using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    private float xRotation;

	void Start () {
	
	}

	void Update () {
        xRotation += Input.GetAxis("Vertical");
        xRotation = Mathf.Clamp(xRotation, 0.0f, 90.0f);
	}
}
