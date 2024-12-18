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
    public float HP { get; set; } = 100f; // ���� �󿡼� ����ȭ
    public UnitType type;

    [SerializeField] private TextMeshPro _HPBar;


    [Header("Attack")]
    [SerializeField] private BoxCollider2D _boxCollider2D;
    [SerializeField] private Weapon _weapon;
    public float AttackDamage { get; set; } = 10f;  // ���ݷ�
    private float detectionRange = 0.8f;  // ���� Ž�� ����
    private float attackCooldown = 1f;    // ���� ��Ÿ��
    private bool isAttacking = false;     // 
    private float lastAttackTime;         // ���� ��Ÿ���� ���� ����. 

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
        // �Ʊ� ����
        var ally = DetectAlly();
        if (ally != null)
        {
            Debug.Log($"�Ʊ� ����: {ally.name}");
            _rb.linearVelocity = Vector2.zero;
            _nAnim.Animator.SetBool("1_Move", false);
            return;
        }

        if (!isAttacking)
        {
            // �� ����
            var enemy = DetectEnemy();
            if (enemy != null)
            {
                Debug.Log($"���� Ÿ�� {type} : �߰ߵ� ��: {enemy.type}");
                isAttacking = true;
                _rb.linearVelocity = Vector2.zero;
                Attack(); // ������ ���� ����
                return;
            }

            // �̵�
            Move();
        }
        else
        {
            // ���� ��� ��
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

        return null; // �Ʊ��� ���ų� �ڱ� �ڽŹۿ� ���� ��� null ��ȯ
    }

    Unit DetectEnemy()
    {
        RaycastHit2D[] hits;

        // ����� Ž�� ���� ����
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

        // Ž���� ��ü �� Unit ������Ʈ�� �ִ� ��ü�� ��ȯ
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.TryGetComponent<Unit>(out var enemyUnit))
            {
                if (enemyUnit.type == OppositeType[type]) // �ݴ� Ÿ������ Ȯ��
                {
                    return enemyUnit; // ���� ����� �� ��ȯ
                }
            }
        }

        return null; // ���� ������ null ��ȯ
    }

    void Attack()
    {
        _nAnim.Animator.SetBool("1_Move", false);
        Debug.Log("Attack");

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Invoke(nameof(EnableAttackCollider), 0.5f); // 0.5�� �� Ȱ��ȭ

            Debug.Log("Attack!");

            _nAnim.Animator.SetTrigger("2_Attack");
            Invoke(nameof(DisableAttackCollider), 1.0f); // 1.0�� �� ��Ȱ��ȭ
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
            // Destroy�� �Ȱ��ƿ�. �����󿡼� ���𰡸� �����Ҷ�, Destroy�Ҷ� Despawn()�Լ��� �̿������ �ؿ�. 
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ApplyDamage(float attackDamage) // ǻ�������� ����ȭ�� ���ٶ� RPC ��� ���� ���ϴ�. 
    {                                               // �׷��� �� RPC�� ���ĸ� ���߿� ���������� ���� ������ �Ұſ���. 
                                                    // Remote Precedure Call  -> �̰Ŵ� ���� ���Ը��ϸ� 
                                                    // Ŭ���̾�Ʈ �Ѵ� �������� �޾Ҵٴ� ���� �˾ƾ��ϴϱ� 
                                                    // �� �Լ��� �Ѵ� �������ִ°Ŷ� ������ ����ϴٰ� ���� �˴ϴ�. 
                                                    // �̰Ŵ� ���� �����󿡼� RPC�� �ǰ� �߿��ϴϱ� ���� ������ �ҰԿ�. 

        Debug.Log($"Begin HP : {HP} attckDamage : {attackDamage}:" + gameObject.name);

        HP -= attackDamage;

        Debug.Log($"End   HP : {HP} attckDamage : {attackDamage}:" + gameObject.name);
    }

    private void OnDrawGizmosSelected()
    {
        // ������� ���� Ž�� ������ �ð������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}