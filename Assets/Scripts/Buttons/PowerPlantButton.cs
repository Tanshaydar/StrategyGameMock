public class PowerPlantButton : AbstractButton
{
    public int Identifier { get; set; }

    protected override void HandleClick()
    {
        _gameManager.CreatePowerPlant(Identifier);
    }
}