using EventAggregation;
using EventAggregation.Messages;

public class PowerPlant : PlaceableMapItem
{
    public int Identifier { get; set; }

    public override void OnMouseDown()
    {
        if (!IsMoving)
        {
            EventAggregator.Instance.Publish(new ShowMenuMessage(MenuBuildingType.PowerPlant, Identifier));
        }
        base.OnMouseDown();
    }
}