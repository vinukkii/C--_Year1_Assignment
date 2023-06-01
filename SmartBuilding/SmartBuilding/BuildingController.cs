//string state1 = "closed";
//string state2 = "out of hours";
//string state3 = "open";
//string state4 = "fire drill";
//string state5 = "fire alarm";

using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace SmartBuilding
{
    // constructors 
    public class BuildingController
    {
        // fields / variables
        private string buildingID;
        private string currentState;

        //L1R1 , L1R4
        public BuildingController(string ID)
        {
            buildingID = ID.ToLower();
            currentState = "out of hours";
        }

        //L2R3
        public BuildingController(string id, string startState)
        {
            buildingID = id.ToLower();
            // buildingID = id;
            string otherState = startState.ToLower();  // to make the uppercase , lower case , or combinations to lower case 


            if ((otherState == "open") || (otherState == "closed") || (otherState == "out of hours"))
            {
                currentState = otherState; 
            }
            else
            {
                throw new ArgumentException("Argument Exception: BuildingController can only be initialised to the following states 'open', 'closed','out of hours'");
            }
            //currentState = " out of hours";



        }

         ILightManager LightManager;
         IFireAlarmManager FireAlarmManager;
         IDoorManager DoorManager;
         IWebService WebService;
         IEmailService EmailService;



        public BuildingController(string id, ILightManager iLightManager, IFireAlarmManager iFireAlarmManager, IDoorManager iDoorManager, IWebService iWebService, IEmailService iEmailService)
        {
            this.buildingID = id.ToLower();
            currentState = "out of hours";
            LightManager = iLightManager;
            FireAlarmManager = iFireAlarmManager;
            DoorManager = iDoorManager;
            WebService = iWebService;
            EmailService = iEmailService;
        }

        
        //L1R4

        public void SetBuildingID(string buildID)
        {
            buildingID = buildID.ToLower();
        }

        //L1R2
        public string GetBuildingID()
        {
            return buildingID;
        }

        //L1R6
        public string GetCurrentState()
        {
            return currentState;
        }

        // L1R7 : 
        //public bool SetCurrentState(string state)
        //{

        //state= state.ToLower();
        //    if (state == "closed")
        //    {
        //        currentState = "closed";
        //        return true;
        //    }

        //    else if (state == "out of hours")
        //    {
        //        currentState = "out of hours";
        //        return true;
        //    }

        //    else if (currentState == "open")
        //    {
        //        currentState = "open";
        //        return true;
        //    }

        //    else if (state == "fire drill")
        //    {
        //        currentState = "fire drill";
        //        return true;
        //    }

        //    else if (state == "fire alarm")
        //    {
        //        currentState = "fire alarm";
        //        return true;
        //    }

        //    else return false;

        //}

        //L2R1 , L2R2
        string historyState;
        public bool SetCurrentState(string state)
        {
            //state = state.ToLower();
            bool result = false;

            if ((state == "closed"))
            {
                if (currentState == "closed")
                {
                    //DoorManager.LockAllDoors();
                    //LightManager.SetAllLights(false);
                    result = true;
                    return result;

                }

                if ((currentState == "out of hours"))
                {
                    //DoorManager.LockAllDoors();
                    //LightManager.SetAllLights(false);
                    currentState = "closed";
                    result = true;
                    return result;
                }

                else if ((historyState == "closed") && ((currentState == "fire alarm") || (currentState == "fire drill")))
                {
                    //DoorManager.LockAllDoors();
                    //LightManager.SetAllLights(false);
                    currentState = "closed";
                    result = true;
                    return result;
                }

                DoorManager.LockAllDoors();
                LightManager.SetAllLights(false);
                return result;

            }

            if ((state == "open"))
            {
                if (currentState == "open")
                {
                    result = true;
                    return result;
                }

                if (currentState == "out of hours")
                {
                    currentState = "open";
                    result = true;
                    return result;
                }

                if (DoorManager.OpenAllDoors() == true)
                {
                    currentState = "open";
                    result = true;
                    return result;
                }

                else if (DoorManager.OpenAllDoors() == false)
                {
                    result = false;
                    return result;


                }

                else if ((historyState == "open") && ((currentState == "fire alarm") || (currentState == "fire drill")))
                {
                    currentState = "open";
                    result = true;
                    return result;
                }

                return result;


            }

            if ((state == "out of hours"))
            {
                if (currentState == "out of hours")
                {
                    result = true;
                    return result;
                }

                if (currentState == "open")
                {
                    currentState = "out of hours";
                    result = true;
                    return result;
                }

                if (currentState == "closed")
                {
                    currentState = "out of hours";
                    result = true;
                    return result;
                }

                else if ((historyState == "out of hours") && ((currentState == "fire alarm") || (currentState == "fire drill")))
                {
                    currentState = "out of hours";
                    result = true;
                    return result;
                }

                return result;
            }

            if (state == "fire alarm")
            {
                if (currentState == "out of hours")
                {

                    historyState = "out of hours";
                    currentState = "fire alarm";

                    //FireAlarmManager.SetAlarm(true);
                    //DoorManager.OpenAllDoors();
                    //LightManager.SetAllLights(true);
                    //WebService.LogFireAlarm("fire alarm");

                    result = true;
                    return result;
                }

                if (currentState == "open")
                {
                    historyState = "open";
                    currentState = "fire alarm";

                    //FireAlarmManager.SetAlarm(true);
                    //DoorManager.OpenAllDoors();
                    //LightManager.SetAllLights(true);
                    //WebService.LogFireAlarm("fire alarm");

                    result = true;
                    return result;
                }

                if (currentState == "closed")
                {
                    historyState = "closed";
                    currentState = "fire alarm";

                    //FireAlarmManager.SetAlarm(true);
                    //DoorManager.OpenAllDoors();
                    //LightManager.SetAllLights(true);
                    //WebService.LogFireAlarm("fire alarm");

                    result = true;
                    return result;
                }

                //FireAlarmManager.SetAlarm(true);
                //DoorManager.OpenAllDoors();
                //LightManager.SetAllLights(true);
                //WebService.LogFireAlarm("fire alarm");
                return result;
            }




            if (state == "fire drill")
            {
                if (currentState == "out of hours")
                {
                    historyState = "out of hours";
                    currentState = "fire drill";
                    result = true;
                    return result;
                }

                if (currentState == "open")
                {
                    historyState = "open";
                    currentState = "fire drill";
                    result = true;
                    return result;
                }

                if (currentState == "closed")
                {
                    historyState = "closed";
                    currentState = "fire drill";
                    result = true;
                    return result;
                }
                return result;
            }

            else if ((state == "fire drill") && ((currentState == "fire drill")))
            {
                result = true;
                return result;
            }



            else if ((state == "fire alarm") && ((currentState == "fire alarm")))
            {
                result = true;
                return result;
            }



            else return result = false;


        }

        //public BuildingController(string buildingID, string currentState, string previousState) : this(buildingID, currentState)
        //{
        //    this.previousState = previousState;
        //}

        public string GetStatusReport()
        {
            string lightStatus = LightManager.GetStatus();
            string doorStatus = DoorManager.GetStatus();
            string fireAlarmStatus = FireAlarmManager.GetStatus();
            string systemStatus = lightStatus+doorStatus+fireAlarmStatus;
            return systemStatus;


        }


        

    }

}