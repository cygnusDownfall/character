using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerGeneralInfo : SingletonNetwork<playerGeneralInfo>
{
    public GameObject dmgShowObj;
    public int cardPoint = 0;
    public byte maxCardPoint = 100;
    public string namePlayer;

}
