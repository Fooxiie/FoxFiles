using System;
using System.Collections.Generic;
using System.Linq;
using FoxFiles.Models;
using Life;
using Life.Network;
using Life.UI;
using UIPanelManager;

namespace FoxFiles.PackagePanel
{
    public class FormTypeDocumentPanel
    {
        private string _title;
        private List<string> _fields;
        private Player _player;
        private Action _previousAction;

        private int doneStep = 0;
        private Dictionary<string, string> finalDirectory;

        public FormTypeDocumentPanel(string title, List<string> fields, Player player, Action previousAction)
        {
            _title = title;
            _fields = fields;
            _player = player;
            _previousAction = previousAction;

            finalDirectory = fields.ToDictionary(x => x);
        }

        private string makeTitle()
        {
            return $"{_title} ({doneStep}\\{_fields.Count})";
        }

        public void Display()
        {
            var panel = new UIPanel(makeTitle(), UIPanel.PanelType.Tab);
            foreach (var field in finalDirectory)
            {
                panel.AddTabLine($"{field.Key} : {field.Value}", ui => Action(field.Key));
            }
            
            

            panel.AddButton("Editer", ui => ui.SelectTab());
            panel.AddButton("Valider", async ui =>
            {
                var typeDocument = new TypeDocument()
                {
                    TypeName = finalDirectory["Nom"],
                    Author = _player.character.Id,
                };
                await UIManager.GetInstance().getFoxInstance().FoxOrm.Save(typeDocument);
                PanelManager.NextPanel(_player, panel, _previousAction);
            });
            panel.AddButton("Retour", ui => PanelManager.NextPanel(_player, panel, _previousAction));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, _player));
            _player.ShowPanelUI(panel);
        }

        private void Action(string fieldName)
        {
            if (!finalDirectory.ContainsKey(fieldName))
            {
                _player.Notify("Erreur formulaire", "Le field n'existe pas", NotificationManager.Type.Error);
                return;
            }

            var inputPanel = new UIPanel(fieldName, UIPanel.PanelType.Input)
            {
                inputPlaceholder = "value.."
            };

            inputPanel.AddButton("Confirmer", (ui) =>
            {
                if (ui.inputText.Length > 0)
                {
                    finalDirectory[fieldName] = inputPanel.inputText;
                    PanelManager.NextPanel(_player, ui, Display);
                }
                else PanelManager.Notification(_player, "Erreur", "Vous devez renseigner une valeur", NotificationManager.Type.Error);
            });
            inputPanel.AddButton("Fermer", (ui) => PanelManager.Quit(ui, _player));
            
            _player.ShowPanelUI(inputPanel);
        }
    }
}