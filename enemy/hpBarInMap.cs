using UnityEngine;
using UnityEngine.UI;

public class hpBarInMap : MonoBehaviour
{
    Slider UI;
    public void syncValue(float value)
    {
        UI = UI != null ? UI : GetComponentInChildren<Slider>();
        UI.value = value;
    }
}
