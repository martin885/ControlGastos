using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlGastos.Models;

namespace ControlGastos.ViewModels
{
    public class DeleteViewModel
    {
        private Balance balance;



        public DeleteViewModel(Balance balance)
        {
            this.balance = balance;
            var balanceViewModel = BalanceViewModel.GetInstance();
            balanceViewModel.Delete(this.balance);
        }
    }
}
