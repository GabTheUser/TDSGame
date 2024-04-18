using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefEnemyBehaviour : MonoBehaviour
{
    public EnemyAI enemyAI;
    public Transform player;
    public Transform[] patrolPoints;
    public NavMeshAgent agent;
    [Header("Look Around")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float clockwiseLookTimeLimit = 2f;
    private float counterclockwiseLookTimeLimit;
    [SerializeField] private float clockwisePauseTime = 1f;
    [Header("Chasers")]
    public float timeToChase;
    private float currentTimeToChase;
    [Space(10)]
    [Header("Switchers")]
    public bool lookAroundOnArrival,
        goPatrol,
        seePlayers,
        attacksPlayers,
        lostPlayers;

    private int currentPatrolIndex, sideLooked;
    private float lookTimer;
    private bool isLookingClockwise;
    private bool isPaused;
    private float pauseTimer;
    Vector3 nextPatrolPoint;
    private void Start()
    {
        counterclockwiseLookTimeLimit = clockwiseLookTimeLimit / 2;
        agent.updateUpAxis = false;
        agent.updateRotation = false;
    }

    public void PatrolingState()
    {
        if (seePlayers)
        {
            enemyAI.currentState = EnemyState.Detect;
            return;
        }
        if (!goPatrol)
        {
            enemyAI.currentState = EnemyState.Idle;
        }
        agent.SetDestination(nextPatrolPoint);
        FaceDirectionOfMovement();
        if (agent.remainingDistance < 0.1f)
        {
            if (lookAroundOnArrival)
            {
                LookAroundAfterArriving();
            }
        }
    }

    public void IdleState()
    {
        if (seePlayers)
        {
            enemyAI.currentState = EnemyState.Detect;
            return;
        }
        if (goPatrol)
        {
            enemyAI.currentState = EnemyState.Patrol;
        }
        FaceDirectionOfMovement();
    }

    public void ChasePlayer()
    {
        if(!attacksPlayers & !seePlayers)
        {
            currentTimeToChase += Time.deltaTime;
            if(currentTimeToChase >= timeToChase)
            {
                currentTimeToChase = 0;
                player = null;
                enemyAI.currentState = EnemyState.LostPlayers;
                return;
            }
        }
        if(attacksPlayers)
        {
            enemyAI.currentState = EnemyState.Attack;
            currentTimeToChase = 0;
            return;
        }
        FaceDirectionOfMovement();
        agent.SetDestination(player.position);
    }

    public void AttackPlayer()
    {
        if (!attacksPlayers || (!seePlayers && !attacksPlayers))
        {
            enemyAI.currentState = EnemyState.Detect;
            return;
        }
        Debug.Log("attackin");
        agent.SetDestination(transform.position);
        LookAtPlayers();
    }

    public void LostPlayers()
    {
        if (seePlayers)
        {
            enemyAI.currentState = EnemyState.Detect;
            return;
        }
        FaceDirectionOfMovement();
        LookAroundAfterArriving();
    }

    private void SetDestinationToNextPatrolPoint()
    {
        // Move to the next patrol point
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        nextPatrolPoint = patrolPoints[currentPatrolIndex].position;
        PatrolingState();
    }

    private void FaceDirectionOfMovement()
    {
        // Get the velocity of the NavMeshAgent
        Vector3 velocity = agent.velocity;

        // If the velocity is not close to Vector3.zero, rotate towards it
        if (velocity != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void LookAtPlayers()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f)); // -90f is often needed for top-down sprites.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void LookAroundAfterArriving()
    {
        if (!isPaused)
        {
            lookTimer += Time.deltaTime;

            // Determine the direction based on the timer
            float direction = (isLookingClockwise) ? 1f : -1f;

            // Look in the determined direction
            transform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);

            // Reset timer when looking is complete
            float lookTimeLimit = (isLookingClockwise) ? clockwiseLookTimeLimit : counterclockwiseLookTimeLimit;
            if (lookTimer > lookTimeLimit)
            {
                lookTimer = 0f;
                isPaused = true; // Start pause after rotation
                pauseTimer = 0f; // Reset pause timer
            }
        }
        else
        {
            // Increment the pause timer
            pauseTimer += Time.deltaTime;

            // Check if the pause is complete
            //float pauseTime = (isLookingClockwise) ? clockwisePauseTime : counterclockwisePauseTime;
            if (pauseTimer >= clockwisePauseTime)
            {
                sideLooked++;
                pauseTimer = 0f;
                isPaused = false; // Resume rotation
                isLookingClockwise = !isLookingClockwise; // Change rotation direction
                if (sideLooked >= 2)
                {
                    sideLooked = 0;
                    SetDestinationToNextPatrolPoint();
                }
            }
        }
    }
}
