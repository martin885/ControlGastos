using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Behaviors
{
   public class DateSelected:Behavior<DatePicker>
    {
        public static readonly BindableProperty
         CommandProperty = BindableProperty.Create(
             propertyName: "Command",
             returnType: typeof(ICommand),
             declaringType: typeof(DateSelected));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(DatePicker bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.DateSelected += Bindable_Completed;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }


        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var ey = sender as DatePicker;
            BindingContext = ey.BindingContext;
        }

        private void Bindable_Completed(object sender, EventArgs e)
        {
            try
            {
                Command.Execute(null);
            }
            catch
            {
                return;
            }
            //var ey = sender as Entry;
            //var cd = ey.BindingContext as ProdSubDefinirViewModel;
            //cd.Complete.Execute(null);
        }
        protected override void OnDetachingFrom(DatePicker bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.DateSelected -= Bindable_Completed;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }
    }
}
