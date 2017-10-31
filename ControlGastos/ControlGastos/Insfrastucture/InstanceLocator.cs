using ControlGastos.ViewModels;


namespace ControlGastos.Infrastructure
{
    public class InstanceLocator
    {
        public InstanceLocator()
        {
            Main = new MainViewModel();
        }

        public MainViewModel Main { get; set; }

    }
}
