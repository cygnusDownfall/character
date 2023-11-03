using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class enemyBehavier : NetworkBehaviour
{
    #region movement
    Transform target = null;
    #region ref
    [Header("----------------Ref-----------------")]
    public CharacterController controller;
    public AudioSource moveAudioSource;
    public Animator animator;
    [SerializeField] Transform defaultPosition;
    #endregion
    [Header("---------------------state--------------------")]
    public float speed = 1;
    float t = 0;
    [SerializeField] float rangeAttack = 2f;
    void move()
    {
        if (target == null)
        {
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
        if (!moveAudioSource.isPlaying)
        {
            moveAudioSource.Play();
        }


        //gan vi tri 
        handle.Complete();
        if (controller && (math.length(dir[0]) > rangeAttack))
        {
            controller.Move(dir[0]);
        }
        else
        {
            target = null;
            moveAudioSource.Stop();
        }
        dir.Dispose();
    }



    public void chasePlayer(GameObject go)
    {
        if (!target) return;

        target = go.transform;
    }
    public void returnToPosition()
    {
        target = defaultPosition;

    }
    #endregion
    #region mono
    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveAudioSource = GetComponent<AudioSource>();
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
