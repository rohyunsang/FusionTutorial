using UnityEngine;
using Fusion;
using System.Collections.Generic;
using TMPro;
public enum UnitType
{
    Human = 0,
    Devil = 1
}

public class Unit : NetworkBehaviour
{

    [Header("Stat")]
    [Networked, OnChangedRender(nameof(HPChanged))]
    public float HP { get; set; } = 100f; // 서버 상에서 동기화
    public UnitType type;

    [SerializeField] private TextMeshPro _HPBar;


    [Header("Attack")]
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private Weapon _weapon;
    public float AttackDamage { get; set; } = 10f;  // 공격력
    private float detectionRange = 0.8f;  // 공격 탐지 범위
    private float attackCooldown = 1f;    // 공격 쿨타임
    private bool isAttacking = false;     // 
    private float lastAttackTime;         // 공격 쿨타임을 위한 변수. 

    [Header("Move")]
    private float moveSpeed = 50f;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CircleCollider2D _circleCollider2D;  // 


    [Header("Anim")]
    private NetworkMecanimAnimator _nAnim;

    [Header("Etc")]
    private static readonly Dictionary<UnitType, UnitType> OppositeType = new Dictionary<UnitType, UnitType>
    {
        { UnitType.Human, UnitType.Devil },
        { UnitType.Devil, UnitType.Human }
    };

    private void Awake()
    {
        Debug.Log("Awake");

        _rb = GetComponent<Rigidbody2D>();
        _weapon.type = this.type;
    }

    public override void Spawned()
    {
        Debug.Log("Spawned");

        _weapon.AttackDamage = AttackDamage;  // network value 
        _nAnim = GetComponent<NetworkMecanimAnimator>();
    }

    public override void FixedUpdateNetwork()
    {
        // 아군 감지
        var ally = DetectAlly();
        if (ally != null)
        {
            Debug.Log($"아군 감지: {ally.name}");
            _rb.linearVelocity = Vector2.zero;
            _nAnim.Animator.SetBool("1_Move", false);
            return;
        }

        if (!isAttacking)
        {
            // 적 감지
            var enemy = DetectEnemy();
            if (enemy != null)
            {
                Debug.Log($"나의 타입 {type} : 발견된 적: {enemy.type}");
                isAttacking = true;
                _rb.linearVelocity = Vector2.zero;
                Attack(); // 감지된 적을 공격
                return;
            }

            // 이동
            Move();
        }
        else
        {
            // 공격 대기 중
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                isAttacking = false;
            }
        }
    }

    void Move()
    {
        _nAnim.Animator.SetBool("1_Move", true);
        if (type == UnitType.Human)
        {
            _rb.linearVelocity = new Vector2(moveSpeed, _rb.linearVelocity.y) * Runner.DeltaTime;
        }
        else if (type == UnitType.Devil)
        {
            _rb.linearVelocity = new Vector2(moveSpeed * -1, _rb.linearVelocity.y) * Runner.DeltaTime;
        }
    }

    GameObject DetectAlly()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _circleCollider2D.radius, LayerMask.GetMask(type.ToString()));

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.TryGetComponent<Unit>(out var allyUnit))
            {
                if (allyUnit.type == type)
                {
                    Vector2 direction = (hit.transform.position - transform.position).normalized;
                    if ((type == UnitType.Human && direction.x > 0) || (type == UnitType.Devil && direction.x < 0))
                    {
                        return hit.gameObject;
                    }
                }
            }
        }

        return null; // 아군이 없거나 자기 자신밖에 없는 경우 null 반환
    }

    Unit DetectEnemy()
    {
        RaycastHit2D[] hits;

        // 방향과 탐지 범위 설정
        if (type == UnitType.Human)
        {
            hits = Physics2D.RaycastAll(transform.position, Vector2.right, detectionRange, LayerMask.GetMask(OppositeType[type].ToString()));
        }
        else if (type == UnitType.Devil)
        {
            hits = Physics2D.RaycastAll(transform.position, Vector2.left, detectionRange, LayerMask.GetMask(OppositeType[type].ToString()));
        }
        else
        {
            return null;
        }

        // 탐지된 객체 중 Unit 컴포넌트가 있는 객체만 반환
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.TryGetComponent<Unit>(out var enemyUnit))
            {
                if (enemyUnit.type == OppositeType[type]) // 반대 타입인지 확인
                {
                    return enemyUnit; // 가장 가까운 적 반환
                }
            }
        }

        return null; // 적이 없으면 null 반환
    }

    void Attack()
    {
        _nAnim.Animator.SetBool("1_Move", false);
        Debug.Log("Attack");

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Invoke(nameof(EnableAttackCollider), 0.5f); // 0.5초 후 활성화

            Debug.Log("Attack!");

            _nAnim.Animator.SetTrigger("2_Attack");
            Invoke(nameof(DisableAttackCollider), 1.0f); // 1.0초 후 비활성화
        }
    }

    void EnableAttackCollider()
    {
        _boxCollider2D.enabled = true;
    }

    void DisableAttackCollider()
    {
        _boxCollider2D.enabled = false;
    }

    void HPChanged()
    {
        Debug.Log($"Health changed to: {HP} :" + gameObject.name);
        
        _HPBar.text = HP.ToString();

        if (HP <= 0)
        {
            Runner.Despawn(Object); 
            // Destroy랑 똑같아요. 서버상에서 무언가를 삭제할때, Destroy할때 Despawn()함수를 이용해줘야 해요. 
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ApplyDamage(float attackDamage) // 퓨전에서는 동기화를 해줄때 RPC 라는 것을 씁니다. 
    {                                               // 그래서 이 RPC가 뭐냐면 나중에 동영상으로 따로 설명을 할거에요. 
                                                    // Remote Precedure Call  -> 이거는 쉽게 쉽게말하면 
                                                    // 클라이언트 둘다 데미지를 받았다는 것을 알아야하니까 
                                                    // 이 함수를 둘다 실행해주는거랑 느낌이 비슷하다고 보면 됩니다. 
                                                    // 이거는 다음 동영상에서 RPC는 되게 중요하니까 따로 설명을 할게요. 

        Debug.Log($"Begin HP : {HP} attckDamage : {attackDamage}:" + gameObject.name);

        HP -= attackDamage;

        Debug.Log($"End   HP : {HP} attckDamage : {attackDamage}:" + gameObject.name);
    }

    private void OnDrawGizmosSelected()
    {
        // 디버깅을 위해 탐지 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}