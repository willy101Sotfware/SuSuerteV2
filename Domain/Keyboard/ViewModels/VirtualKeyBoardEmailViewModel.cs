using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualKeyboard.Wpf;
using VirtualKeyboard.Wpf.Types;

using VirtualKeyboard.Wpf.ViewModels;

namespace VirtualKeyboard.Wpf.ViewModels
{
    class VirtualKeyBoardEmailViewModel : VirtualKeyboardViewModel
    {
        public Command ChangeKeyboardType { get; }

        public VirtualKeyBoardEmailViewModel(string initialValue) : base(initialValue)
        {
            KeyboardType = KeyboardType.Email;
            ChangeKeyboardType = new Command(a =>
            {
                if (KeyboardType == KeyboardType.Email) KeyboardType = KeyboardType.Special;
                else KeyboardType = KeyboardType.Email;


            });
        }
    }
}
