using EventAggregation;
using EventAggregation.Messages;
using UiItems;
using UnityEngine.UI;

public class PowerPlantMenu : BaseMenu
{
    public Text MenuText;

    public PowerPlantMenu()
    {
        Type = MenuBuildingType.PowerPlant;
    }

    protected override void Show(int identifier)
    {
        EventAggregator.Instance.Publish(new HideMenuExceptMessage(MenuBuildingType.PowerPlant));
        gameObject.SetActive(true);
        MenuText.text = "Power Plant #" + identifier;
    }
}