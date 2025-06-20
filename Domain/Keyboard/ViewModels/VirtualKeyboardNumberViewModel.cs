using System.ComponentModel;
using VirtualKeyboard.Wpf;
using VirtualKeyboard.Wpf.Types;
using VirtualKeyboard.Wpf.ViewModels;

namespace VirtualKeyboard.Wpf.ViewModels
{
    class VirtualKeyboardNumberViewModel : VirtualKeyboardViewModel
    {
        public Command DeleteAll { get; }


        public VirtualKeyboardNumberViewModel(string initialValue) : base(initialValue)
        {
            KeyboardType = KeyboardType.Number;
            DeleteAll = new Command(a => KeyboardText = "");

        }

    }
}
