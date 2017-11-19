public class SoldierButton : AbstractButton
{
    private int _identifier;

    protected override void HandleClick()
    {
        _gameManager.CreateSoldier(_identifier);
    }

    public void SetIdentifier(int identifier)
    {
        this._identifier = identifier;
    }
}