using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [SerializeField] private Button _gameStartButton;

    void Start()
    {
        _gameStartButton.onClick.AddListener(FindAnyObjectByType<Launcher>().GameStart);
    }

    void Update()
    {
        
    }
}
