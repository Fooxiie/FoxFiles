using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FoxFiles.Models;
using Life;
using FoxORM;
using RTG;
using UnityEngine;

namespace FoxFiles
{
    public class FoxFiles : Plugin
    {
        public FoxOrm FoxOrm;

        public FoxFiles(IGameAPI api) : base(api)
        {
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            
            try
            {
                UIManager.GetInstance().SpawnMenu();
                UIManager.GetInstance().SpawnAdminMenu();
                UIManager.GetInstance().SetFoxFilesInstance(this);

                InitDatabase();
            }
            catch (Exception error)
            {
                Debug.LogError("[FoxFiles] :" +  error);
            }
        }

        private void InitDatabase()
        {
            FoxOrm = new FoxOrm(Path.Combine(pluginsPath, "FoxFiles/database.sqlite"));

            FoxOrm.RegisterTable<Configuration>();
            FoxOrm.RegisterTable<TypeDocument>();
            FoxOrm.RegisterTable<Document>();
            VerifyConfigMigrations();
        }

        private async void VerifyConfigMigrations()
        {
            Dictionary<string, string> defaultConfig = new Dictionary<string, string>()
            {
                { "minAdminLvlRequired", "5" }
            };
            var listConfig = await FoxOrm.QueryAll<Configuration>();

            foreach (var pair in defaultConfig)
            {
                var found = false;
                foreach (var configuration in listConfig.Where(configuration => configuration.configCode == pair.Key))
                {
                    found = true;
                }

                if (found) continue;
                var newConfig = new Configuration()
                {
                    value = pair.Value,
                    configCode = pair.Key
                };
                var resultInsert = await FoxOrm.Save(newConfig);
                Debug.LogWarning($"[Migration FoxFiles] adding config {pair.Key} : {resultInsert}");
            }
        }
    }
}