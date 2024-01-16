using System.Collections.Generic;
using System.Linq;
using FoxFiles.Models;
using FoxFiles.PackagePanel;
using FoxORM;
using Life;
using Life.Network;
using Life.UI;
using Microsoft.Win32.SafeHandles;
using MyMenu.Entities;
using UIPanelManager;
using UnityEngine;

namespace FoxFiles
{
    public class UIManager
    {
        private static UIManager _instance = null;

        private FoxFiles _foxInstance;
        private Section _section;
        private Section _adminSection;

        private UIManager()
        {
        }

        public static UIManager GetInstance()
        {
            return _instance ?? (_instance = new UIManager());
        }

        public void SetFoxFilesInstance(FoxFiles foxFiles)
        {
            _foxInstance = foxFiles;
        }

        public FoxFiles getFoxInstance()
        {
            return _foxInstance;
        }

        public void SpawnMenu()
        {
            _section = new Section(Section.GetSourceName(), Section.GetSourceName(), "v1.0.0", "Fooxiie");
            _section.Line = new UITabLine("Mes documents", OpenMyDocuments);
            _section.Insert();
        }

        private void OpenMyDocuments(UIPanel panel)
        {
            panel.AddTabLine("Rien à voir ici !", _section.GetPlayer(panel).ClosePanel);
        }

        public void SpawnAdminMenu()
        {
            _adminSection = new Section(Section.GetSourceName() + "Admin", Section.GetSourceName() + " Admin", "v1.0.0",
                "Fooxiie", onlyAdmin: true);
            _adminSection.Line = new UITabLine("Administrations (FoxFiles)",
                ui => OpenAdminPanel(_adminSection.GetPlayer(ui)));
            _adminSection.Insert();
        }

        private void OpenAdminPanel(Player player)
        {
            UIPanel panel = new UIPanel("Admin Document", UIPanel.PanelType.Tab);
            panel.AddTabLine("Mes Types de documents",
                ui => PanelManager.NextPanel(player, ui, () => PanelTypeDocument(player)));

            panel.AddButton("Sélectionner", ui => ui.SelectTab());
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));
            player.ShowPanelUI(panel);
        }

        private void PanelTypeDocument(Player player)
        {
            UIPanel panel = new UIPanel("Mes Types Documents", UIPanel.PanelType.Tab);
            panel.AddTabLine("Lister les types",
                ui => PanelManager.NextPanel(player, ui, () => ListAllTypeDocument(player)));
            panel.AddTabLine("Créer un type",
                ui => PanelManager.NextPanel(player, ui, () => CreateTypeDocument(player)));

            panel.AddButton("Sélectionner", ui => ui.SelectTab());
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, panel, () => OpenAdminPanel(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));
            player.ShowPanelUI(panel);
        }

        private async void ListAllTypeDocument(Player player)
        {
            var panel = new UIPanel("Tous les types documents", UIPanel.PanelType.Tab);
            foreach (var typeDocument in await _foxInstance.FoxOrm.QueryAll<TypeDocument>())
            {
                panel.AddTabLine(typeDocument.TypeName, null);
            }

            if (panel.lines.Count == 0)
            {
                panel.AddTabLine(
                    $"<color={PanelManager.Colors[NotificationManager.Type.Error]}>Aucun Type de document</color>",
                    null);
            }

            // panel.AddButton("Editer", ui => ui.SelectTab());
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, panel, () => PanelTypeDocument(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));
            player.ShowPanelUI(panel);
        }

        private void CreateTypeDocument(Player player)
        {
            var formTypeDocument = 
                new FormTypeDocumentPanel("Type document",
                    new List<string>() { "Nom" },
                    player, () => PanelTypeDocument(player));
            formTypeDocument.Display();
        }
    }
}