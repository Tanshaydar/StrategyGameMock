using System;
using UnityEngine;
using UnityEngine.UI;

public class PowerPlantMenu : MonoBehaviour
{
    public Text MenuText;

    public void ShowPowerPlantMenu(int identifier)
    {
        gameObject.SetActive(true);
        MenuText.text = "Power Plant #" + identifier;
    }
}