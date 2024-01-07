using System;
using UnityEngine;
public class playerInfo : characterInfo
{
    #region BattleStatus
    public byte healScale = 100;//don vi %
    public byte recoveryMPScale = 100;//don vi %
    #endregion

    #region needed for skill access

    public override void healing(int heal)
    {
        hp.Value += Convert.ToInt32(heal * (healScale / 100f));
        if (hp.Value > maxHP) hp.Value = maxHP;
    }

    public override void addChain(Effect effect)
    {
        base.addChain(effect);

    }
    public virtual void gainMana(byte mana)
    {
        mp = (mp + mana > maxMP) ? maxMP : mana;
        manaBar.Instance.increaseMana(mana);
    }
    public virtual void lostmana(byte mana)
    {
        mp = (byte)((mp >= mana) ? (mp - mana) : 0);
        manaBar.Instance.decreaseMana(mana);
    }
    #endregion

    #region monobehavior
    protected override void OnEnable()
    {
        base.OnEnable();
        load();
        onDie.AddListener((info) =>
        {
            StartCoroutine(playerReLifePoint.current.ReSpawn());
        });
        hp.OnValueChanged += onHpChanged;

    }
    override protected void OnDisable()
    {
        base.OnDisable();
        hp.OnValueChanged -= onHpChanged;
    }

    private void onHpChanged(int previousValue, int newValue)
    {
        Debug.Log("hp changed:from " + previousValue + " to " + newValue);
        Debug.Log("hp change Owner of playerinfo is: " + OwnerClientId);
        if (!IsOwner) return;
        hpBar.Instance.Value = newValue / (float)maxHP;
    }
    #endregion

    public void resetStatus()
    {
        hp.Value = maxHP;
        gainMana((byte)(maxMP - mp));

    }

    #region saveload
    public void save()
    {
        saveload s = new saveload();
        playerInfoData data = new playerInfoData();
        data.setInfo(this);
        s.save("localplayerinfo", data);

    }
    public void load()
    {
        saveload l = new saveload();
        try
        {
            playerInfoData data = l.load<playerInfoData>("localplayerinfo");
            if (data == default)
            {
                return;
            }
            data.copyTo(this);
        }
        catch (Exception e)
        {
            Debug.Log("can not load from file " + e.Message);
            resetStatus();
        }

    }
    #endregion
}
