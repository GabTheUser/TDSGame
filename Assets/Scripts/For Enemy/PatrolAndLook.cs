using UnityEngine.AI;
using UnityEngine;

public class PatrolAndLook : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;
    public NavMeshAgent agent;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float clockwiseLookTimeLimit = 2f;
    private float counterclockwiseLookTimeLimit;

    [SerializeField] private float clockwisePauseTime = 1f;

    private int currentPatrolIndex, sideLooked;
    private float lookTimer;
    private bool isLookingClockwise;
    private bool isPaused;
    private float pauseTimer;

    private void Start()
    {
        counterclockwiseLookTimeLimit = clockwiseLookTimeLimit / 2;
        agent.updateUpAxis = false;
        agent.updateRotation = false;
    }

    private void Update()
    {
        LookAroundAfterArriving();
        FaceDirectionOfMovement();
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

    private void SetDestinationToNextPatrolPoint()
    {
        // Move to the next patrol point
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        Vector3 nextPatrolPoint = patrolPoints[currentPatrolIndex].position;
        agent.SetDestination(nextPatrolPoint);
    }

    private void LookAroundAfterArriving()
    {
        if (agent.remainingDistance < 0.1f)
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
                    if(sideLooked >= 2)
                    {
                        sideLooked=0;
                        SetDestinationToNextPatrolPoint();
                    }
                }
            }
        }
    }
}