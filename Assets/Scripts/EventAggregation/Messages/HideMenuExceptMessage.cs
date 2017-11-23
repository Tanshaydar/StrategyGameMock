namespace EventAggregation.Messages
{
    public class HideMenuExceptMessage
    {
        public MenuBuildingType ExceptType { get; private set; }

        public HideMenuExceptMessage(MenuBuildingType exceptType)
        {
            ExceptType = exceptType;
        }
    }
}