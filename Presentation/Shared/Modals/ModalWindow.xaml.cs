﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace SuSuerteV2.Modals
{
    /// <summary>
    /// Lógica de interacción para ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        private ModalViewModel _viewModel;
        public ModalWindow(ModalViewModel modal)
        {
            InitializeComponent();
            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Width = this.Height * 9 / 16;
            this.WindowVB.Height = SystemParameters.PrimaryScreenHeight;
            this.WindowVB.Width = this.Height * 9 / 16;

            this._viewModel = modal;

            this.DataContext = _viewModel;

            ConfigureModal();
        }

        private void ConfigureModal()
        {
            this.BtnOk.Visibility = _viewModel.TypeModal.BtnOkVisibility;
            this.BtnYes.Visibility = _viewModel.TypeModal.BtnYesVisibility;
            this.BtnNo.Visibility = _viewModel.TypeModal.BtnNoVisibility;
            this.LoadGif.Visibility = _viewModel.TypeModal.LoadGifVisibility;
        }

        private void BtnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {

            this.DialogResult = true;

        }

        private void BtnYes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnNo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = false;
        }
    }

    public class ModalViewModel : INotifyPropertyChanged
    {
        private string _message = string.Empty;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyRaised(nameof(Message));
            }
        }

        

        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyRaised(nameof(Title));
            }
        }

        private ModalType _typeModal;

        public ModalType TypeModal
        {
            get
            {
                return _typeModal;
            }
            set
            {
                _typeModal = value;
                OnPropertyRaised(nameof(TypeModal));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }
    }

    public class ModalType
    {
        public static ModalType Info => new InfoModal();
        public static ModalType Success => new InfoModal(); // Puedes crear SuccessModal si necesitas
        public static ModalType Warning => new InfoModal(); // Puedes crear WarningModal si necesitas
        public static ModalType Error => new InfoModal();   // Puedes crear ErrorModal si necesitas
        public static ModalType Question => new ConfirmationModal();
        public static ModalType Loading => new LoadModal();

        public Visibility BtnOkVisibility { get; set; }
        public Visibility BtnYesVisibility { get; set; }
        public Visibility BtnNoVisibility { get; set; }
        public Visibility LoadGifVisibility { get; set; }
    }

    public class InfoModal : ModalType
    {
        public InfoModal()
        {
            BtnOkVisibility = Visibility.Visible;
            BtnYesVisibility = Visibility.Collapsed;
            BtnNoVisibility = Visibility.Collapsed;
            LoadGifVisibility = Visibility.Collapsed;

        }

    }

    public class LoadModal : ModalType
    {
        public LoadModal()
        {
            BtnOkVisibility = Visibility.Collapsed;
            BtnYesVisibility = Visibility.Collapsed;
            BtnNoVisibility = Visibility.Collapsed;
            LoadGifVisibility = Visibility.Visible;

        }
    }

    public class ConfirmationModal : ModalType
    {
        public ConfirmationModal()
        {
            BtnOkVisibility = Visibility.Collapsed;
            BtnYesVisibility = Visibility.Visible;
            BtnNoVisibility = Visibility.Visible;
            LoadGifVisibility = Visibility.Collapsed;

        }
    }
}
