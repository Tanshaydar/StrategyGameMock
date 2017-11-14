using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractButton : MonoBehaviour
{
    protected GameManager _gameManager;
    
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    protected abstract void HandleClick();
}