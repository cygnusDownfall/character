using UnityEngine;

[RequireComponent(typeof(enemyBehavier), typeof(clientNetworkTransform))]
public class dragonflyBehavier : MonoBehaviour, iEnemySPBehaviour
{
    [SerializeField] float speedIncreasePerAttack = 0.2f;

    //------Ref-----
    enemyInfo info;
    enemyBehavier behavier;

    private bool playerIsInRange;

    public string detail { get => "Mỗi lần tấn công sẽ tăng tốc độ di chuyển"; }
    #region mono
    private void Start()
    {
        info = GetComponent<enemyInfo>();
        behavier = GetComponent<enemyBehavier>();


        behavier.onAttack.AddListener(attackHandle);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != PlayerController.Instance.player) return;
        playerIsInRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != PlayerController.Instance.player) return;
        playerIsInRange = false;
    }
    private void OnDisable()
    {
        behavier.onAttack.RemoveListener(increaseSpeed);
    }
    #endregion
    public void attackHandle(bool attackMode)
    {
        if (!playerIsInRange) return;
        // player o trong pham vi 
        increaseSpeed();
        PlayerController.Instance.playerInfo.takeDamage(info.attack, attackMode ? info.farAttackDmgType : info.nearAttackDmgType);
    }

    public void increaseSpeed(bool _ = false)
    {
        info.speed += 1;
    }


}
