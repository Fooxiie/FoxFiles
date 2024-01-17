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

        private Dictionary<string, string> _finalDirectory;
        private List<int> _bizAllowed = new List<int>();
        private bool _isEditing = false;
        private int oldId = 0;

        public FormTypeDocumentPanel(string title, List<string> fields, Player player, Action previousAction)
        {
            _title = title;
            _fields = fields;
            _player = player;
            _previousAction = previousAction;

            _finalDirectory = fields.ToDictionary(x => x);
        }

        private string MakeTitle()
        {
            return $"{_title}";
        }

        public void Display()
        {
            var panel = new UIPanel(MakeTitle(), UIPanel.PanelType.Tab);
            foreach (var field in _finalDirectory)
            {
                panel.AddTabLine($"<color=#dc8031>{field.Key}</color> : {field.Value}", ui => EditField(field.Key));
            }

            panel.AddTabLine(
                "<color=#dc8031>Bizs autorisés</color>" + (_bizAllowed.Count > 0 ? $"({_bizAllowed.Count})" : ""),
                ui => PanelManager.NextPanel(_player, panel, Autorize));

            panel.AddButton("Editer", ui => ui.SelectTab());

            panel.AddButton("Valider", Save);
            panel.AddButton("Retour", ui => PanelManager.NextPanel(_player, panel, _previousAction));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, _player));
            _player.ShowPanelUI(panel);
        }

        async void Save(UIPanel ui)
        {
            var typeDocument = new TypeDocument()
            {
                TypeName = _finalDirectory["Nom"],
                Author = _player.character.Id,
                AllowedIdBiz = String.Join(",", _bizAllowed)
            };
            if (_isEditing)
                typeDocument.Id = oldId;
            await UIManager.GetInstance().getFoxInstance().FoxOrm.Save(typeDocument);

            _bizAllowed.Clear();
            PanelManager.NextPanel(_player, ui, _previousAction);
        }

        private void Autorize()
        {
            UIPanel panel = new UIPanel("Selectionner les type entreprise autorisés", UIPanel.PanelType.Tab);

            Nova.biz.LoadBizs();
            foreach (var biz in Nova.biz.bizs)
            {
                if (_bizAllowed.Contains(biz.Id))
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
                if (_bizAllowed.Contains(Nova.biz.bizs[ui.selectedTab].Id))
                {
                    _bizAllowed.Remove(Nova.biz.bizs[ui.selectedTab].Id);
                }
                else
                {
                    _bizAllowed.Add(Nova.biz.bizs[ui.selectedTab].Id);
                }

                PanelManager.NextPanel(_player, panel, Autorize);
            });
            panel.AddButton("Retour", ui =>
                PanelManager.NextPanel(_player, panel, Display));
            _player.ShowPanelUI(panel);
        }

        private void EditField(string fieldName)
        {
            if (!_finalDirectory.ContainsKey(fieldName))
            {
                _player.Notify("Erreur formulaire", "Le field n'existe pas", NotificationManager.Type.Error);
                return;
            }

            var inputPanel = new UIPanel(fieldName, UIPanel.PanelType.Input)
            {
                inputPlaceholder = "Entrer votre valeur"
            };

            inputPanel.AddButton("Confirmer", (ui) =>
            {
                if (ui.inputText.Length > 0)
                {
                    _finalDirectory[fieldName] = inputPanel.inputText;
                    PanelManager.NextPanel(_player, ui, Display);
                }
                else
                    PanelManager.Notification(_player, "Erreur", "Vous devez renseigner une valeur",
                        NotificationManager.Type.Error);
            });
            inputPanel.AddButton("Fermer", (ui) => PanelManager.Quit(ui, _player));

            _player.ShowPanelUI(inputPanel);
        }

        public async void Edit(TypeDocument typeDocument)
        {
            _finalDirectory["Nom"] = typeDocument.TypeName;
            foreach (var typeDocBizAllowed in typeDocument.AllowedIdBiz.Split(','))
            {
                _bizAllowed.Add(int.Parse(typeDocBizAllowed));
            }

            _isEditing = true;
            oldId = typeDocument.Id;
        }
    }
}