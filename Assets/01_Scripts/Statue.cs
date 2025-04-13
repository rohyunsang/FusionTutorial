using Fusion;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public UnitType type;
    [Networked, OnChangedRender(nameof(HPChanged))]
    public float HP { get; set; } = 100f;

    [SerializeField] private TextMeshPro _HPBar;

    private static readonly Dictionary<UnitType, UnitType> OppositeType = new Dictionary<UnitType, UnitType>
    {
        { UnitType.Human, UnitType.Devil },
        { UnitType.Devil, UnitType.Human }
    };

    void HPChanged()
    {
        Debug.Log($"Health changed to : {HP} : " + gameObject.name);

        if (HP < 0)
        {
            Debug.Log("조각상 체력 다 떨어짐.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ApplyDamage(float attackDamage)
    {
        Debug.Log($"Begin HP : {HP} : attackDamage : {attackDamage} : " + gameObject.name);
        HP -= attackDamage;
        Debug.Log($"End HP : {HP} : attackDamage : {attackDamage} : " + gameObject.name);

        // 이제 다른 곳에서 RPC_ApplyDamage()를 호출해주면 되는거에요. 
    }
}
