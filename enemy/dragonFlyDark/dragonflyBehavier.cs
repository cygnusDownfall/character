using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(enemyBehavier), typeof(NetworkTransform))]
public class dragonflyBehavier : MonoBehaviour, iEnemySPBehaviour
{
    public GameObject rangeObj;

    //------Ref-----
    skillObj rangeSkill;
    enemyInfo info;
    enemyBehavier behavier;

    private bool playerIsInRange;

    public string detail { get => "Mỗi lần tấn công sẽ tăng tốc độ di chuyển"; }
    private void Start()
    {
        info = GetComponent<enemyInfo>();
        behavier = GetComponent<enemyBehavier>();
        rangeSkill ??= rangeObj.GetComponent<skillObj>();
        rangeSkill.triggerEnter.AddListener((g1, g2) =>
        {
            if (g2 != PlayerController.Instance.player) return;
            playerIsInRange = true;

        });
        rangeSkill.triggerExit.AddListener((g1, g2) =>
        {
            if (g2 != PlayerController.Instance.player) return;
            playerIsInRange = false;

        }
        );

        behavier.onAttack.AddListener(attackHandle);
    }
    public void attackHandle(bool attackMode)
    {
        if (!playerIsInRange) return;
        increaseSpeed();
        PlayerController.Instance.playerInfo.takeDamage(info.attack, attackMode ? info.farAttackDmgType : info.nearAttackDmgType);
    }

    public void increaseSpeed(bool _ = false)
    {
        info.speed += 1;
    }

    private void OnDisable()
    {
        behavier.onAttack.RemoveListener(increaseSpeed);
    }
}
