using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerGeneralInfo : SingletonNetwork<playerGeneralInfo>
{
    public GameObject dmgShowObj;
    public int cardPoint = 0;
    public byte maxCardPoint = 100;
    public string namePlayer;
    public List<GameObject> skillObject;
    public int getIdSkillObj(GameObject skillObj)
    {
        return skillObject.IndexOf(skillObj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnSkillObjServerRpc(ulong clientID, int i, Vector3 pos, Quaternion rot, skillMoveType movetype, Vector3 targetPos, float speed = 5, int delay = 0, bool haveSelfSpawned = false)
    {
        if (haveSelfSpawned)
        {
            if (NetworkManager.Singleton.LocalClientId == NetworkManager.Singleton.LocalClientId)
                return;
        }
        var newSpawned = Instantiate(skillObject[i], pos, rot);
        var spawnedObj = newSpawned.GetComponent<NetworkObject>();

        spawnedObj.SpawnWithOwnership(clientID);
        Dic.singleton.moveTypes[movetype].addMoveAsync(newSpawned, targetPos, speed, delay);
    }
}
