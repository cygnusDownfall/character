using UnityEngine;
using UnityEngine.UI;

public class hpBarInMap : MonoBehaviour
{
    Slider UI;
    characterInfo info;
    public void syncValue(float value)
    {
        UI = UI != null ? UI : GetComponentInChildren<Slider>();
        UI.value = value;
        Debug.Log("UI hp bar in map :" + UI);
    }
    private void Start()
    {
        UI = GetComponentInChildren<Slider>();
        info = GetComponentInParent<characterInfo>();
        info.hp.OnValueChanged += (o, n) =>
        {
            Debug.Log("sync hp bar in map at:");
            syncValue(n * 1f / info.maxHP);
        };
    }
    public void OnDestroy()
    {

    }
}
