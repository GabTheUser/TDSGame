using UnityEngine;
using UnityEngine.Events;

public enum EnemyState
{
    Idle,
    Patrol,
    Detect,
    Attack,
    Reload,
    Hide,
    LostPlayers
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState currentState = EnemyState.Idle;
    public UnityEvent idleEvent, patrolEvent, detectedPlayerEvent, attackEvent, reloadEvent, hideEvent, lostPlayers;

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.LostPlayers:
                lostPlayers?.Invoke();
                break;
            case EnemyState.Hide:
                hideEvent?.Invoke();
                break;
            case EnemyState.Reload:
                reloadEvent?.Invoke();
                break;
            case EnemyState.Attack:
                attackEvent?.Invoke();
                break;
            case EnemyState.Detect:
                detectedPlayerEvent?.Invoke();
                break;
            case EnemyState.Patrol:
                patrolEvent?.Invoke();
                break;
            case EnemyState.Idle:
                idleEvent?.Invoke();
                break;
        }
    }
}
