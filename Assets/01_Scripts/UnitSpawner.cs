using UnityEngine;
using Fusion;
using UnityEngine.UI;


public class UnitSpawner : NetworkBehaviour
{
    // 1. ��ư�� �������� ������ ������. 
    [SerializeField] private Button _human_WarriorSpawnButton;
    [SerializeField] private Button _devil_WarriorSpawnButton;
    [SerializeField] private NetworkPrefabRef _human_WarriorPrefab;
    [SerializeField] private NetworkPrefabRef _devil_WarriorPrefab;

    private void Awake()
    {
        // ��ư�� �Լ��� ��������� �մϴ�. 

        _human_WarriorSpawnButton.onClick.AddListener(SpawnHumanWarriorUnit);
        _devil_WarriorSpawnButton.onClick.AddListener(SpawnDevilWarriorUnit);
    }

    // ���������� ������ �����ϴ� �Լ� �ϳ� �־����.

    private void SpawnHumanWarriorUnit() // using Button
    {
        Runner.Spawn(_human_WarriorPrefab, new Vector3(-5f, 0f, 0f), Quaternion.Euler(0f, 180f, 0f));
    }

    private void SpawnDevilWarriorUnit() // using Button
    {
        Runner.Spawn(_devil_WarriorPrefab, new Vector3(5f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
    }
}
