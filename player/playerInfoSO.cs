using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class playerInfoData
{
    public int hp;
    public int maxHP;
    public byte healScale = 100;//don vi %
    public byte mp;
    public byte maxMP;
    public byte recoveryMPScale = 100;//don vi %
    public int attack;
    public List<DmgType> defenseType;
    public List<byte> defenseValue;
    public byte speed = 1;

    public string namePlayer;
    public byte teamID;
    public charJob job;

    public void setInfo(playerInfo info)
    {
        hp = info.hp.Value;
        maxHP = info.maxHP;
        healScale = info.healScale;
        mp = info.mp;
        maxMP = info.maxMP;
        recoveryMPScale = info.recoveryMPScale;
        attack = info.attack;
        defenseType = info.defence.Keys.ToList<DmgType>();
        defenseValue = info.defence.Values.ToList<byte>();
        speed = info.speed;
        namePlayer = info.namePlayer;
        teamID = info.teamID;
        job = info.job;

    }
    public void copyTo(playerInfo info)
    {
        info.hp.Value = hp;
        info.healScale = healScale;
        info.maxHP = maxHP;
        info.mp = mp;
        info.maxMP = maxMP;
        info.recoveryMPScale = recoveryMPScale;
        info.attack = attack;
        var defense = new Dictionary<DmgType, byte>();
        for (int i = 0; i < defenseType.Count; i++)
        {
            defense.Add(defenseType[i], defenseValue[i]);
        }
        info.defence = defense;
        info.speed = speed;
        info.namePlayer = namePlayer;
        info.teamID = teamID;
        info.job = job;

    }
    public void save()
    {
        saveload save = new saveload();
        save.save("playerData", this);
    }
    public void load()
    {
        saveload load = new saveload();
        var res = load.load<playerInfoData>("playerData");
        if (res != null)
        {
            hp = res.hp;
            maxHP = res.maxHP;
            healScale = res.healScale;
            mp = res.mp;
            maxMP = res.maxMP;
            recoveryMPScale = res.recoveryMPScale;
            attack = res.attack;
            defenseType = res.defenseType;
            defenseValue = res.defenseValue;
            speed = res.speed;
            namePlayer = res.namePlayer;
            teamID = res.teamID;
            job = res.job;
        }
    }
}

