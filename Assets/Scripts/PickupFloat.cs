using UnityEngine;

public class PickupFloat : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.08f;
    public float swaySpeed = 1.5f;
    public float swayAmount = 0.03f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        float newX = startPosition.x + Mathf.Cos(Time.time * swaySpeed) * swayAmount;

        transform.position = new Vector3(newX, newY, startPosition.z);
    }
}