using Fusion;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Statue : NetworkBehaviour
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

        _HPBar.text = HP.ToString();

        if (HP < 0)
        {
            Debug.Log("조각상 체력 다 떨어짐.");

            string team = FindAnyObjectByType<Launcher>().SetTeam(FindAnyObjectByType<NetworkRunner>());

            if (type == UnitType.Human) // 체력이 0이하인 조각상이 휴먼 타입일 때 그러면 -> 데빌팀이 이긴거죠. 
            {
                if(team == "Devil")   // 파괴된 성은 휴먼팀 내팀은 데빌팀 -> 그럼 승리 .
                {
                    FindAnyObjectByType<GameManager>()._victoryPanel.SetActive(true);
                }
                else
                {
                    FindAnyObjectByType<GameManager>()._defeatPanel.SetActive(true);
                }
            }
            else if (type == UnitType.Devil)
            {
                if (team == "Human")
                {
                    FindAnyObjectByType<GameManager>()._victoryPanel.SetActive(true);
                }
                else
                {
                    FindAnyObjectByType<GameManager>()._defeatPanel.SetActive(true);
                }
            }
            // 근데 지금 게임을 플레이했을때 소환 버튼 상대팀 꺼를 소환 가능한다. 
            // 이러면 엄청난 버그죠. 그래서 이거를 막아줄 거에요 .
            
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
