using System.Collections.Generic;

public class enemyInfo : characterInfo
{
    public enemyLevelSpecial enemyLevel;
    public List<Effect> effectsWhenHit;
    public float rangeAttack = 1f;
    public DmgType nearAttackDmgType = DmgType.Physic;
    public float rangeAttackFar = 5f;
    public DmgType farAttackDmgType = DmgType.Dark;

    void loadInfo()
    {
        hp = new Unity.Netcode.NetworkVariable<int>(maxHP * (int)enemyLevel);
        mp = maxMP;
        attack *= (int)enemyLevel;
    }
    public override void takeDamage(int dmg, DmgType dmgType)
    {
        base.takeDamage(dmg, dmgType);
    }

    private void Start()
    {
        loadInfo();
    }
}
