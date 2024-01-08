using Life;

namespace FoxFiles
{
    public class FoxFiles: Plugin
    {
        public FoxFiles(IGameAPI api) : base(api)
        {
            UIManager.GetInstance().SpawnMenu();
            UIManager.GetInstance().SpawnAdminMenu();
        }
        
        
    }
}