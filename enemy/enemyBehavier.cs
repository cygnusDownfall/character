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
    CalcPositionMoveJob calcPos;
    JobHandle handle;
    NativeArray<float3> dir;
    #endregion
    [Header("---------------------state--------------------")]
    private float lastAttackTime;
    private float delayBeforeNextAttack = 1;
    /// <summary>
    /// false= near  true= far
    /// </summary>x
    public byte distanceMaxChase = 10;
    public float adjustYAxisTargetWhenMove = 1;
    [Header("--------------------Event--------------------")]
    public UnityEngine.Events.UnityEvent<bool> onAttack;
    void moveToChasePlayer()
    {
        if (target == null)
        {
            return;
        }
        if (target != PlayerController.Instance.player) return;
        // length = 2 for 2 element is dir after calc and origin 
        dir = new NativeArray<float3>(2, Allocator.TempJob);
        function.LookAtNegXAxis(transform, target.position + Vector3.up);
        (float3 oldPos, float3 tarPos) = (function.vector3ToFloat3(transform.position), function.vector3ToFloat3(target.position));

        Debug.Log("obj: " + this.gameObject + "\told:" + oldPos + "tarPos:" + tarPos);
        calcPos = new CalcPositionMoveJob()
        {
            oldPosition = oldPos,
            dir = dir,
            targetPos = tarPos,
            timeDelta = Time.deltaTime,
            speed = info.speed,
        };
        handle = calcPos.Schedule();
        //xu li animation va sound
        if (!moveAudioSource.isPlaying)
        {
            moveAudioSource.Play();
        }
    }
    void handleAfterCalcPos()
    {
        if (target == null)
        {
            return;
        }
        if (target != PlayerController.Instance.player) return;

        //gan vi tri 
        handle.Complete();
        var len = math.length(calcPos.dir[1]);
        Debug.Log("after calc of obj: " + this.gameObject);
        //
        if (len > distanceMaxChase)
        {
            Debug.Log("return to default position. Len:" + len);
            returnToPosition();

        }
        else if (len > info.rangeAttackFar)
        {
            Debug.Log("move");
            transform.position = function.float3ToVector3(calcPos.dir[0]);
        }
        else if (len > info.rangeAttack)
        {
            Debug.Log("move and far attack");
            transform.position = function.float3ToVector3(calcPos.dir[0]) + Vector3.up * adjustYAxisTargetWhenMove;
            if (Time.time - lastAttackTime > delayBeforeNextAttack)
            {
                attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            if (Time.time - lastAttackTime > delayBeforeNextAttack)
            {
                attack(false);
                lastAttackTime = Time.time;
            }
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
        onAttack.Invoke(attackMode);


    }
    public void chasePlayer(Transform go)
    {
        if (target != null) return;
        Debug.Log("chase player:" + go);
        target = go;
    }
    public void returnToPosition()
    {
        StartCoroutine(waitToSetReturnPos());
    }
    private System.Collections.IEnumerator waitToSetReturnPos()
    {
        var mesh = GetComponentInChildren<MeshRenderer>();
        while (IsMeshVisible(mesh))
        {
            yield return new WaitForSeconds(1);
        }
        target = null;
        if (defaultPosition != null)
        {
            transform.position = defaultPosition.position;
        }
    }
    private bool IsMeshVisible(MeshRenderer targetMeshRenderer)
    {
        if (targetMeshRenderer == null)
        {
            Debug.LogWarning("Target MeshRenderer not assigned!");
            return false;
        }

        // Check if the mesh is visible from any camera
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), targetMeshRenderer.bounds);
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
        defaultPosition = transform.parent;
    }
    void Update()
    {
        moveToChasePlayer();
    }
    private void LateUpdate()
    {
        handleAfterCalcPos();
    }


    #endregion
}
[BurstCompile]
public struct CalcPositionMoveJob : IJob
{
    public float3 oldPosition;
    public float3 targetPos;
    /// <summary>
    /// dir[0] have normalized and then calculate, dir[1] is Raw dir from oldPos to targetPos
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
        dir[1] = dir3;
        dir3 = math.normalize(dir3);
        dir[0] = oldPosition + dir3 * speed * timeDelta;
    }

}
