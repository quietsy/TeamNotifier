using System;
using System.Collections.Generic;
using TeamNotifier.Models;
using System.ComponentModel;

namespace TeamNotifier.ViewModels
{
    public class CommandViewModel : ObservableObject
    {
        public DelegateCommand DeleteCommand { get; set; }

        public CommandViewModel()
        {
            Model = new CommandModel { CommandId = Guid.NewGuid(), CommandNumber = 0,
                Hotkey = "", Message = "", MessageColor = MessageColors.Green, IsSingle = true,
                IsLocal =false, TimeReminder = TimeSpan.Zero, IsRepeat = false };
            Model.PropertyChanged += ContainedElementChanged;
        }

        public CommandViewModel(CommandModel model)
        {
            Model = model;
            Model.PropertyChanged += ContainedElementChanged;
        }

        private void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        public CommandModel Model { get; set; }
    }
}
