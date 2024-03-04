using UnityEngine;

public class CircleRotator : MonoBehaviour
{
    public float rotationSpeed = -30.0f; // Degrees per second

    // Update is called once per frame
    void Update()
    {
        // Rotate around the z-axis at the given speed
        transform.Rotate(new Vector3(0, 0, 1) * rotationSpeed * Time.deltaTime);
    }
}
