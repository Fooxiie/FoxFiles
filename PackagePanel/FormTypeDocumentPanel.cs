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
        private List<int> bizAllowed = new List<int>();

        public FormTypeDocumentPanel(string title, List<string> fields, Player player, Action previousAction)
        {
            _title = title;
            _fields = fields;
            _player = player;
            _previousAction = previousAction;

            finalDirectory = fields.ToDictionary(x => x);
        }

        private string MakeTitle()
        {
            return $"{_title} ({doneStep}\\{_fields.Count})";
        }

        public void Display()
        {
            var panel = new UIPanel(MakeTitle(), UIPanel.PanelType.Tab);
            foreach (var field in finalDirectory)
            {
                panel.AddTabLine($"<color=#dc8031>{field.Key}</color> : {field.Value}", ui => EditField(field.Key));
            }

            panel.AddTabLine("<color=#dc8031>Bizs autorisés</color>",
                ui => PanelManager.NextPanel(_player, panel, Autorize));

            panel.AddButton("Editer", ui => ui.SelectTab());

            async void Action(UIPanel ui)
            {
                var typeDocument = new TypeDocument()
                    { TypeName = finalDirectory["Nom"], Author = _player.character.Id, };
                await UIManager.GetInstance().getFoxInstance().FoxOrm.Save(typeDocument);
                var typeDocumentGenerated = await UIManager.GetInstance().getFoxInstance().FoxOrm
                    .Query<TypeDocument>(elmt =>
                        elmt.TypeName == typeDocument.TypeName && elmt.Author == _player.character.Id);
                foreach (var bizId in bizAllowed)
                {
                    var typeDocBizAllowed = new TypeDocBizAllowed()
                    {
                        BizAllowedId = bizId,
                        TypeDocumentId = typeDocumentGenerated[0].Id
                    };
                    await UIManager.GetInstance().getFoxInstance().FoxOrm.Save<TypeDocBizAllowed>(typeDocBizAllowed);
                }

                bizAllowed.Clear();
                PanelManager.NextPanel(_player, panel, _previousAction);
            }

            panel.AddButton("Valider", Action);
            panel.AddButton("Retour", ui => PanelManager.NextPanel(_player, panel, _previousAction));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, _player));
            _player.ShowPanelUI(panel);
        }

        private void Autorize()
        {
            UIPanel panel = new UIPanel("Selectionner les type entreprise autorisés", UIPanel.PanelType.Tab);

            Nova.biz.LoadBizs();
            foreach (var biz in Nova.biz.bizs)
            {
                if (bizAllowed.Contains(biz.Id))
                {
                    panel.AddTabLine("<color=#85DF6A>" + biz.BizName + "</color>", ui => { });
                }
                else
                {
                    panel.AddTabLine(biz.BizName, ui => { });
                }
            }

            panel.AddButton("Valider", ui =>
            {
                if (bizAllowed.Contains(Nova.biz.bizs[ui.selectedTab].Id))
                {
                    bizAllowed.Remove(Nova.biz.bizs[ui.selectedTab].Id);
                }
                else
                {
                    bizAllowed.Add(Nova.biz.bizs[ui.selectedTab].Id);
                }
                PanelManager.NextPanel(_player, panel, Autorize);
            });
            panel.AddButton("Retour", ui =>
                PanelManager.NextPanel(_player, panel, Display));
            _player.ShowPanelUI(panel);
        }

        private void EditField(string fieldName)
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
                else
                    PanelManager.Notification(_player, "Erreur", "Vous devez renseigner une valeur",
                        NotificationManager.Type.Error);
            });
            inputPanel.AddButton("Fermer", (ui) => PanelManager.Quit(ui, _player));

            _player.ShowPanelUI(inputPanel);
        }
    }
}