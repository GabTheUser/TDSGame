using UnityEngine;
using UnityEngine.Events;

public class HPManager : MonoBehaviour
{
    public bool tank;
    public RegenManager regenManager;
    public int currentHp, currentShield, currentArmor, currentAntiExplosionArmor;

    [SerializeField] private UnityEvent deathEvent;
    [SerializeField] private bool disableIt, exterminate;

    public HPVarsSaver healthVarsSaver;

    private void Start()
    {
        GetStatsFromHPVariablesSaver();
    }   

    public void TakeDamage(int takenDamage, bool explosiveDamage)
    {
        if (currentShield > 0)
        {
            currentShield -= takenDamage;
            if (currentShield < 0)
            {
                takenDamage = Mathf.Abs(currentShield);
                currentShield = 0;
            }
        }
        if (currentShield <= 0)
        {
            if (explosiveDamage)
            {
                takenDamage -= currentAntiExplosionArmor;
            }
            else
            {
                takenDamage -= currentArmor;
            }
            if (takenDamage < 0)
            {
                takenDamage = 0;
            }
            currentHp -= takenDamage;
        }
        if (regenManager != null)
        {
            if (takenDamage > 0) regenManager.RegisterHit();
        }

        if (currentHp <= 0)
        {
            if (currentHp < 0) currentHp = 0;

            deathEvent?.Invoke();
            if (disableIt)
            {
                gameObject.SetActive(false);
            }
            if (exterminate)
            {
                Destroy(gameObject);
            }
        }
    }

    public void GetStatsFromHPVariablesSaver()
    {
        currentHp = healthVarsSaver.maxHp;
        currentArmor = healthVarsSaver.maxArmor;
        currentShield = healthVarsSaver.maxShield;
        currentAntiExplosionArmor = healthVarsSaver.maxAntiExplosionArmor;
    }
}
