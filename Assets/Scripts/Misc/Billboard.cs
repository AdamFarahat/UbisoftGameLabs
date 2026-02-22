using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    public Vector3 cameraOffset;

    void Update()
    {
        transform.forward = Camera.main.transform.forward;
        Transform cam = Camera.main.transform;
        transform.localPosition = cam.forward * cameraOffset.z + cam.right * cameraOffset.x + cam.up * cameraOffset.y;
    }
}
