namespace EventAggregation.Messages
{
    public class SoldierSelectedMessage
    {
        public Soldier Soldier { get; private set; }

        public SoldierSelectedMessage(Soldier soldier)
        {
            Soldier = soldier;
        }
    }
}
