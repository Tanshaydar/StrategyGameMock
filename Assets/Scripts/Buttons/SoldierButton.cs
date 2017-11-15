public class SoldierButton : AbstractButton
{
    private int identifier;

    protected override void HandleClick()
    {
        _gameManager.CreateSoldier(identifier);
    }

    public void SetIdentifier(int identifier)
    {
        this.identifier = identifier;
    }
}