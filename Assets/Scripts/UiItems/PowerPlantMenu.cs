using UnityEngine;
using UnityEngine.UI;

public class PowerPlantMenu : MonoBehaviour
{
    public Text MenuText;
    private GameManager _gameManager;

    public void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void ShowPowerPlantMenu(int identifier)
    {
        if (_gameManager == null)
        {
            _gameManager = FindObjectOfType<GameManager>();
        }
        _gameManager.DisableMenus();
        gameObject.SetActive(true);
        MenuText.text = "Power Plant #" + identifier;
    }
}