using UnityEngine;
public class OrbitFollowCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5.2f, height = 1.8f, sensitivity = .18f, minPitch = -18f, maxPitch = 62f;
    float yaw, pitch = 15f;
    Vector2 lookDelta;
    public void AddLook(Vector2 delta) { lookDelta += delta; }
    void LateUpdate()
    {
        if (!target) return;
        yaw += lookDelta.x * sensitivity;
        pitch = Mathf.Clamp(pitch - lookDelta.y * sensitivity, minPitch, maxPitch);
        lookDelta = Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
        yaw += Input.GetAxis("Mouse X") * 2.5f;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 2f, minPitch, maxPitch);
#endif
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 focus = target.position + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, focus - rot * Vector3.forward * distance, 16f * Time.deltaTime);
        transform.rotation = rot;
    }
}