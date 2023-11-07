using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class enemyArea : MonoBehaviour
{
    enemyBehavier[] childs;
    [SerializeField] float rangeDetectOfEach = 5f;
    public int numPlayerInArea { get; private set; }
    void loadChild()
    {
        int length = transform.childCount;
        childs = new enemyBehavier[length];
        for (int i = 0; i < length; i++)
        {
            childs[i] = transform.GetChild(i).gameObject.GetComponent<enemyBehavier>();
        }
    }
    #region mono
    private void Start()
    {
        //set thong so 
        numPlayerInArea = 0;

        //
        loadChild();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out playerInfo _)) { return; }
        numPlayerInArea++;
        float3[] float3s = childs.Select(eb => function.vector3ToFloat3(eb.gameObject.transform.position)).ToArray();
        NativeArray<float3> childPos = new NativeArray<float3>(float3s, Allocator.TempJob);
        NativeArray<int> indexs = new NativeArray<int>(childs.Length, Allocator.TempJob);
        for (int i = 0; i < indexs.Length; i++)
        {
            indexs[i] = -1;
        }
        enemyInRange detecter = new enemyInRange()
        {
            childChaseIndexs = indexs,
            childPos = childPos,
            targetPos = function.vector3ToFloat3(other.gameObject.transform.position),
            rangeDetect = rangeDetectOfEach,

        };
        var handle = detecter.Schedule();
        //tat ca deu xoay nhin player? 
        var plobj = other.gameObject.transform;
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.transform.LookAt(plobj);
        }

        // xu li khi player ve 0 se gay ra loi 
        if (numPlayerInArea <= 0) { return; }
        //xu li cac child chase player
        handle.Complete();
        for (int i = 0; i < numPlayerInArea; i++)
        {
            Debug.Log("index chase:" + i);
            childs[indexs[i]].chasePlayer(plobj.gameObject);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out playerInfo _)) { return; }
        if (numPlayerInArea > 0) numPlayerInArea--;
    }
    #endregion
}
[BurstCompile]
struct enemyInRange : IJob
{
    public float3 targetPos;
    public float rangeDetect;
    [ReadOnly] public NativeArray<float3> childPos;
    public NativeArray<int> childChaseIndexs;
    public void Execute()
    {
        float range;
        for (int i = 0; i < childPos.Length; i++)
        {
            //???? distance??????
            range = math.distance(targetPos, childPos[i]);
            if (range > rangeDetect)
            {
                continue;
            }
            for (int j = 0; j < childChaseIndexs.Length; j++)
            {
                if (childChaseIndexs[j] != -1)
                {
                    continue;
                }
                childChaseIndexs[j] = i;
            }
        }
    }
}

