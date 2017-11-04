using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Behaviors
{
    public class ItemTapped : Behavior<ListView>
    {
        public static readonly BindableProperty
            CommandProperty = BindableProperty.Create(
                propertyName: "Command",
                returnType: typeof(ICommand),
                declaringType: typeof(ItemTapped));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.ItemTapped += Bindable_Completed;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }


        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var ey = sender as ListView;
            BindingContext = ey.BindingContext;
        }

        private void Bindable_Completed(object sender, ItemTappedEventArgs e)
        {
            Command.Execute(null);
            //var ey = sender as Entry;
            //var cd = ey.BindingContext as ProdSubDefinirViewModel;
            //cd.Complete.Execute(null);
        }
        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.ItemTapped -= Bindable_Completed;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }
    }
}
