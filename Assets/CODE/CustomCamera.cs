using UnityEngine;

// CustomCamera.cs
// Pengganti file lama yang bernama `Camera` untuk menghindari konflik nama
public class CustomCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Camera hanya maju (tidak mundur ke kiri)
        if (desiredPosition.x < transform.position.x)
            desiredPosition.x = transform.position.x;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
