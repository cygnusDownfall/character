using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CharacterController))]
public class enemyBehavier : NetworkBehaviour
{
    #region movement
    Transform target = null;
    public CharacterController controller;
    public SplineContainer spl;
    [SerializeField] Transform defaultPosition;
    public float speed = 1;
    float t = 0;
    [SerializeField] float rangeAttack = 2f;
    void move()
    {
        if (target == null)
        {
            moveAsDefaultSpline();
            return;
        }

        NativeArray<float3> dir = new NativeArray<float3>(1, Allocator.TempJob);
        (float3 oldPos, float3 tarPos) = (new float3(transform.position.x, transform.position.y, transform.position.z), new float3(target.position.x, target.position.y, target.position.z));
        CalcPositionMoveJob calcPos = new CalcPositionMoveJob()
        {
            oldPosition = oldPos,
            dir = dir,
            targetPos = tarPos,
            timeDelta = Time.deltaTime,
            speed = speed,
        };
        var handle = calcPos.Schedule();
        //xu li animation va sound


        //gan vi tri 
        handle.Complete();
        if (controller && math.length(dir[0]) > rangeAttack)
        {
            controller.Move(dir[0]);
        }
        else
        {
            target = null;
        }
        dir.Dispose();
    }

    void moveAsDefaultSpline()
    {
        t += Time.deltaTime;
        //what range of value t
        float sint = math.sin(t);
        var pos = spl.EvaluatePosition(sint);
        transform.position = new Vector3(pos.x, pos.y, pos.z);

    }

    public void chasePlayer(GameObject go)
    {
        if (!target) return;

        target = go.transform;
    }
    public void returnToPosition()
    {
        target = default;

    }
    #endregion
    #region mono
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        move();
    }

    #endregion
}
[BurstCompile]
public struct CalcPositionMoveJob : IJob
{
    public float3 oldPosition;
    public float3 targetPos;
    [WriteOnly] public NativeArray<float3> dir;
    public float timeDelta;
    public float speed;
    public void Execute()
    {
        float3 dir3 = targetPos - oldPosition;
        dir3 = math.normalize(dir3);
        dir[0] = speed * dir3 * timeDelta;
    }
}
