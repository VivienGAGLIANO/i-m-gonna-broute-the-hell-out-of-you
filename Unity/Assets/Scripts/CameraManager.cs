using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform playerTransform;
    public Transform chevalTranform;

    private Camera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    public float distMax = 15f;
    public float distMin = 15f;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        thisCamera.orthographicSize = Mathf.Max(distMin, Mathf.Min(distMax, Vector2.Distance(playerTransform.position, chevalTranform.position)));
    }
}
