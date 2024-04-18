using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public EnemyAI enemyAI;
    public DefEnemyBehaviour defaultEnemyAi;
    public LayerMask detectingLayers;
    public float detectionRadius = 5f;
    public float attackRange = 4f;
    public float detectionAngle = 45f;
    public Transform player;

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectingLayers);
        bool foundPlayer = false; // Flag to track if the player is seen.

        foreach (Collider2D hit in hits)
        {
            Vector2 directionToPlayer = (hit.transform.position - transform.position).normalized;
            float angleToPlayer = Vector2.Angle(transform.up, directionToPlayer);

            if (angleToPlayer < detectionAngle * 0.5f)
            {
                // Check if there are no obstacles between the enemy and the player
                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, detectingLayers);

                if (raycastHit.collider != null && raycastHit.collider.CompareTag("Player"))
                {
                    player = hit.transform;
                    defaultEnemyAi.player = player;
                    foundPlayer = true;
                    break; // No need to continue checking if the player is found.
                }
            }
        }

        // Update the seePlayers flag based on whether the player is found.
        defaultEnemyAi.seePlayers = foundPlayer;

        // Check for attacking the player.
        if (defaultEnemyAi.seePlayers)
        {
            RaycastHit2D raycastHitAtk = Physics2D.Raycast(transform.position, transform.up, attackRange, detectingLayers);
            if (raycastHitAtk.collider != null && raycastHitAtk.collider.CompareTag("Player"))
            {
                defaultEnemyAi.attacksPlayers = true;
            }
            else
            {
                defaultEnemyAi.attacksPlayers = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 forward = transform.up;
        Vector3 right = Quaternion.AngleAxis(detectionAngle * 0.5f, Vector3.forward) * forward;
        Vector3 left = Quaternion.AngleAxis(-detectionAngle * 0.5f, Vector3.forward) * forward;

        Gizmos.color = Color.red;
        for (float i = -detectionAngle * 0.5f; i <= detectionAngle * 0.5f; i += 5f) // Adjust the increment as needed
        {
            Vector3 direction = Quaternion.AngleAxis(i, Vector3.forward) * transform.up;
            Gizmos.DrawRay(transform.position, direction * detectionRadius);
        }
    }
}
