using UnityEngine;
using UnityEngine.UI;

public class BarrackMenu : MonoBehaviour
{
    public Text MenuText;

    public void ShowBarrackMenu(int identifier)
    {
        gameObject.SetActive(true);
        MenuText.text = "Barrack #" + identifier;
    }
}