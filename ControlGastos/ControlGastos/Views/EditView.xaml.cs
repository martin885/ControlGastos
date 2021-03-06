﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControlGastos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditView : ContentPage
    {
       

        public EditView()
        {

            InitializeComponent();
            instance = this;
        }

        #region Singleton

        static EditView instance;

        public static EditView GetInstance()
        {
            if (instance == null)
            {
                return new EditView();
            }
            return instance;
        }
        #endregion


    }
}