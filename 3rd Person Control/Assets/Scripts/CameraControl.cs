using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    #region INIT STUFF
    public bool debugLines = true;
    public bool isColliding = false;

    public float distanceFromTarget = -8f;
    public float rotMin = 0.0f;
    public float rotMax = 30.0f;

    public LayerMask collisionLayer;

    private Vector3[] adjustedClipPoints;
    private Vector3[] desiredClipPoints;

    private float collisionSpaceSize = 3.41f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private Camera camera;
    private CharacterControl player;

    private Vector3 targetPos = Vector3.zero;
    private Vector3 targetOffSet = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 destination = Vector3.zero;
    private Vector3 adjustedDestination = Vector3.zero;
    #endregion

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();
        camera = Camera.main;
        adjustedClipPoints = new Vector3[5];
        desiredClipPoints = new Vector3[5];

        UpdateClipPoints(this.transform.position, this.transform.rotation, ref adjustedClipPoints);
        UpdateClipPoints(destination, this.transform.rotation, ref desiredClipPoints);
	}

	void Update () {

        xRotation -= Input.GetAxis("Mouse Y");
        yRotation += Input.GetAxis("Mouse X");
        xRotation = Mathf.Clamp(xRotation, rotMin, rotMax);

        targetPos = player.transform.position + targetOffSet;

        destination = Quaternion.Euler(xRotation, yRotation, 0.0f) * Vector3.forward * distanceFromTarget;
        destination += targetPos;

        if (CollisionDetected(desiredClipPoints, targetPos))
        {
            adjustedDestination = Quaternion.Euler(xRotation, player.yRotation, 0.0f) * -Vector3.forward * GetAdjustedDistance(player.transform.position);
            adjustedDestination += targetPos;
            this.transform.position = adjustedDestination;
        }
        else
        {
            this.transform.position = destination;
        }
        this.transform.LookAt(targetPos);
	}

    void FixedUpdate()
    {
        UpdateClipPoints(camera.transform.position, this.transform.rotation, ref adjustedClipPoints);
        UpdateClipPoints(destination, this.transform.rotation, ref desiredClipPoints);

        // Draws Debug Lines.
        if (debugLines)
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.DrawLine(targetPos, desiredClipPoints[i], Color.white);
                Debug.DrawLine(targetPos, adjustedClipPoints[i], Color.green);
            }
        }
    }
    // --- <Summary Begin> ---
    // Checks for Collision for each clipping point
    // --- <Summary End> ---
    bool CollisionDetected(Vector3[] clipPoint, Vector3 position)
    {
        for (int i = 0; i < clipPoint.Length; i++)
        {
            Ray ray = new Ray(position, clipPoint[i] - position);
            float distance = Vector3.Distance(clipPoint[i], position);
            if (Physics.Raycast(ray, distance, collisionLayer))
            {
                return true;
            }
        }
        return false;
    }

    // --- <Summary Begin> ---
    // Updates the Clipping Points to proper location of the camera.
    // --- <Summary End> ---
    public void UpdateClipPoints(Vector3 position, Quaternion rotation, ref Vector3[] intoArray)
    {
        if (camera == null)
        {
            return;
        }
        else
        {
            intoArray = new Vector3[5];

            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / collisionSpaceSize) * z;
            float y = x / camera.aspect;
            // Top Left
            intoArray[0] = (rotation * new Vector3(-x, y, z)) + position;
            // Top Right
            intoArray[1] = (rotation * new Vector3(x, y, z)) + position;
            // Bottom Left
            intoArray[2] = (rotation * new Vector3(-x, -y, z)) + position;
            // Bottom Left
            intoArray[3] = (rotation * new Vector3(x, -y, z)) + position;
            // Camera Position
            intoArray[4] = camera.transform.position;
        }
    }

    // --- <Summary Begin> ---
    // Finds the shortest distance, for new location of camera and clipping points.
    // --- <Summary End> ---
    public float GetAdjustedDistance(Vector3 position)
    {
        float distance = -1f;

        for (int i = 0; i < desiredClipPoints.Length; i++)
        {
            Ray ray = new Ray(position, desiredClipPoints[i] - position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                {
                    distance = hit.distance;
                }
                else
                {
                    if (hit.distance < distance)
                    {
                        distance = hit.distance;
                    }
                }
            }
        }

        if (distance == -1f)
        {
            return 0f;
        }
        else
        {
            return distance;
        }
    }
}