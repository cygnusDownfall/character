using UnityEngine;

[RequireComponent(typeof(enemyBehavier))]
public class dragonflyBehavier : MonoBehaviour, iEnemySPBehaviour
{
    enemyInfo info;
    enemyBehavier behavier;

    public string detail { get => "Mỗi lần tấn công sẽ tăng tốc độ di chuyển"; }
    private void Start()
    {
        info = GetComponent<enemyInfo>();
        behavier = GetComponent<enemyBehavier>();

        behavier.onAttack.AddListener(increaseSpeed);
    }
    public void increaseSpeed()
    {
        info.speed += 1;
    }

    private void OnDisable()
    {
        behavier.onAttack.RemoveListener(increaseSpeed);
    }
}
