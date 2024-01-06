using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : NetworkBehaviour
{
    /// <summary>
    /// value from 0 to 1
    /// </summary>
    public float Value
    {
        get => GetComponent<Image>().material.GetFloat("_value");
        set
        {
            if (value > 1) Value = 1;
            if (value < 0) Value = 0;
            GetComponent<Image>().material.SetFloat("_value", value);
        }
    }

}
