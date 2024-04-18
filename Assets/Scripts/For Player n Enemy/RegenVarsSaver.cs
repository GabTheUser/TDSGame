using UnityEngine;
[CreateAssetMenu(fileName = "RegenVarsSaver", menuName = "ScriptableObjects/RegenVarsSaver")]
public class RegenVarsSaver : ScriptableObject
{
    public float maxTimeToWaitAfterHit, regenTimeShield, regenTimeHp;
    public int regenShield, regenHp;
}
