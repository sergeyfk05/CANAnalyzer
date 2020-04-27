/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.VM
{
    public abstract class BaseClosableVM : BaseVM
    {
        public event EventHandler Closed;
        protected void RaiseClosedEvent()
        {
            Closed?.Invoke(this, null);
        }
    }
}
