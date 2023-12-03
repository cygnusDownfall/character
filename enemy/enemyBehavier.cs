using System.Collections;
using System.Threading.Tasks;
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
    public Transform target = null;
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
    /// false= near  true= far
    /// </summary>
    public byte seccondDelayToReturn = 3;
    public byte distanceMaxChase = 10;
    [Header("--------------------Event--------------------")]
    public UnityEngine.Events.UnityEvent onAttack;
    void moveToChasePlayer()
    {
        if (target == null)
        {
            return;
        }
        NativeArray<float3> dir = new NativeArray<float3>(1, Allocator.TempJob);
        (float3 oldPos, float3 tarPos) = (new float3(transform.position.x, transform.position.y, transform.position.z), new float3(target.position.x, target.position.y, target.position.z));

        Debug.Log("old:" + oldPos + "tarPos:" + tarPos);
        CalcPositionMoveJob calcPos = new CalcPositionMoveJob()
        {
            oldPosition = oldPos,
            dir = dir,
            targetPos = tarPos,
            timeDelta = Time.deltaTime,
            speed = info.speed,
        };
        var handle = calcPos.Schedule();
        Debug.Log("moveAudio: " + moveAudioSource != null);
        //xu li animation va sound
        if (!moveAudioSource.isPlaying)
        {
            moveAudioSource.Play();
        }


        //gan vi tri 
        handle.Complete();
        var len = math.length(calcPos.dir[0]);
        Debug.Log("calc dir " + calcPos.dir[0]);
        //
        if (len > distanceMaxChase)
        {
            //
            returnToPosition();

        }
        else if (rb && (len > info.rangeAttackFar))
        {
            rb.velocity = calcPos.dir[0];
        }
        else if (len > info.rangeAttack)
        {
            attack();

        }
        else
        {

        }

        dir.Dispose();
    }

    /// <param name="attackMode">true is far and false is near </param>
    void attack(bool attackMode = true)
    {
        //play animation
        moveAudioSource.Stop();
        if (attackMode)
        {
            animator.SetTrigger("attackFar");
        }
        else
        {
            animator.SetTrigger("attack");
        }
    }
    void attackHit(playerInfo player, bool attackMode = true)
    {
        player.takeDamage(info.attack, attackMode ? info.farAttackDmgType : info.nearAttackDmgType);
    }

    public void chasePlayer(Transform go)
    {
        if (target != null) return;
        Debug.Log("chase player:" + go);
        target = go;
    }
    public Task returnToPosition()
    {
        Task.Delay(seccondDelayToReturn);
        return Task.Run(
            () =>
            {

                if (defaultPosition != null)
                {
                    transform.position = defaultPosition.position;
                }
            }
        );

    }
    public void OnDie(characterInfo info)
    {
        //animator.SetBool("die", true);
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
        info = gameObject.GetComponent<enemyInfo>();
        info.onDie.AddListener(OnDie);
        defaultPosition = transform;
    }
    void Update()
    {
        moveToChasePlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != PlayerController.Instance.player) { return; }

    }

    #endregion
}
[BurstCompile]
public struct CalcPositionMoveJob : IJob
{
    public float3 oldPosition;
    public float3 targetPos;
    /// <summary>
    /// dir after calculate and have normalized 
    /// </summary>
    [WriteOnly] public NativeArray<float3> dir;
    public float timeDelta;
    public float speed;
    /// <summary>
    /// 
    /// </summary>
    public void Execute()
    {
        float3 dir3 = targetPos - oldPosition;
        dir3 = math.normalize(dir3);
        dir[0] = speed * dir3 * timeDelta;
    }
}
