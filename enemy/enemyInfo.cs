using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class enemyInfo : characterInfo
{
    public enemyLevelSpecial enemyLevel = enemyLevelSpecial.elite;
    public List<Effect> effectsWhenHit;
    public float rangeAttack = 1f;
    public DmgType nearAttackDmgType = DmgType.Physic;
    public float rangeAttackFar = 5f;
    public DmgType farAttackDmgType = DmgType.Dark;

    void loadInfo()
    {
        maxHP= maxHP * (byte)enemyLevel;
        hp.Value = maxHP;
        mp = maxMP;
        attack *= (byte)enemyLevel;
    }
    private void Start()
    {
        loadInfo();
    }
}
