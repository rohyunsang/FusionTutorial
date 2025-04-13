using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public float AttackDamage { get; set; } = 10f;
    public UnitType type;

    private static readonly Dictionary<UnitType, UnitType> OppositeType = new Dictionary<UnitType, UnitType>
    {
        { UnitType.Human, UnitType.Devil },
        { UnitType.Devil, UnitType.Human }
    };
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Unit>(out var targetUnit))
        {
            if (HasStateAuthority == false) return;

            if (targetUnit.type == OppositeType[type])
            {
                Debug.Log($"Hit {targetUnit.type}");
                targetUnit.RPC_ApplyDamage(AttackDamage);
                AttackDamage = 0f;

                // 적에게 맞은 화살은 삭제. 
                Invoke(nameof(DestroyProjectile), 0.05f);
                GetComponent<CircleCollider2D>().enabled = false; // 두번 충돌 될 수도 있으니까, Collider를 꺼 줄게요.
            }
        }
        else if (collision.TryGetComponent<Statue>(out var statue))
        {
            if (HasStateAuthority == false) return;

            if (statue.type == OppositeType[type])
            {
                Debug.Log($"Hit {statue.type}");
                statue.RPC_ApplyDamage(AttackDamage);
                AttackDamage = 0f;

                // 적에게 맞은 화살은 삭제. 
                Invoke(nameof(DestroyProjectile), 0.05f);
                GetComponent<CircleCollider2D>().enabled = false; // 두번 충돌 될 수도 있으니까, Collider를 꺼 줄게요.
            }
        }
    }

    private void DestroyProjectile()
    {
        Runner.Despawn(Object);
    }
}
