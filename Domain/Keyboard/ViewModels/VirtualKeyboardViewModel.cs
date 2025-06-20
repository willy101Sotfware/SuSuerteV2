using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualKeyboard.Wpf.Types;

namespace VirtualKeyboard.Wpf.ViewModels
{
    class VirtualKeyboardViewModel : INotifyPropertyChanged
    {
        public bool _isPasswordVisible;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                NotifyPropertyChanged(nameof(IsPasswordVisible));
            }
        }
        public bool _isRow1Collapsed = false;
        public bool _isRow2Collapsed = false;

        public bool IsRow1Collapsed
        {
            get => _isRow1Collapsed;
            set
            {
                if (_isRow1Collapsed != value)
                {
                    _isRow1Collapsed = value;
                    NotifyPropertyChanged(nameof(IsRow1Collapsed));
                    // Ensure the other row collapses when this one is expanded
                    if (value)
                    {
                        IsRow2Collapsed = false;
                    }
                }
            }
        }

        public bool IsRow2Collapsed
        {
            get => _isRow2Collapsed;
            set
            {
                if (_isRow2Collapsed != value)
                {
                    _isRow2Collapsed = value;
                    NotifyPropertyChanged(nameof(IsRow2Collapsed));
                    // Ensure the other row collapses when this one is expanded
                    if (value)
                    {
                        IsRow1Collapsed = false;
                    }
                }
            }
        }
        public PackIconKind _iconKind = PackIconKind.EyeOff;
        public PackIconKind IconKind
        {
            get => _iconKind;
            set
            {
                if (_iconKind != value)
                {
                    _iconKind = value;
                    NotifyPropertyChanged(nameof(IconKind));
                }
            }
        }
        public string initialValue = string.Empty;

        public string _keyboardText;
        public string KeyboardText
        {
            get => _keyboardText;
            set
            {
                _keyboardText = value;
                NotifyPropertyChanged(nameof(KeyboardText));
            }
        }
        public KeyboardType _keyboardType;
        public KeyboardType KeyboardType
        {
            get => _keyboardType;
            set
            {
                _keyboardType = value;
                NotifyPropertyChanged(nameof(KeyboardType));
            }
        }

        private bool _uppercase;
        public bool Uppercase
        {
            get => _uppercase;
            private set
            {
                _uppercase = value;
                NotifyPropertyChanged(nameof(Uppercase));
            }
        }
        private int _caretPosition;
        public int CaretPosition
        {
            get => _caretPosition;
            set
            {
                _caretPosition = value;
                NotifyPropertyChanged(nameof(CaretPosition));
            }
        }
        private string _selectedValue;
        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value;
                NotifyPropertyChanged(nameof(SelectedValue));
            }
        }
        public Command AddCharacter { get; }
        public Command ChangeCasing { get; }
        public Command RemoveCharacter { get; }
        public Command ChangeKeyboardType { get; }
        public Command Accept { get; }
        public Command Cancel { get; }
        public Command DeleteAll { get; }
        public Command ShowPassworsdCommand { get; }
        public VirtualKeyboardViewModel(string initialValue)
        {
            this.initialValue = initialValue;
            _keyboardText = initialValue;
            _keyboardType = KeyboardType.Alphabet;
            _uppercase = false;
            CaretPosition = _keyboardText.Length;

            AddCharacter = new Command(a =>
            {
                if (a is string character)
                    if (character.Length == 1)
                    {
                        if (Uppercase) character = character.ToUpper();
                        if (!string.IsNullOrEmpty(SelectedValue))
                        {
                            RemoveSubstring(SelectedValue);
                            KeyboardText = KeyboardText.Insert(CaretPosition, character);
                            CaretPosition++;
                            SelectedValue = "";
                        }
                        else
                        {
                            KeyboardText = KeyboardText.Insert(CaretPosition, character);
                            CaretPosition++;
                        }
                    }
                    else if (character.Length != 0)
                    {
                        foreach (char c in character)
                        {
                            KeyboardText = KeyboardText.Insert(CaretPosition, Convert.ToString(c));
                            CaretPosition++;
                        }
                    }

            });
            ChangeCasing = new Command(a => Uppercase = !Uppercase);
            RemoveCharacter = new Command(a =>
            {
                if (!string.IsNullOrEmpty(SelectedValue))
                {
                    RemoveSubstring(SelectedValue);
                }
                else
                {
                    var position = CaretPosition - 1;
                    if (position >= 0)
                    {
                        KeyboardText = KeyboardText.Remove(position, 1);
                        if (position < KeyboardText.Length) CaretPosition--;
                        else CaretPosition = KeyboardText.Length;
                    }
                }
            });
            ChangeKeyboardType = new Command(a =>
            {
                if (KeyboardType == KeyboardType.Alphabet) KeyboardType = KeyboardType.Special;
                else KeyboardType = KeyboardType.Alphabet;


            });
            DeleteAll = new Command(a => KeyboardText = "");

            Accept = new Command(a => VKeyboard.Close());
            Cancel = new Command(a =>
            {
                KeyboardText = this.initialValue;
                VKeyboard.Close();
            });


        }

        public void RemoveSubstring(string substring)
        {
            var position = KeyboardText.IndexOf(substring);
            KeyboardText = KeyboardText.Remove(position, substring.Length);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public string ConvertLetterInAsterisk(string initialValue)
        {
            KeyboardText = "";
            foreach (char ch in initialValue)
            {
                KeyboardText += "*";

            }
            return KeyboardText;
        }

    }
}
