﻿using ControlGastos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.ViewModels
{
    public class BorrarBalanceGeneralViewModel
    {
        private BalanceGeneral balanceGeneral;

        public BorrarBalanceGeneralViewModel(BalanceGeneral balanceGeneral)
        {



            this.balanceGeneral = balanceGeneral;
            var balanceGeneralViewModel = BalanceGeneralViewModel.GetInstance();
            balanceGeneralViewModel.Delete(this.balanceGeneral);
        }
    }
}
