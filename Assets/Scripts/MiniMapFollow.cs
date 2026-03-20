using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;
    public float fixedZ = -10f;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            fixedZ
        );
    }
}