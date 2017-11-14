public class BarrackButton : AbstractButton
{
    public int Identifier { get; set; }
    
    protected override void HandleClick()
    {
        _gameManager.CreateBarrack(Identifier);
    }
}