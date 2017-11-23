using EventAggregation;
using EventAggregation.Messages;
using UiItems;
using UnityEngine.UI;

public class BarrackMenu : BaseMenu
{
    public Text MenuText;
    private SoldierButton _soldierButton;

    public BarrackMenu()
    {
        Type = MenuBuildingType.Barrack;
    }

    public void Awake()
    {
        _soldierButton = GetComponentInChildren<SoldierButton>();
    }

    protected override void Show(int identifier)
    {
        EventAggregator.Instance.Publish(new HideMenuExceptMessage(MenuBuildingType.Barrack));
        gameObject.SetActive(true);
        MenuText.text = "Barrack #" + identifier;
        _soldierButton.SetIdentifier(identifier);
    }
}