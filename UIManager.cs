using MyMenu.Entities;

namespace FoxFiles
{
    public class UIManager
    {
        private static UIManager _instance = null;

        private UIManager()
        {
        }

        public static UIManager GetInstance()
        {
            return _instance ?? (_instance = new UIManager());
        }

        public void SpawnMenu()
        {
            Section section = new Section(Section.GetSourceName(), Section.GetSourceName(), "v1.0.0", "Fooxiie");
            section.Insert();
        }

        public void SpawnAdminMenu()
        {
            Section section = new Section(Section.GetSourceName(), Section.GetSourceName() + " Admin", "v1.0.0",
                "Fooxiie", onlyAdmin: true);
            
            section.Insert();
        }
    }
}