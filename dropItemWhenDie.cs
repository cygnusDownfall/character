using UnityEngine;
[RequireComponent(typeof(characterInfo))]
public class dropItemWhenDie : MonoBehaviour
{
    characterInfo charInfo;
    public GameObject[] items;
    void Start()
    {
        charInfo = GetComponent<characterInfo>();
    }
    private void OnEnable()
    {
        charInfo.onDie.AddListener(dropItem);
    }
    public void dropItem(characterInfo _)
    {
        foreach (var item in items)
        {
            if (itemPooling.Instance.TakeOut(item.name, transform.position, transform.rotation) != default) continue;
            Instantiate(item, transform.position, transform.rotation);
        }
    }
}
