using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class enemyInfo : characterInfo
{
    public enemyLevelSpecial enemyLevel = enemyLevelSpecial.elite;
    public Effect[] effectsWhenHit;
    public float rangeAttack = 1f;
    public DmgType nearAttackDmgType = DmgType.Physic;
    public float rangeAttackFar = 5f;
    public DmgType farAttackDmgType = DmgType.Dark;

    void loadInfo()
    {
        hp.Value = maxHP * (byte)enemyLevel;
        mp = maxMP;
        attack *= (byte)enemyLevel;
    }
    private void Start()
    {
        loadInfo();
    }
}
