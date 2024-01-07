using System.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[RequireComponent(typeof(enemyBehavier), typeof(clientNetworkTransform))]
public class dragonflyBehavier : MonoBehaviour, iEnemySPBehaviour
{
    [SerializeField] float speedIncreasePerAttack = 0.2f;

    //------Ref-----
    enemyInfo info;
    enemyBehavier behavier;
    public BitArray m_field = new BitArray(2);
    public bool isPlayerAlly;
    public bool playerIsInRange
    {
        get => m_field.Get(0);
        set
        {
            m_field.Set(0, value);
        }
    }

    public string detail { get => "Mỗi lần tấn công sẽ tăng tốc độ di chuyển"; }
    #region mono
    private void Start()
    {
        info = GetComponent<enemyInfo>();
        behavier = GetComponent<enemyBehavier>();
        if (isPlayerAlly && NetworkManager.Singleton.IsHost)
        {
            behavier.defaultPosition = PlayerController.Instance.player.transform;
        }

        behavier.onAttack.AddListener(attackHandle);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerAlly && other.TryGetComponent<enemyBehavier>(out var enemy) && enemy != behavier)
        {
            behavier.chasePlayer(other.transform);
            return;
        }
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
        behavier.onAttack.RemoveListener(attackHandle);
    }
    #endregion
    public void attackHandle(bool attackMode)
    {
        if (isPlayerAlly)
        {
            var targetinfo = behavier.target.GetComponent<enemyInfo>();
            targetinfo.takeDamage(info.attack, attackMode ? info.farAttackDmgType : info.nearAttackDmgType);
            return;
        }
        if (!playerIsInRange) return;
        // player o trong pham vi 
        increaseSpeed();
        PlayerController.Instance.playerInfo.takeDamage(info.attack, attackMode ? info.farAttackDmgType : info.nearAttackDmgType);
    }

    public void increaseSpeed()
    {
        info.speed += 1;
    }


}
