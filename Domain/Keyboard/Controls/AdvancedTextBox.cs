using System.Windows;
using System.Windows.Controls;

namespace VirtualKeyboard.Wpf.Controls
{
    public class AdvancedTextBox : TextBox
    {
        public int CaretPosition
        {
            get { return (int)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
        }

        public static readonly DependencyProperty CaretPositionProperty =
            DependencyProperty.Register("CaretPosition", typeof(int), typeof(AdvancedTextBox),
                new PropertyMetadata(0, OnCaretPositionChange));

        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(string), typeof(AdvancedTextBox),
                new PropertyMetadata("", OnSelectedTextChange));

        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        public static readonly DependencyProperty TextValueProperty =
            DependencyProperty.Register("TextValue", typeof(string), typeof(AdvancedTextBox),
                new PropertyMetadata("", OnTextValueChange));

        public int CharacterLimit
        {
            get { return (int)GetValue(CharacterLimitProperty); }
            set { SetValue(CharacterLimitProperty, value); }
        }

        public static readonly DependencyProperty CharacterLimitProperty =
            DependencyProperty.Register("CharacterLimit", typeof(int), typeof(AdvancedTextBox),
                new PropertyMetadata(20));

        public double MinFontSize
        {
            get { return (double)GetValue(MinFontSizeProperty); }
            set { SetValue(MinFontSizeProperty, value); }
        }

        public static readonly DependencyProperty MinFontSizeProperty =
            DependencyProperty.Register("MinFontSize", typeof(double), typeof(AdvancedTextBox),
                new PropertyMetadata(8.0));

        public AdvancedTextBox()
        {
            SelectionChanged += AdvancedTextBox_SelectionChanged;
            TextChanged += (s, e) => SetValue(TextValueProperty, Text);
            TextWrapping = TextWrapping.Wrap; // Ajusta el texto automáticamente
            AcceptsReturn = true; // Permite múltiples líneas
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto; // Desplazamiento vertical si es necesario
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled; // Evita desplazamiento horizontal
        }

        private void AdvancedTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetValue(CaretPositionProperty, CaretIndex);
            SetValue(SelectedValueProperty, SelectedText);
        }

        private static void OnCaretPositionChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            int? value = e.NewValue as int?;
            ((TextBox)sender).CaretIndex = value ?? 0;
        }

        private static void OnSelectedTextChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string value = e.NewValue as string;
            ((TextBox)sender).SelectedText = value ?? "";
        }

        private static void OnTextValueChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (AdvancedTextBox)sender;
            int caretPosition = s.CaretPosition;
            string value = e.NewValue as string;
            s.Text = value;
            s.CaretIndex = caretPosition <= value.Length ? caretPosition : value.Length;
            s.AdjustFontSize();
        }

        private void AdjustFontSize()
        {
            // Verifica si el tamaño actual del texto excede el límite permitido
            if (Text.Length <= CharacterLimit)
            {
                FontSize = 40; // Tamaño inicial de fuente
                return;
            }

            // Ajusta el tamaño de la fuente, pero asegúrate de no reducirla en exceso
            double fontSize = 40 - (Text.Length - CharacterLimit) * 0.5; // Ajusta el factor de reducción
            fontSize = Math.Max(fontSize, MinFontSize); // Asegura el tamaño mínimo de fuente

            // En lugar de solo reducir el tamaño de fuente, considera habilitar un desplazamiento
            if (fontSize == MinFontSize)
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                TextWrapping = TextWrapping.Wrap;
            }

            FontSize = fontSize;
        }
    }
}
