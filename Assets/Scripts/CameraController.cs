using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float zoomoutSpeed;

    // Update is called once per frame
    void Update()
    {
        camera.orthographicSize += zoomoutSpeed * Time.deltaTime;
    }
}
