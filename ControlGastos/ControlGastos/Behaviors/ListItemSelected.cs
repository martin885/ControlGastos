using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Behaviors
{
    public class ListItemSelected : Behavior<ListView>
    {
        public static readonly BindableProperty
            CommandProperty = BindableProperty.Create(
                propertyName: "Command",
                returnType: typeof(ICommand),
                declaringType: typeof(ListItemSelected));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.ItemSelected += Bindable_ItemSelected;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }


        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var ey = sender as ListView;
            BindingContext = ey.BindingContext;
            
        }

        private void Bindable_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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
        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.ItemSelected -= Bindable_ItemSelected;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }
    }
}