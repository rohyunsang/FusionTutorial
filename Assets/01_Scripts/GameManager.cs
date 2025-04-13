using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject _victoryPanel;
    [SerializeField] public GameObject _defeatPanel;

    [SerializeField] private GameObject _humanWarriorUIGroup;
    [SerializeField] private GameObject  _humanArcherUIGroup;
    [SerializeField] private GameObject _devilWarriorUIGroup;
    [SerializeField] private GameObject  _devilArcherUIGroup;


    [SerializeField] private Button  _homeButton1;
    [SerializeField] private Button  _homeButton2;


    [SerializeField] private NetworkRunner networkRunner;

    private string team;

    private void Awake()
    {
        _homeButton1.onClick.AddListener(LeaveBattleScene);
        _homeButton2.onClick.AddListener(LeaveBattleScene);
    }

    private void Start()
    {
        networkRunner = FindAnyObjectByType<NetworkRunner>();
        // team 가져오기. 
        team = FindAnyObjectByType<Launcher>().SetTeam(networkRunner);

        if (team == "Human")
        {
            // Human 팀 UI만 보이게. 
            _humanWarriorUIGroup.SetActive(true);
            _humanArcherUIGroup.SetActive(true);
        }
        else
        {
            // Devil 팀 UI만 보이게 

            _devilArcherUIGroup.SetActive(true);
            _devilWarriorUIGroup.SetActive(true);
        }
    }



    public void LeaveBattleScene()
    {
        StartCoroutine(ShutdownAndLoadMainScene());
    }

    private IEnumerator ShutdownAndLoadMainScene()
    {
        if (networkRunner != null)
        {
            yield return networkRunner.Shutdown();
            // Fusion Session(방)에 들어가서 저희가 멀티 플레이 게임을 했잖아요? 
            // 그 Session 을 나갑니다. 
        }

        SceneManager.LoadScene("Title");
    }
}
