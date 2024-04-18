using UnityEngine;

public class RegenManager : MonoBehaviour
{
    public bool gotHit;
    public float maxTimeToWaitAfterHit, regenTimeShield, regenTimeHp;
    public int regenShield, regenHp;
    private HPManager healthManager;
    public HPVarsSaver healthVarsSaver;
    public RegenVarsSaver regenVarsSaver;

    private float curTimeToWaitAfterHit, timerHp, timerShield;
    private void Start()
    {
        healthManager = GetComponent<HPManager>();
        GetStatsFromRegenVarsSaver();
    }
    void Update()
    {
        if (gotHit)
        {
            if (curTimeToWaitAfterHit > 0)
            {
                curTimeToWaitAfterHit -= Time.deltaTime;
                if (curTimeToWaitAfterHit <= 0)
                {
                    gotHit = false;
                }
            }
        }
        else
        {
            if (healthManager.currentShield < healthVarsSaver.maxShield) // regen shield
            {
                timerShield += Time.deltaTime;
                if (timerShield >= regenTimeShield)
                {
                    timerShield = 0;
                    healthManager.currentShield += regenShield;
                }
            }
            else if (healthManager.currentShield > healthVarsSaver.maxShield)
            {
                healthManager.currentShield = healthVarsSaver.maxShield;
            }

            if (healthManager.currentHp < healthVarsSaver.maxHp) // regen hp
            {
                timerHp += Time.deltaTime;
                if (timerHp >= regenTimeHp)
                {
                    timerHp = 0;
                    healthManager.currentHp += regenHp;
                }
            }
            else if (healthManager.currentHp > healthVarsSaver.maxHp)
            {
                healthManager.currentHp = healthVarsSaver.maxHp;
            }

        }
    }

    public void RegisterHit()
    {
        timerHp += 0;
        timerShield += 0;
        curTimeToWaitAfterHit = maxTimeToWaitAfterHit;
        gotHit = true;
    }

    public void GetStatsFromRegenVarsSaver()
    {
        maxTimeToWaitAfterHit = regenVarsSaver.maxTimeToWaitAfterHit;
        regenTimeHp = regenVarsSaver.regenTimeHp;
        regenTimeShield = regenVarsSaver.regenTimeShield;
        regenHp = regenVarsSaver.regenHp;
        regenShield = regenVarsSaver.regenShield;
    }
}
