using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBuilding
{
   public interface IFireAlarmManager : Manager
    {
        public void SetAlarm(bool isActive);

        //public string GetStatus()
        //{
        //    string status = "FireAlarm,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK, ";
        //    return status;
        //}
    }
}
