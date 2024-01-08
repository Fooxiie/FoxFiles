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
        private FoxOrm _foxOrm;

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

                InitDatabase();
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        private void InitDatabase()
        {
            _foxOrm = new FoxOrm(Path.Combine(pluginsPath, "FoxFiles/database.sqlite"));

            _foxOrm.RegisterTable<Configuration>();
            VerifyConfigMigrations();
        }

        private async void VerifyConfigMigrations()
        {
            Dictionary<string, string> defaultConfig = new Dictionary<string, string>()
            {
                { "minAdminLvlRequired", "5" }
            };
            var listConfig = await _foxOrm.QueryAll<Configuration>();

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
                var resultInsert = await _foxOrm.Save(newConfig);
                Debug.LogWarning($"[Migration FoxFiles] adding config {pair.Key} : {resultInsert}");
            }
        }
    }
}