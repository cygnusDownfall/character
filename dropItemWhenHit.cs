using UnityEngine;
[RequireComponent(typeof(characterInfo))]

public class dropItemWhenHit : MonoBehaviour
{
    characterInfo charInfo;
    public GameObject[] items;
    /// <summary>
    /// rate drop item in % unit 
    /// </summary>
    public static int rate = 5;
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
        if (Random.Range(0, 100) > rate) return;
        var rd = Random.Range(0, items.Length);

        var item = itemPooling.Instance.TakeOut(items[rd].name, transform.position, transform.rotation);
        if (item == default)
        {
            item = Instantiate(items[rd], transform.position, transform.rotation);
        }


    }
}
