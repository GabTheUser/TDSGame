using UnityEngine;
[CreateAssetMenu(fileName = "HPVarsSaver", menuName = "ScriptableObjects/HealthVarsSaver")]
public class HPVarsSaver : ScriptableObject
{
    public int maxHp, maxShield, maxArmor, maxAntiExplosionArmor;
}
