using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour {

    private CameraControl cam;

    private float speed = 5.45f;
    private float crossLimit = 0.689f;
    [HideInInspector]
    public  float yRotation;

    private Vector3 dir;
    private Rigidbody rb;

	void Start () {
        rb = GetComponent<Rigidbody>();
	}

	void Update () {
        // Movement
        dir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        UpdatedDirection(dir);
        rb.transform.Translate(dir * speed * Time.deltaTime);
        // Y Rotation for character and camera
        yRotation += Input.GetAxis("Mouse X");
        this.transform.rotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
	}

    // --- <Summary Begin> ---
    // Fixes increased speed, by having more than one vector greater or smaller than 0.
    // --- <Summary End> ---
    void UpdatedDirection(Vector3 vec)
    {
        if (vec.x >= crossLimit && vec.z >= crossLimit)
        {
            vec.x = crossLimit;
            vec.z = crossLimit;
        }
        else if (vec.x >= crossLimit && vec.z <= -crossLimit)
        {
            vec.x = crossLimit;
            vec.z = -crossLimit;
        }
        else if (vec.x <= -crossLimit && vec.z >= crossLimit)
        {
            vec.x = -crossLimit;
            vec.z = crossLimit;
        }
        else if (vec.x <= -crossLimit && vec.z <= -crossLimit)
        {
            vec.x = -crossLimit;
            vec.z = -crossLimit;
        }
    }
}
