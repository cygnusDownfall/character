using UnityEngine;
[RequireComponent(typeof(characterInfo))]

public class dropItemWhenHit : MonoBehaviour
{
    characterInfo charInfo;
    public GameObject[] items;
    void Start()
    {
        charInfo = GetComponent<characterInfo>();
    }
    private void OnEnable()
    {
        charInfo.onAttacked.AddListener(dropItem);
    }
    public void dropItem(characterInfo _)
    {
        var rd = Random.Range(0, items.Length);

        var item = itemDropPooling.Instance.TakeOut(items[rd].name, transform.position, transform.rotation);
        if (item == default)
        {
            item = Instantiate(items[rd], transform.position, transform.rotation);
        }


    }
}
