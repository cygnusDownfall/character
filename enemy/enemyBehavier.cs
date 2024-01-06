using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(enemyInfo), typeof(dissolve))]
public class enemyBehavier : NetworkBehaviour
{
    #region movement
    public Transform target = null;
    #region ref
    [Header("----------------Ref-----------------")]
    public AudioSource moveAudioSource;
    enemyInfo info;
    public Unity.Netcode.Components.NetworkAnimator animator;
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
        if (!target.Equals(PlayerController.Instance.player.transform)) return;
        // length = 2 for 2 element is dir after calc and origin 
        dir = new NativeArray<float3>(2, Allocator.TempJob);
        function.LookAtXAxis(transform, target.position + Vector3.up);
        (float3 oldPos, float3 tarPos) = (function.vector3ToFloat3(transform.position), function.vector3ToFloat3(target.position));

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
        if (!target.Equals(PlayerController.Instance.player.transform)) return;

        //gan vi tri 
        handle.Complete();
        var len = math.length(calcPos.dir[1]);
        if (len > distanceMaxChase)
        {
            Debug.Log("return to default position. Len:" + len);
            returnToPosition();

        }
        else if (len > info.rangeAttackFar)
        {
            transform.position = function.float3ToVector3(calcPos.dir[0]);
        }
        else if (len > info.rangeAttack)
        {
            transform.position = function.float3ToVector3(calcPos.dir[0]) + Vector3.up * adjustYAxisTargetWhenMove;

            attack();
            lastAttackTime = Time.time;
        }
        else
        {
            attack(false);
            lastAttackTime = Time.time;
        }

        dir.Dispose();
    }
    /// <param name="attackMode">true is far and false is near </param>
    void attack(bool attackMode = true)
    {
        if (Time.time - lastAttackTime < delayBeforeNextAttack) return;
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
        target = null;
        StartCoroutine(waitToSetReturnPos());
    }
    private System.Collections.IEnumerator waitToSetReturnPos(int delay = 3)
    {
        var mesh = GetComponentInChildren<MeshRenderer>();
        yield return new WaitForSeconds(delay);
        while (IsMeshVisible(mesh))
        {
            yield return new WaitForSeconds(1);
        }
        Debug.Log("return to default pos:" + transform.position);

        if (defaultPosition != null)
        {
            transform.position = defaultPosition.position;
            transform.rotation = defaultPosition.rotation;
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
        Debug.Log("Die");
        target = null;
        GetComponent<dissolve>().RunDisolve();
        Destroy(gameObject, 2);

    }
    #endregion
    #region mono
    void Start()
    {
        moveAudioSource = GetComponentInParent<AudioSource>();
        info = gameObject.GetComponent<enemyInfo>();
        info.onDie.AddListener(OnDie);
        transform.position = defaultPosition.position;
        animator = GetComponent<Unity.Netcode.Components.NetworkAnimator>();
        animator.Animator = GetComponent<Animator>();
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
    /// calculate new position and save to dir[0], directionand save to dir[1]
    /// </summary>
    public void Execute()
    {
        float3 dir3 = targetPos - oldPosition;
        dir[1] = dir3;
        dir3 = math.normalize(dir3);
        dir[0] = oldPosition + dir3 * speed * timeDelta;
    }

}
