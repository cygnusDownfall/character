using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(enemyInfo))]
public class enemyBehavier : NetworkBehaviour
{
    #region movement
    Transform target = null;
    #region ref
    [Header("----------------Ref-----------------")]
    public AudioSource moveAudioSource;
    Rigidbody rb;
    enemyInfo info;
    public Animator animator;
    [SerializeField] Transform defaultPosition;
    #endregion
    [Header("---------------------state--------------------")]
    float t = 0;
    /// <summary>
    /// 0 = near  1= far
    /// </summary>
    bool attackMode = false;
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
            speed = info.speed,
        };
        var handle = calcPos.Schedule();
        //xu li animation va sound
        if (!moveAudioSource.isPlaying)
        {
            moveAudioSource.Play();
        }


        //gan vi tri 
        handle.Complete();
        var len = math.length(dir[0]);
        if (rb && (len > info.rangeAttackFar))
        {
            rb.velocity = dir[0];
        }
        else
        {
            target = null;
            moveAudioSource.Stop();
            attack();
        }
        dir.Dispose();
    }

    void attack()
    {
        //play animation
        //goi effect 
        if (attackMode)
        {
            animator.SetTrigger("attackFar");
        }
        else
        {
            animator.SetTrigger("attackFar");
        }
    }
    void attackHit(playerInfo player)
    {
        player.takeDamage(info.attack, attackMode ? info.farAttackDmgType : info.nearAttackDmgType);
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
    public void OnDie(characterInfo info)
    {
        animator.SetBool("die", true);
        var meshs = GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshs)
        {
            mesh.gameObject.GetComponent<dissolve>().RunDisolve();
        }

    }
    #endregion
    #region mono
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAudioSource = GetComponent<AudioSource>();
        gameObject.GetComponent<enemyInfo>().onDie.AddListener(OnDie);
    }
    void Update()
    {
        move();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject != PlayerController.Instance.player) return;

    //     chasePlayer(other.gameObject);
    // }
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
