using UnityEngine;

public class ExplosionEffectTimer : MonoBehaviour
{
    [SerializeField] private float timeToTurnOff;
    [SerializeField] private BulletBehaviour bulletBehaviour;
    private float currentTimeToTurnOff;
    private void OnEnable()
    {
        currentTimeToTurnOff = timeToTurnOff;
    }
    private void Update()
    {
        currentTimeToTurnOff -= Time.deltaTime;
        if(currentTimeToTurnOff < 0)
        {
            gameObject.SetActive(false);
            bulletBehaviour.TurnOffBullet();
        }
    }
}
