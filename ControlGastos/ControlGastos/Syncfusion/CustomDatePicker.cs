using Syncfusion.SfPicker.XForms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
//Agregar libreria Syncfusion.SfPicker
namespace ControlGastos.Syncfusion
{
    public class CustomDatePicker:SfPicker
    {
        #region Public Properties

        // Months API is used to modify the Day collection as per change in Month

        internal Dictionary<string, string> Months { get; set; }

        /// <summary>

        /// Date is the actual DataSource for SfPicker control which will holds the collection of Day ,Month and Year

        /// </summary>

        /// <value>The date.</value>

        public ObservableCollection<object> Date { get; set; }

        //Day is the collection of day numbers

        //internal ObservableCollection<object> Day { get; set; }

        //Month is the collection of Month Names

        internal ObservableCollection<object> Month { get; set; }

        //Year is the collection of Years from 1990 to 2042

        internal ObservableCollection<object> Year { get; set; }

        public ObservableCollection<string> Headers { get; set; }
        #endregion
        public CustomDatePicker()

        {

            Months = new Dictionary<string, string>();

            Date = new ObservableCollection<object>();

            //Day = new ObservableCollection<object>();

            Month = new ObservableCollection<object>();

            Year = new ObservableCollection<object>();

            /// <summary>

            /// Headers API is holds the column name for every column in date picker

            /// </summary>

            /// <value>The Headers.</value>

            Headers = new ObservableCollection<string>();

            Headers.Add("Month");

            //Headers.Add("Day");

            Headers.Add("Year");

            //SfPicker header text

            HeaderText = "Seleccionar Mes y Año";

            //Enable Footer

            ShowFooter = true;

            //Enable SfPicker Header

            ShowHeader = true;

            //Enable Column Header of SfPicker

            ShowColumnHeader = true;

            // Column header text collection

            this.ColumnHeaderText = Headers;

            PopulateDateCollection();

            this.ItemsSource = Date;

        }
        private void PopulateDateCollection()

        {

            //populate months

            for (int i = 1; i < 13; i++)

            {

                if (!Months.ContainsKey(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3)))

                    Months.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));

                Month.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3));

            }

            //populate year

            for (int i = 1990; i < 2050; i++)

            {

                Year.Add(i.ToString());

            }

            //populate Days

            //for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)

            //{

            //    if (i < 10)

            //    {

            //        Day.Add("0" + i);

            //    }

            //    else

            //        Day.Add(i.ToString());

            //}

            Date.Add(Month);

            //Date.Add(Day);

            Date.Add(Year);

        }

    }
}
