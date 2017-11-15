using UnityEngine;
using UnityEngine.UI;

public class BarrackMenu : MonoBehaviour
{
    public Text MenuText;
    private GameManager _gameManager;
    private SoldierButton _soldierButton;

    public void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _soldierButton = GetComponentInChildren<SoldierButton>();
    }

    public void ShowBarrackMenu(int identifier)
    {
        if (_gameManager == null)
        {
            _gameManager = FindObjectOfType<GameManager>();
        }
        _gameManager.DisableMenus();
        gameObject.SetActive(true);
        MenuText.text = "Barrack #" + identifier;
        _soldierButton.SetIdentifier(identifier);
    }
}