public class PowerPlant : PlaceableMapItem
{
    public int Identifier { get; set; }

    public override void OnMouseDown()
    {
        if (!IsMoving)
        {
            _gameManager.PowerPlantMenu.ShowPowerPlantMenu(Identifier);
        }
        base.OnMouseDown();
    }
}