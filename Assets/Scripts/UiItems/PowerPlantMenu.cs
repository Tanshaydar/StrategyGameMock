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
        _gameManager.DisableMenus();
        gameObject.SetActive(true);
        MenuText.text = "Power Plant #" + identifier;
    }
}