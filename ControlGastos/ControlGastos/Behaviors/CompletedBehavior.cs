using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Behaviors
{
    public class CompletedBehavior:Behavior<Entry>
    {
        public static readonly BindableProperty
            CommandProperty = BindableProperty.Create(
                propertyName: "Command",
                returnType: typeof(ICommand),
                declaringType: typeof(CompletedBehavior));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Completed += Bindable_Completed;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }


        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var ey = sender as Entry;
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
        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Completed-= Bindable_Completed;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }
    }
}
