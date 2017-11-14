public class Barrack : PlaceableMapItem
{
    public int Identifier { get; set; }

    public override void OnMouseDown()
    {
        if (!IsMoving)
        {
            _gameManager.BarrackMenu.ShowBarrackMenu(Identifier);
        }
        else
        {
            base.OnMouseDown();
        }        
    }
}