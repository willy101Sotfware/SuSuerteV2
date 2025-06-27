using Newtonsoft.Json;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.ApiService;
using SuSuerteV2.Domain.ApiService.IntegrationModels;
using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Modals;
using SuSuerteV2.UserControls;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SuSuerteV2.Presentation.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para SelectNumUC.xaml
    /// </summary>
    public partial class SelectNumUC : AppUserControl
    {
        Transaction _ts;
        private List<TextBox> textBoxes = new List<TextBox>();
        private SelectNumViewModel viewModel = new SelectNumViewModel();
        private ApiIntegration _apiIntegration;

        public SelectNumUC()
        {
            InitializeComponent();


            foreach (UIElement item in mainContainer.Children)
            {
                if (item is TextBox texBox)
                {
                    textBoxes.Add(texBox);

                }
            }
            _ts = Transaction.Instance;
            _ts.TypeChance = new List<Listadotipochancefield>();

            if (_ts.ListaChances==null)
            {
                _ts.ListaChances=new List<Transaction.Chance>();
            }
            if (_ts.Editar)
            {
                LoadEditChance();
            }
            imgLoteria.Source = _ts.ListaLoteriasSeleccionadas[0].ImageS;
            if (_ts.ListaLoteriasSeleccionadas.Count >= 2)
            {
                viewModel.ImgLot2 = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "LoteriasS", _ts.ListaLoteriasSeleccionadas[1].Nombre + ".png");
            }
            if (_ts.ListaLoteriasSeleccionadas.Count >= 3)
            {
                viewModel.ImgLot3 = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "LoteriasS", _ts.ListaLoteriasSeleccionadas[2].Nombre + ".png");
            }

            txtValidaciones.Text = "";


        }


        private void LoadEditChance()
        {
            try
            {

                string numChance = _ts.ListaChances[_ts.IndexChanceToEdit].Numero;

                if (numChance.Length == 4)
                {
                    Num1.Text = numChance.Substring(0, 1);
                    Num2.Text = numChance.Substring(1, 1);
                    Num3.Text = numChance.Substring(2, 1);
                    Num4.Text = numChance.Substring(3, 1);
                }
                if (numChance.Length == 3)
                {
                    Num1.Text = "";
                    Num2.Text = numChance.Substring(0, 1);
                    Num3.Text = numChance.Substring(1, 1);
                    Num4.Text = numChance.Substring(2, 1);
                }

                viewModel.ValorDirecto = _ts.ListaChances[_ts.IndexChanceToEdit].Directo;
                viewModel.ValorCombinado = _ts.ListaChances[_ts.IndexChanceToEdit].Combinado;
                viewModel.ValorPata = _ts.ListaChances[_ts.IndexChanceToEdit].Pata;
                viewModel.ValorUna = _ts.ListaChances[_ts.IndexChanceToEdit].Una;

                _ts.ListaChances.RemoveAt(_ts.IndexChanceToEdit);


            }
            catch (Exception ex)
            {

            }


        }

        private TextBox GetTextBoxFocused()
        {

            return textBoxes.Where(txtBox => txtBox.IsFocused).FirstOrDefault();
        }


        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsEnabled = false;
            });
        }

        public bool IsDataValid()
        {
            try
            {

                // Validar campos obligatorios
                foreach (var txtBox in textBoxes)
                {
                    // Num1 es opcional, si no se coloca, juega solo 3 cifras
                    if (txtBox.Name.StartsWith("Num"))
                    {
                        if (txtBox.Name.Equals("Num1")) continue;

                        if (txtBox.Text.Equals(string.Empty))
                        {
                            txtValidaciones.Text = $"*No ha ingresado el digito {txtBox.Name.Substring(3)}";
                            return false;
                        }
                        continue;
                    }

                    // Validamos si hay valores en alguno de los dos Directo o Combinado
                    if (viewModel.ValorDirecto == 0 && viewModel.ValorCombinado == 0)
                    {
                        txtValidaciones.Text = $"Se debe ingresar valor de apuesta a Directo o Combinado";
                        return false;
                    }

                    // Validamos que si haya texto en los txtBox y que no haya caracteres raros
                    string caracteresSpeciales = @"[!@#%^&*'""`;]";
                    if (txtBox.Text.Equals(string.Empty) || Regex.IsMatch(txtBox.Text, caracteresSpeciales))
                    {
                        txtValidaciones.Text = $"Valor invalido en {txtBox.Name.Substring(3)}";
                        return false;
                    }
                }

                //Validar que las cantidades sean mayores a 500 y que sean multiplo de 100
                foreach (var txtBox in textBoxes)
                {
                    if (txtBox.Name.StartsWith("Num")) continue;

                    var txtVal = txtBox.Text;
                    txtVal = new string(txtVal.Where(char.IsDigit).ToArray());

                    int val = string.IsNullOrEmpty(txtVal) ? 0 : Convert.ToInt32(txtVal);



                    if (val != 0 && val < 600)
                    {
                        txtValidaciones.Text = $"La apuesta mínima a {txtBox.Name.Substring(3)} debe ser igual o superior a $600";
                        return false;
                    }

                    if (val != 0 && (val % 100) != 0)
                    {
                        txtValidaciones.Text = $"La apuesta a {txtBox.Name.Substring(3)} debe ser multiplo de $100";
                        return false;
                    }

                }

                return true;

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Info,"SelecNumUC IsDataValid Catch : " + ex.Message, null);
            
                return false;
            }



        }

        public void GuardarNumero()
        {
            try
            {
                var numChance = Num1.Text + Num2.Text + Num3.Text + Num4.Text;

                var chance = new Transaction.Chance
                {
                    Loterias = _ts.ListaLoteriasSeleccionadas,
                    FechaJuego = DateTime.ParseExact(_ts.Fecha, "dd/MM/yyyy", null),
                    Numero = numChance,
                    Directo = viewModel.ValorDirecto,
                    Combinado = viewModel.ValorCombinado,
                    Pata = viewModel.ValorPata,
                    Una = viewModel.ValorUna,
                };

                if (_ts.Editar)
                {
                    _ts.ListaChances.Insert(_ts.IndexChanceToEdit, chance);
                    _ts.Editar = false;
                    _ts.IndexChanceToEdit = 0;
                    return;
                }

                EventLogger.SaveLog(EventType.Info, "SelecNumUC GuardarNumero", JsonConvert.SerializeObject(chance));
        

                foreach (var x in _ts.TipoChance.listadotipochanceField)
                {

                    if (x.cifrasField == chance.Numero.Count())
                    {

                        chance.TipoChance = x.idField;
                    }

                }

                _ts.ListaChances.Add(chance);


            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "SelecNumUC GuardarNumero", ex.Message, null);
            

            }




        }

        private void BtnRandomNum(object sender, EventArgs e)
        {
            Random randObj = new Random();
            string numTxt = randObj.Next(1, 9999).ToString("D4");

            Num1.Text = numTxt.Substring(0, 1);
            Num2.Text = numTxt.Substring(1, 1);
            Num3.Text = numTxt.Substring(2, 1);
            Num4.Text = numTxt.Substring(3, 1);
        }


        private void KeyboardBtnDeleteAll(object sender, EventArgs e)
        {
            try
            {

                TextBox target = GetTextBoxFocused();

                switch (target.Name)
                {
                    case "txtDirecto":

                        viewModel.ValorDirecto = 0;
                        break;
                    case "txtCombinado":
                        viewModel.ValorCombinado = 0;
                        break;
                    case "txtPata":
                        viewModel.ValorPata = 0;
                        break;
                    case "txtUna":
                        viewModel.ValorUna = 0;
                        break;
                    default:
                        target.Text = "";
                        break;
                }

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "SelecNumUC KeyboardBtnDeleteAll", ex.Message, null);
      
            }
        }




        private void KeyboardBtnDelete(object sender, EventArgs e)
        {
            try
            {
                TextBox target = GetTextBoxFocused();

                if (!(target.Text.Length > 0))
                {
                    return;
                }

                string txtTarget = target.Text;
                txtTarget = new string(txtTarget.Where(char.IsDigit).ToArray());
                txtTarget = txtTarget.Remove(txtTarget.Length - 1);

                if (target.Name.StartsWith("txt"))
                {
                    txtTarget = txtTarget != string.Empty ? txtTarget : "0";
                    switch (target.Name)
                    {
                        case "txtDirecto":
                            viewModel.ValorDirecto = Convert.ToInt32(txtTarget);
                            break;
                        case "txtCombinado":
                            viewModel.ValorCombinado = Convert.ToInt32(txtTarget);
                            break;
                        case "txtPata":
                            viewModel.ValorPata = Convert.ToInt32(txtTarget);
                            break;
                        case "txtUna":
                            viewModel.ValorUna = Convert.ToInt32(txtTarget);
                            break;
                        default:
                            target.Text = txtTarget;
                            break;
                    }

                    return;
                }

                target.Text = txtTarget;



            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "SelecNumUC KeyboardBtnDelete Catch", ex.Message, null);
            }
        }

        private void KeyboardTouch(object sender, EventArgs e)
        {
            try
            {
                Image imgNum = (Image)sender;

                TextBox target = GetTextBoxFocused();

                switch (target.Name)
                {
                    case "txtDirecto":
                        viewModel.ValorDirecto = Convert.ToInt32(viewModel.ValorDirecto.ToString() + imgNum.Tag);
                        break;
                    case "txtCombinado":
                        viewModel.ValorCombinado = Convert.ToInt32(viewModel.ValorCombinado.ToString() + imgNum.Tag);
                        break;
                    case "txtPata":
                        viewModel.ValorPata = Convert.ToInt32(viewModel.ValorPata.ToString() + imgNum.Tag);
                        break;
                    case "txtUna":
                        viewModel.ValorUna = Convert.ToInt32(viewModel.ValorUna.ToString() + imgNum.Tag);
                        break;
                    default:
                        target.Text = imgNum.Tag.ToString();
                        break;
                }

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "SelecNumUC KeyboardTouch", ex.Message, null);
            }
        }


        public void ValidateChance()
        {
            try
            {
                int LotId = 1;
                List<ApuestasValidate> LstApuestas = new List<ApuestasValidate>();

                List<LoteriaValidate> LstLoteria = new List<LoteriaValidate>();

                foreach (var x in _ts.ListaChances)
                {
                    EventLogger.SaveLog(EventType.Info, "SelecNumUC ValidateChance", JsonConvert.SerializeObject(x));
                  

                    ApuestasValidate Apuestas = new ApuestasValidate();

                    Apuestas.id = LotId++;
                    Apuestas.NumeroApostado = x.Numero;
                    Apuestas.ValorDirecto = x.Directo;
                    Apuestas.ValorCombinado = x.Combinado;
                    Apuestas.ValorPata = x.Pata;
                    Apuestas.ValorUna = x.Una;
                    Apuestas.tipoChance = new TipoChanceValidateM
                    {
                        Id = x.TipoChance
                    };


                    foreach (var y in x.Loterias)
                    {
                        LoteriaValidate Lot = new LoteriaValidate();

                        Lot.codigo = Convert.ToInt32(y.CodigoCodesa);

                        LstLoteria.Add(Lot);
                    }


                    Apuestas.ListLoteriasValidate = new ListLoteriasValidate
                    {
                        loteria = new List<LoteriaValidate>()
                    };

                    Apuestas.ListLoteriasValidate.loteria = LstLoteria;

                    LstApuestas.Add(Apuestas);
                }

                RequestValidateChance Request = new RequestValidateChance();

                Request.Subproducto = new SubproductoValidate()
                {

                    CodigoServicio = _ts.ProductSelected.Codigoservicio,
                    Id = _ts.ProductSelected.Id

                };

                Request.LstApuestas = new ListApuestasValidate
                {
                    apuestas = new List<ApuestasValidate>()
                };

                Request.AsumeIva = false;

                Request.FechaSorteo = _ts.Fecha;

                Request.Transaccion = _ts.IdTransaccionApi;

                Request.LstApuestas.apuestas = LstApuestas;

                Task.Run(async () =>
                {


                    var Response = await ApiIntegration.ValidateChance(Request);

                     _nav.CloseModal();

                    if (Response != null)
                    {

                        if (Response.Estado)
                        {

                            _ts.ApuestaValidate = Response.Chance.Listadoapuestas.Apuesta;
                            _nav.NavigateTo(new VerificarUC());
                  
                        }
                        else
                        {
                            _nav.ShowModal("No se pudo validar el chance de manera correcta, intenta nuevamente", ModalType.Error);
                            _nav.NavigateTo(new MenuUC());
                       

                        }


                    }
                    else
                    {
                        _nav.ShowModal("No se pudo validar el chance de manera correcta, intenta nuevamente", ModalType.Error);
                        _nav.NavigateTo(new MenuUC());
                    
                    }


                });

                _nav.ShowModal("Estamos Validando la información un momento por favor", ModalType.Loading);

            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "SelecNumUC ValidateChance Catch", ex.Message, null);
               
                _nav.ShowModal("No se pudo validar el chance de manera correcta, intenta nuevamente", ModalType.Error);
                _nav.NavigateTo(new MenuUC());
             
            }
        }


        private void BtnCancelar(object sender, EventArgs e)
        {
            bool noContinuar = _nav.ShowModal("¿Está seguro que desea hacer una nueva apuesta? Las apuestas que tenga en cola serán eliminadas.", ModalType.Info);
            if (noContinuar)
            {
                _ts.ListaChances = null;
                _ts.Editar = false;
                _ts.IndexChanceToEdit = 0;
                _nav.NavigateTo(new DateUC());
              
            }
        }

        private void BtnContinuar(object sender, EventArgs e)
        {
            if (IsDataValid())
            {

                InhabilitarVista();

                GuardarNumero();

                ValidateChance();
            }
        }


        private void BtnAddNum(object sender, EventArgs e)
        {
            if (IsDataValid())
            {

                if (_ts.ListaChances.Count >= 6)
                {
                    _nav.ShowModal("ya tienes el limite máximo de números apostados", ModalType.Error);
                    return;
                }

                GuardarNumero();
                _nav.NavigateTo(new ApuestaUC());
                
            }
        }

        private void BtnAtras(object sender, EventArgs e)
        {
            //StopTimer();
            InhabilitarVista();
            Task.Run(() =>
            {
                Thread.Sleep(100);
                _nav.NavigateTo(new DateUC());
              
            });
        }


        private void Num1_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num2.Focus();
        }

        private void Num2_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num3.Focus();
        }

        private void Num3_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num3_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num4.Focus();
        }

        private void Num4_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num4_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtDirecto.Focus();
        }


    }
    internal class SelectNumViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _valorDirecto = 0;
        public int ValorDirecto
        {
            get
            {
                return _valorDirecto;
            }
            set
            {
                _valorDirecto = value;
                OnPropertyChanged(nameof(ValorDirecto));
            }
        }
        private int _valorCombinado = 0;
        public int ValorCombinado
        {
            get
            {
                return _valorCombinado;
            }
            set
            {
                _valorCombinado = value;
                OnPropertyChanged(nameof(ValorCombinado));
            }
        }
        private int _valorPata = 0;
        public int ValorPata
        {
            get
            {
                return _valorPata;
            }
            set
            {
                _valorPata = value;
                OnPropertyChanged(nameof(ValorPata));
            }
        }
        private int _valorUna = 0;

        public int ValorUna
        {
            get
            {
                return _valorUna;
            }
            set
            {
                _valorUna = value;
                OnPropertyChanged(nameof(ValorUna));
            }
        }

        private string _imgLot2 = string.Empty;

        public string ImgLot2
        {
            get
            {
                return _imgLot2;
            }
            set
            {
                _imgLot2 = value;
                OnPropertyChanged(nameof(ImgLot2));
            }
        }

        private string _imgLot3 = string.Empty;

        public string ImgLot3
        {
            get
            {
                return _imgLot3;
            }
            set
            {
                _imgLot3 = value;
                OnPropertyChanged(nameof(ImgLot3));
            }
        }

        private string _msgValidations;
        public string MsgValidations
        {
            get
            {
                return _msgValidations;
            }
            set
            {
                _msgValidations = value;
                OnPropertyChanged(nameof(MsgValidations));
            }
        }
    }
}
