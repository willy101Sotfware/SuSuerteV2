﻿
using SuSuerteV2.UserControls.ClassDiffrenceTextBox;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VirtualKeyboard.Wpf.Controls;
using VirtualKeyboard.Wpf.Types;
using VirtualKeyboard.Wpf.ViewModels;
using VirtualKeyboard.Wpf.Views;

namespace VirtualKeyboard.Wpf
{
    public static class VKeyboard
    {
        private const string _keyboardValueName = "KeyboardValueContent";
        private const string _keyboardName = "KeyboardContent";

        private static Type _hostType = typeof(DefaultKeyboardHost);

        private static TaskCompletionSource<string> _tcs;
        private static Window _windowHost;
        private static KeyboardType keyboardType;

        public static void Config(Type hostType)
        {
            if (hostType.IsSubclassOf(typeof(Window))) _hostType = hostType;
            else throw new ArgumentException();
        }


        public static void Listen<T>(Expression<Func<T, string>> property) where T : UIElement
        {
            EventManager.RegisterClassHandler(typeof(T), UIElement.PreviewMouseLeftButtonDownEvent, (RoutedEventHandler)(async (s, e) =>
            {
                if (s is AdvancedTextBox) return;

                // Verificar si el UIElement tiene un nombre específico o una propiedad personalizada
                if (s is TextBox textBox)
                {
                    // Si el TextBox tiene una propiedad que indica que no se debe abrir el teclado
                    if (textBox.Name == "txtSegundoNombre" || textBox.Name == "txtNumCel" || textBox.Name == "txtEmaill" || textBox.Name == "Mail" || textBox.Name == "txtCelularForm" || textBox.Name == "txtEmailForm" || textBox.Name == "txtNombreRegistroCel" || textBox.Name == "txtApellidoRegistroCel" || textBox.Name == "txtSegundoApellidoRegistroCel" || textBox.Name == "txtNumIdRegistroCel")
                    {
                        // Reemplazo de switch con if-else
                        if (s is NumericBox)
                        {
                            keyboardType = KeyboardType.Number;
                        }
                        else if (s is EmailTextbox)
                        {
                            keyboardType = KeyboardType.Email;
                        }
                        else
                        {
                            keyboardType = KeyboardType.Alphabet;
                        }
                        var memberSelectorExpression = property.Body as MemberExpression;
                        if (memberSelectorExpression == null) return;
                        var prop = memberSelectorExpression.Member as PropertyInfo;
                        if (prop == null) return;
                        var initValue = (string)prop.GetValue(s);
                        var value = await OpenAsync(initValue);
                        prop.SetValue(s, value, null);


                    }
                    else
                    {
                        return; // No abrir el teclado
                    }
                }


            }));
        }


       

        public static Task<string> OpenAsync(string initialValue = "")
        {
            if (_windowHost != null) throw new InvalidOperationException();

            _tcs = new TaskCompletionSource<string>();
            _windowHost = (Window)Activator.CreateInstance(_hostType);

            switch (keyboardType)
            {
    
                case (KeyboardType.Number):
                    _windowHost.DataContext = new VirtualKeyboardNumberViewModel(initialValue);



                    break;
                case (KeyboardType.Email):
                    _windowHost.DataContext = new VirtualKeyBoardEmailViewModel(initialValue);

                    break;           
                default:
                    _windowHost.DataContext = new VirtualKeyboardViewModel(initialValue);



                    break;

            }

            ((ContentControl)_windowHost.FindName(_keyboardValueName)).Content = new KeyboardValueView();
            ((ContentControl)_windowHost.FindName(_keyboardName)).Content = new VirtualKeyboardView();
            void handler(object s, CancelEventArgs a)
            {
                var result = GetResult();
                ((Window)s).Closing -= handler;
                _windowHost = null;
                _tcs?.SetResult(result);
                _tcs = null;
            }

            _windowHost.Closing += handler;

            _windowHost.Owner = Application.Current.MainWindow;
            _windowHost.Show();
            return _tcs.Task;
        }

        public static Task<string> OpenAsync(Window windowHost, string initialValue = "")
        {
            if (_windowHost != null) throw new InvalidOperationException();

            _tcs = new TaskCompletionSource<string>();
            _windowHost = (Window)Activator.CreateInstance(_hostType);


            switch (keyboardType)
            {

                case (KeyboardType.Number):
                    _windowHost.DataContext = new VirtualKeyboardNumberViewModel(initialValue);
                    break;
                case (KeyboardType.Email):
                    _windowHost.DataContext = new VirtualKeyBoardEmailViewModel(initialValue);
                    break;
                default:
                    _windowHost.DataContext = new VirtualKeyboardViewModel(initialValue);
                    break;

            }

            ((ContentControl)_windowHost.FindName(_keyboardValueName)).Content = new KeyboardValueView();
            ((ContentControl)_windowHost.FindName(_keyboardName)).Content = new VirtualKeyboardView();
            void handler(object s, CancelEventArgs a)
            {
                var result = GetResult();
                ((Window)s).Closing -= handler;
                _windowHost = null;
                _tcs?.SetResult(result);
                _tcs = null;
            }

            _windowHost.Closing += handler;

            _windowHost.Owner = windowHost;
            _windowHost.Show();
            return _tcs.Task;
        }

        public static void Close()
        {
            if (_windowHost == null) return;

            _windowHost.Close();
        }

        private static string GetResult()
        {
            var viewModel = (VirtualKeyboardViewModel)_windowHost.DataContext;
            return viewModel.KeyboardText;
        }
    }
}
