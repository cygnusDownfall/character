using System;
using UnityEngine;

public class playerInfo : characterInfo
{
    #region BattleStatus
    public byte healScale = 100;//don vi %
    public byte recoveryMPScale = 100;//don vi %
    #endregion
    #region socialStatus
    public string namePlayer;

    public charJob job;
    public int cardPoint = 0;

    #endregion



    #region needed for skill access

    public override void healing(int heal)
    {
        hp.Value += Convert.ToInt32(heal * (healScale / 100f));
    }

    public override void addChain(Effect effect)
    {

    }
    public void removeChain(Effect effect)
    {
        chainEffect.Remove(effect);
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
    void Start()
    {
        load();
    }

    protected override void OnEnable()
    {
        hp.OnValueChanged += onHpChanged;

    }
    // override protected void OnDisable()
    // {
    //     base.OnDisable();
    //     hp.OnValueChanged -= onHpChanged;

    // }

    private void onHpChanged(int previousValue, int newValue)
    {

    }

    void Update()
    {
    }
    #endregion

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
        playerInfoData data = l.load<playerInfoData>("localplayerinfo");
        if (data == default)
        {
            return;
        }
        data.copyTo(this);
    }
    #endregion
}
