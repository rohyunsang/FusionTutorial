using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float AttackDamage { get; set; }
    public UnitType type;

    private static readonly Dictionary<UnitType, UnitType> OppositeType = new Dictionary<UnitType, UnitType>
    {
        { UnitType.Human, UnitType.Devil },
        { UnitType.Devil, UnitType.Human }
    };

    private void Awake()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Unit>(out var targetUnit))
        {
            if (targetUnit.type == OppositeType[type]) 
            {
                Debug.Log($"Hit {targetUnit.type}");
                targetUnit.RPC_ApplyDamage(AttackDamage); 
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}