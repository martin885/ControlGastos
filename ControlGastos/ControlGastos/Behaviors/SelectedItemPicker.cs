using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Behaviors
{
  public class SelectedItemPicker:Behavior<Picker>
    {
        public static readonly BindableProperty
         CommandProperty = BindableProperty.Create(
             propertyName: "Command",
             returnType: typeof(ICommand),
             declaringType: typeof(SelectedItemPicker));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(Picker bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.SelectedIndexChanged += Bindable_Completed;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }


        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var ey = sender as Picker;
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
        protected override void OnDetachingFrom(Picker bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.SelectedIndexChanged -= Bindable_Completed;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }
    }
}
