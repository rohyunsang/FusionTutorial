using Fusion;
using UnityEngine;

public class Archer : Unit
{
    [Header("Projectile")]
    [SerializeField] private NetworkPrefabRef projectilePrefab; // 화살 프리팹
    [SerializeField] private Transform _firePoint;

    protected override void Awake()
    {
        base.Awake();

        detectionRange = 3.0f;
    }


    public override void Attack()
    {
        _nAnim.Animator.SetBool("1_Move", false);
        Debug.Log("Attack");

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Debug.Log("Attack!");

            _nAnim.Animator.SetTrigger("2_Attack");

            Invoke(nameof( ShootProjectile), 0.7f); // 0.5초 정도 딜레이 
        }
    }

    // 화살생성 함수 . 

    private void ShootProjectile()
    {
        if (projectilePrefab != null && _firePoint != null)
        {
            // 발사체 생성
            var projectile = Runner.Spawn(projectilePrefab, _firePoint.position, _firePoint.rotation);
            projectile.GetComponent<Projectile>().AttackDamage = 10f;

            if (type == UnitType.Human)
            {
                projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(10f, 0f), ForceMode2D.Impulse);
            }
            else if (type == UnitType.Devil)
            {
                projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10f, 0f), ForceMode2D.Impulse);
            }
        }
    }
}
