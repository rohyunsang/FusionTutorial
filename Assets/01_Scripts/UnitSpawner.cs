using UnityEngine;
using Fusion;
using UnityEngine.UI;


public class UnitSpawner : NetworkBehaviour
{
    // 1. 버튼을 눌렀을때 유닛이 생성됨. 
    [SerializeField] private Button _human_WarriorSpawnButton;
    [SerializeField] private Button _devil_WarriorSpawnButton;
    [SerializeField] private NetworkPrefabRef _human_WarriorPrefab;
    [SerializeField] private NetworkPrefabRef _devil_WarriorPrefab;

    private void Awake()
    {
        // 버튼에 함수를 연결해줘야 합니다. 

        _human_WarriorSpawnButton.onClick.AddListener(SpawnHumanWarriorUnit);
        _devil_WarriorSpawnButton.onClick.AddListener(SpawnDevilWarriorUnit);
    }

    // 실질적으로 유닛을 생성하는 함수 하나 있어야죠.

    private void SpawnHumanWarriorUnit() // using Button
    {
        Runner.Spawn(_human_WarriorPrefab, new Vector3(-5f, 0f, 0f), Quaternion.Euler(0f, 180f, 0f));
    }

    private void SpawnDevilWarriorUnit() // using Button
    {
        Runner.Spawn(_devil_WarriorPrefab, new Vector3(5f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
    }
}
