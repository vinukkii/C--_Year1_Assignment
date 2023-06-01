﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBuilding
{
    public interface IDoorManager : Manager
    {
        public bool OpenDoor(int doorID);
        public bool LockDoor(int doorID);
        public bool OpenAllDoors();
        public bool LockAllDoors();

        //public string GetStatus()
        //{
        //    string status = "Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK, ";
        //    return status;
        //}
    }
}
