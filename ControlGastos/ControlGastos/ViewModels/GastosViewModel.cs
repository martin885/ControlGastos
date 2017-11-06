﻿using ControlGastos.Models;
using ControlGastos.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.ViewModels
{
    public class GastosViewModel:INotifyPropertyChanged
    {
     
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        DataService dataService;
        DialogService dialogService;
        #endregion

        #region Propiedades y Atributos
        public string Mes { get; set; }
        public DateTime Date { get; set; }
        public string OrigenGasto { get; set; }
        public string SelectedItem { get; set; }
        Gastos Gastos { get; set; }
        public List<Gastos> ListaGastos { get; set; }
        public ObservableCollection<string> PickerCategorias { get; set; }
        string _sumaGastoCategoria;
        public string SumaGastoCategoria
        {
            get
            {
                return _sumaGastoCategoria;
            }
            set
            {
                if (_sumaGastoCategoria != value)
                {
                    _sumaGastoCategoria = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SumaGastoCategoria)));
                }
            }
        }
        string _sumaGasto;
        public string SumaGasto
        {
            get
            {
                return _sumaGasto;
            }
            set
            {
                if (_sumaGasto != value)
                {
                    _sumaGasto = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SumaGasto)));
                }
            }
        }
        string _montoGasto;
        public string MontoGasto
        {
            get
            {
                return _montoGasto;
            }
            set
            {
                if (_montoGasto != value)
                {
                    _montoGasto = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MontoGasto)));
                }
            }
        }
        public string _categoria;
        public string Categoria
        {
            get
            {
                return _categoria;
            }
            set
            {
                if (_categoria != value)
                {
                    _categoria = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Categoria)));
                }
            }
        }
        #endregion

        #region Commands

        public ICommand AgregarGastoCommand
        {
            get
            {
                return new RelayCommand(AgregarGasto);
            }
        }

        private async void AgregarGasto()
        {
            //Crea el objeto Ingreso, lo agrego a la lista del mes, y después se hace la sumatoria de la lista
            if (SelectedItem == null)
            {
               await dialogService.ShowMessage("Error", "Se debe seleccionar una categoría");
                return;
            }


            Gastos = new Gastos();
            ListaGastos.Add(Gastos);
            Gastos.Anio = Date.ToString("yyyy");
            Gastos.Mes = Date.ToString("MMMM");
            Gastos.Dia = Date.ToString("dd");
            Gastos.GastoNombre = OrigenGasto;
            Gastos.Categoria = SelectedItem;
            if (MontoGasto == null)
            {
                MontoGasto = 0.ToString();
            }
            Gastos.GastosCantidad = MontoGasto;
            //Realizar la sumatoria con los ingresos pertenecientes al mes y año elegido
            SumaGasto = ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio&&x.Categoria== SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            MontoGasto = null;
            dataService.Save(ListaGastos, true);
        }
        public ICommand DateSelectedCommand
        {
            get
            {
                return new RelayCommand(DateSelected);
            }
        }

        private void DateSelected()
        {
            SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMMM") && x.Anio == Date.ToString("yyyy")).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
        }
        public ICommand SelectedItemChangedCommand
        {
            get
            {
                return new RelayCommand(SelectedItemChanged);
            }
        }

        private void SelectedItemChanged()
        {
            Categoria = SelectedItem;
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMMM") && x.Anio == Date.ToString("yyyy") && x.Categoria == SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
        }
        #endregion

        #region Constructor
        public GastosViewModel()
        {
            dataService = new DataService();
            dialogService = new DialogService();
            Cargas();

        }
        #endregion

        #region Métodos
        private void Cargas()
        {
            Categoria = "Sin Seleccionar";
            PickerCategorias = new ObservableCollection<string>();
            PickerCategorias.Add("Ocio");
            PickerCategorias.Add("Servicios");
            PickerCategorias.Add("Alimentos");
            Mes = DateTime.Now.ToString("MMMM");
            Date = DateTime.Now;
            ListaGastos = new List<Gastos>();
            if (dataService.CheckTableIsEmpty<Gastos>())
            {
                ListaGastos = dataService.Get<Gastos>(true);
                SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMMM") && x.Anio == Date.ToString("yyyy")&&x.Categoria==SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
                SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMMM") && x.Anio == Date.ToString("yyyy")).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            }
        }

        }
        #endregion

    
}

