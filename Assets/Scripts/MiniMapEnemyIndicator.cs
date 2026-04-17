using UnityEngine;

public class MiniMapEnemyIndicator : MonoBehaviour
{
    public Transform player;
    public Transform enemiesParent;
    public RectTransform minimapArea;
    public RectTransform enemyIndicator;

    [Header("Indicator Position")]
    public float distanceFromCenter = 35f;
    public float edgePadding = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 360f;
    public float angleOffset = 0f;

    private void Update()
    {
        if (player == null || enemiesParent == null || minimapArea == null || enemyIndicator == null)
            return;

        Transform closestEnemy = GetClosestEnemy();

        if (closestEnemy == null)
        {
            enemyIndicator.gameObject.SetActive(false);
            return;
        }

        enemyIndicator.gameObject.SetActive(true);

        Vector2 direction = (closestEnemy.position - player.position);

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        Vector2 targetPosition = direction * distanceFromCenter;

        float halfWidth = minimapArea.rect.width * 0.5f - edgePadding;
        float halfHeight = minimapArea.rect.height * 0.5f - edgePadding;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -halfWidth, halfWidth);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -halfHeight, halfHeight);

        enemyIndicator.anchoredPosition = targetPosition;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        enemyIndicator.localRotation = Quaternion.RotateTowards(
            enemyIndicator.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    Transform GetClosestEnemy()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        Transform[] allChildren = enemiesParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child == enemiesParent)
                continue;

            if (!child.gameObject.activeInHierarchy)
                continue;

            if (!child.CompareTag("Enemies"))
                continue;

            float distance = Vector2.Distance(player.position, child.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = child;
            }
        }

        return closest;
    }
}