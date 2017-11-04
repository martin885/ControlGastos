using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Behaviors
{
    public class ToggleBehavior : Behavior<Switch>
    {
        public static readonly BindableProperty
            CommandProperty = BindableProperty.Create(
                propertyName: "Command",
                returnType: typeof(ICommand),
                declaringType: typeof(ToggleBehavior));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(Switch bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Toggled += Bindable_Toggled;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }


        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var ey = sender as Switch;
            BindingContext = ey.BindingContext;
        }

        private void Bindable_Toggled(object sender, ToggledEventArgs e)
        {
            Command.Execute(null);
            //var ey = sender as Entry;
            //var cd = ey.BindingContext as ProdSubDefinirViewModel;
            //cd.Complete.Execute(null);
        }
        protected override void OnDetachingFrom(Switch bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Toggled -= Bindable_Toggled;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }
    }
}
