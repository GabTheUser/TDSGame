using UnityEngine;
using UnityEngine.Events;

public class ProjectileMovement : MonoBehaviour
{
    public float maxSpeed, curSpeed;
    public float rotationSpeed, maxTimeToTurn;

    [Header("Behavior is null if default!")]
    [SerializeField] private UnityEvent projBehaviour;

    [SerializeField] private Transform rotateCenter;

    private float curTimeToTurn, maxTimeToTurnB;

    private void OnEnable()
    {
        curSpeed = maxSpeed;
        maxTimeToTurn /= 2;
        maxTimeToTurnB = maxTimeToTurn * 2;
    }

    private void Update()
    {
        MoveProjectile();
        projBehaviour?.Invoke();
    }

    public void WaveBullet()
    {
        if (curTimeToTurn < maxTimeToTurn)
        {
            curTimeToTurn += Time.deltaTime;
            RotateProjectile();
        }
        else
        {
            ResetRotationParameters();
        }
    }

    public void SpinningBullet()
    {
        rotateCenter.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        CheckChildCountAndDestroy();
    }

    public void DestroyBullet()
    {
        gameObject.SetActive(false);
    }

    private void MoveProjectile()
    {
        transform.Translate(curSpeed * Time.deltaTime * Vector2.up);
    }

    private void RotateProjectile()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void ResetRotationParameters()
    {
        curTimeToTurn = 0;
        rotationSpeed *= -1;
        maxTimeToTurn = maxTimeToTurnB;
    }

    private void CheckChildCountAndDestroy()
    {
        if (rotateCenter.childCount == 0)
        {
            DestroyBullet();
        }
    }
}
