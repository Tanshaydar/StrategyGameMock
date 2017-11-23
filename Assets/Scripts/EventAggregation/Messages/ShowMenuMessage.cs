namespace EventAggregation.Messages
{
    public class ShowMenuMessage
    {
        public MenuBuildingType Type { get; private set; }
        public int Id { get; private set; }

        public ShowMenuMessage(MenuBuildingType type, int id)
        {
            Type = type;
            Id = id;
        }
    }
}