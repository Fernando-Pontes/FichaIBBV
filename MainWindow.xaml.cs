using FichaIBBV.Models;
using FichaIBBV.Services;
using FichaIBBV.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Data.Json;

namespace FichaIbbv
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IMembrosService _membrosService;
        private IEnderecosService _enderecosService;
        private INaoMembroService _naoMembroService;
        private IMembros_NaoMembrosService _membros_NaoMembrosService;

        private List<NaoMembrosModel> lstNaoMembros = new List<NaoMembrosModel>();

        public MainWindow(IMembrosService membrosService,
            IEnderecosService enderecosService,
            INaoMembroService naoMembroService,
            IMembros_NaoMembrosService membros_NaoMembrosService)
        {
            _membrosService = membrosService;
            _enderecosService = enderecosService;
            _naoMembroService = naoMembroService;
            _membros_NaoMembrosService = membros_NaoMembrosService;

            InitializeComponent();

            dgMembros.ItemsSource = _membrosService.GetAll();

            List<EnderecosModel> lstEnd = _enderecosService.GetAll();
            inclusao_cmbEnderecos.ItemsSource = null;
            inclusao_cmbEnderecos.ItemsSource = lstEnd;

            inclusao_naoMembros_dgNaoMembros.ItemsSource = _naoMembroService.GetAll().ToList<NaoMembrosModel>();
        }

        private void dgMembros_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            DataGrid dataGrid = (DataGrid)sender;

            gvNaoMembros.ItemsSource = null;

            if (dataGrid.SelectedItem != null)
            {
                dynamic row = ((DataGrid)sender).SelectedItem;

                var selMembro = _membrosService.GetById(row.IdMembros);

                txtNome.Text = selMembro.Nome;
                dtNasc.SelectedDate = selMembro.DataNasc;

                if (selMembro.DataBatismo == DateTime.MinValue)
                {
                    DateTime? emptyDate = null;
                    dtBatismo.SelectedDate = emptyDate;
                    dtBatismo.Text = "Selecione uma data";
                }
                else
                {
                    dtBatismo.SelectedDate = selMembro.DataBatismo;
                }
                txtTel.Text = selMembro.Telefone;
                txtEmail.Text = selMembro.Email;
                txtRG.Text = selMembro.Rg;
                txtEndereco.Text = selMembro.Endereco.Logradouro + ", " + selMembro.Endereco.Numero;
                txtCep.Text = selMembro.Endereco.Cep;
                txtBairro.Text = selMembro.Endereco.Bairro;
                txtLocalidade.Text = selMembro.Endereco.Localidade;
                txtUf.Text = selMembro.Endereco.Uf;
                txtComplemento.Text = selMembro.Endereco.Complemento;

                gvNaoMembros.ItemsSource = _naoMembroService.GetByIdMembro(selMembro.IdMembros);
            }
        }


        private void LimparForm()
        {
            foreach (TextBox textBox in FindInVisualTreeDown(this.dados, typeof(TextBox)))
            {
                textBox.Clear();
            }

            foreach (TextBox textBox in FindInVisualTreeDown(this.endereco, typeof(TextBox)))
            {
                textBox.Clear();
            }

            foreach(TextBox textBox in FindInVisualTreeDown(this.inclusao_dados, typeof(TextBox)))
            {
                textBox.Clear();
            }

            foreach (TextBox textBox in FindInVisualTreeDown(this.inclusao_endereco, typeof(TextBox)))
            {
                textBox.Clear();
            }

            foreach (TextBox textBox in FindInVisualTreeDown(this.inclusao_nao_membros, typeof(TextBox)))
            {
                textBox.Clear();
            }

            foreach (DatePicker datePicker in FindInVisualTreeDown(this.dados, typeof(DatePicker)))
            {
                DateTime? emptyDate = null;
                datePicker.SelectedDate = emptyDate;
                datePicker.Text = "Selecione uma data";
            }

            foreach (DatePicker datePicker in FindInVisualTreeDown(this.inclusao_dados, typeof(DatePicker)))
            {
                DateTime? emptyDate = null;
                datePicker.SelectedDate = emptyDate;
                datePicker.Text = "Selecione uma data";
            }

            foreach (DatePicker datePicker in FindInVisualTreeDown(this.inclusao_nao_membros, typeof(DatePicker)))
            {
                DateTime? emptyDate = null;
                datePicker.SelectedDate = emptyDate;
                datePicker.Text = "Selecione uma data";
            }

            foreach (CheckBox checkBox in FindInVisualTreeDown(this.inclusao_endereco, typeof(CheckBox)))
            {
                checkBox.IsChecked = false;
            }

            foreach (CheckBox checkBox in FindInVisualTreeDown(this.inclusao_nao_membros, typeof(CheckBox)))
            {
                checkBox.IsChecked = false;
            }

            foreach (ComboBox combo in FindInVisualTreeDown(this.inclusao_endereco,typeof(ComboBox)))
            {
                combo.SelectedIndex = -1;
            }
        }

        private static IEnumerable<DependencyObject> FindInVisualTreeDown(DependencyObject obj, Type type)
        {
            if (obj != null)
            {
                if (obj.GetType() == type)
                {
                    yield return obj;
                }

                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    foreach (var child in FindInVisualTreeDown(VisualTreeHelper.GetChild(obj, i), type))
                    {
                        if (child != null)
                        {
                            yield return child;
                        }
                    }
                }
            }

            yield break;
        }

        private void consultaNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            LimparForm();

            string txt = ((TextBox)sender).Text;
            dgMembros.ItemsSource = _membrosService.GetByNome(txt);
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            MembrosModel novoMembro = new MembrosModel();

            if (inclusao_dtBatismo.SelectedDate != null)
            {
                novoMembro.DataBatismo = inclusao_dtBatismo.SelectedDate.Value;
            }
            novoMembro.DataNasc = inclusao_dtNasc.SelectedDate.Value;
            novoMembro.Email = inclusao_txtEmail.Text;
            novoMembro.Nome = inclusao_txtNome.Text;
            novoMembro.Rg = inclusao_txtRG.Text;
            novoMembro.Telefone = inclusao_txtTel.Text;
            if (inclusao_chkEndereco.IsChecked.Value)
            {
                novoMembro.Endereco = new EnderecosModel();
                novoMembro.Endereco.Bairro = inclusao_txtBairro.Text;
                novoMembro.Endereco.Cep = inclusao_txtCep.Text;
                novoMembro.Endereco.Logradouro = inclusao_txtEndereco.Text;
                novoMembro.Endereco.Localidade = inclusao_txtLocalidade.Text;
                novoMembro.Endereco.Numero = inclusao_txtNumero.Text;
                novoMembro.Endereco.Complemento = inclusao_txtComplemento.Text;
                novoMembro.Endereco.Uf = inclusao_txtUf.Text;
            }
            else
            {
                novoMembro.IdEnderecos = ((EnderecosModel)inclusao_cmbEnderecos.SelectedItem).IdEnderecos;
            }


            if (inclusao_ChkNaoMembro.IsChecked.Value)
            {
                foreach (NaoMembrosModel nm in lstNaoMembros)
                {
                    nm.IdNao_Membros = _naoMembroService.Insert(nm);
                }
            }

            novoMembro.IdMembros = _membrosService.Insert(novoMembro);

            if (lstNaoMembros.Count > 0)
            {
                foreach (NaoMembrosModel nm in lstNaoMembros)
                {
                    Membros_NaoMembrosModel m = new Membros_NaoMembrosModel()
                    {
                        IdMembros = novoMembro.IdMembros,
                        IdNao_Membros = nm.IdNao_Membros
                    };

                    _membros_NaoMembrosService.Insert(m);
                };
            }
            else
            {
                if(inclusao_naoMembros_dgNaoMembros.SelectedItems.Count > 0)
                {
                    foreach (NaoMembrosModel nm in inclusao_naoMembros_dgNaoMembros.SelectedItems)
                    {
                        Membros_NaoMembrosModel m = new Membros_NaoMembrosModel()
                        {
                            IdMembros = novoMembro.IdMembros,
                            IdNao_Membros = nm.IdNao_Membros
                        };

                        _membros_NaoMembrosService.Insert(m);
                    }
                }
            }
            LimparForm();

            dgMembros.ItemsSource = _membrosService.GetAll();
            dgMembros.Items.Refresh();

            inclusao_naoMembros_dgNaoMembros.ItemsSource = _naoMembroService.GetAll();
            inclusao_naoMembros_dgNaoMembros.Items.Refresh();

            tbCtrl.SelectedIndex = 0;
        }

        private void inclusao_txtCep_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox txtSender = (TextBox)sender;

                if (!String.IsNullOrEmpty(txtSender.Text))
                {
                    using (WebClient wb = new WebClient())
                    {
                        Uri uri = new Uri($"https://viacep.com.br/ws/{txtSender.Text}/json/");
                        string cepRetorno = wb.DownloadString(uri);
                        ViaCepModel jsonRetorno = JsonConvert.DeserializeObject<ViaCepModel>(cepRetorno);
                        inclusao_txtEndereco.Text = jsonRetorno.logradouro;
                        inclusao_txtBairro.Text = jsonRetorno.bairro;
                        inclusao_txtCep.Text = jsonRetorno.cep;
                        inclusao_txtLocalidade.Text = jsonRetorno.localidade;
                        inclusao_txtUf.Text = jsonRetorno.uf;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = (CheckBox)sender;

            inclusao_txtCep.Text = string.Empty;
            inclusao_txtEndereco.Text = string.Empty;
            inclusao_txtNumero.Text = string.Empty;
            inclusao_txtBairro.Text = string.Empty;
            inclusao_txtLocalidade.Text = string.Empty;
            inclusao_txtUf.Text = string.Empty;
            inclusao_txtComplemento.Text = string.Empty;

            inclusao_cmbEnderecos.IsEnabled = !chkSender.IsChecked.Value;
            if (!inclusao_cmbEnderecos.IsEnabled)
            {
                inclusao_cmbEnderecos.Text = string.Empty;
            }
            inclusao_txtCep.IsEnabled = chkSender.IsChecked.Value;
            inclusao_txtNumero.IsEnabled = chkSender.IsChecked.Value;
            inclusao_txtComplemento.IsEnabled = chkSender.IsChecked.Value;
        }

        private void inclusao_cmbEnderecos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbSender = (ComboBox)sender;

            EnderecosModel selEndereco = (EnderecosModel)cmbSender.SelectedItem;

            if (selEndereco != null)
            {
                inclusao_txtCep.Text = selEndereco.Cep;
                inclusao_txtEndereco.Text = selEndereco.Logradouro;
                inclusao_txtNumero.Text = selEndereco.Numero;
                inclusao_txtBairro.Text = selEndereco.Bairro;
                inclusao_txtLocalidade.Text = selEndereco.Localidade;
                inclusao_txtUf.Text = selEndereco.Uf;
                inclusao_txtComplemento.Text = selEndereco.Complemento;
            }
        }

        private void inclusao_naoMembros_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NaoMembrosModel naoMembros = new NaoMembrosModel()
            {
                Nome = inclusao_naoMembro_txtNome.Text,
                DataNasc = inclusao_naoMembros_dpDtNasc.SelectedDate.Value
            };

            lstNaoMembros.Add(naoMembros);

            inclusao_naoMembros_dgNaoMembros.ItemsSource = lstNaoMembros;
            inclusao_naoMembros_dgNaoMembros.Items.Refresh();
        }

        private void inclusao_txtPesquisarNaoMembro_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtSender = (TextBox)sender;
            inclusao_naoMembros_dgNaoMembros.ItemsSource = _naoMembroService.GetByNome(txtSender.Text);
        }


        private void inclusao_ChkNaoMembro_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = (CheckBox)sender;

            inclusao_txtPesquisarNaoMembro.IsEnabled = chkSender.IsChecked.Value;

            inclusao_naoMembro_txtNome.IsEnabled = chkSender.IsChecked.Value;
            inclusao_naoMembros_dpDtNasc.IsEnabled = chkSender.IsChecked.Value;

            if (!chkSender.IsChecked.Value)
            {
                inclusao_naoMembros_dgNaoMembros.ItemsSource = _naoMembroService.GetAll();
                inclusao_naoMembros_dgNaoMembros.IsEnabled = true;
            }
            else
            {
                inclusao_naoMembros_dgNaoMembros.ItemsSource = lstNaoMembros;
                inclusao_naoMembros_dgNaoMembros.IsEnabled = false;

            }
        }
    }
}
