using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SmartBuilding
{
    public interface ILightManager : Manager
    {

        public void Setlight(bool isOn , int lightID);
        public void SetAllLights(bool isOn);
        bool SetAllLights();

        //public string GetStatus()
        //{
        //    string status = "Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK, ";
        //    return status; 
        //}



    }
}
