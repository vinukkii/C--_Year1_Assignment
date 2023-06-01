using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NuGet.Frameworks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SmartBuilding;
using System.Diagnostics.SymbolStore;
using System.Globalization;

namespace SmartBuildingTests
{
    [TestFixture]
    public class BuildingControllerTests
    {
        ILightManager LightManager = Substitute.For<ILightManager>();
        IFireAlarmManager FireAlarmManager = Substitute.For<IFireAlarmManager>();
        IDoorManager DoorManager = Substitute.For<IDoorManager>();
        IWebService WebService = Substitute.For<IWebService>();
        IEmailService EmailService = Substitute.For<IEmailService>();

        ILightManager LightManagerFault = Substitute.For<ILightManager>();
        IFireAlarmManager FireAlarmManagerFault = Substitute.For<IFireAlarmManager>();
        IDoorManager DoorManagerFault = Substitute.For<IDoorManager>();
        IWebService WebServiceFault = Substitute.For<IWebService>();
        IEmailService EmailServiceFault = Substitute.For<IEmailService>();



        [SetUp]
        public void setup()
        {
            // for the correctly working systems : 
            LightManager.GetStatus().Returns("Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            FireAlarmManager.GetStatus().Returns("FireAlarm,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            DoorManager.GetStatus().Returns("Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,");

            //for the incorrect get status :

            LightManagerFault.GetStatus().Returns("Lights,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,");
            FireAlarmManagerFault.GetStatus().Returns("FireAlarm,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,");
            DoorManagerFault.GetStatus().Returns("Doors,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,");

            //for L3R4 and L3R5
            //Here we are saying that the method is true 
            DoorManager.OpenAllDoors().Returns(true);
            DoorManager.LockAllDoors().Returns(true);

            //Here we are saying that the method is false 
            DoorManagerFault.OpenAllDoors().Returns(false);
            DoorManagerFault.LockAllDoors().Returns(false);


            // for L4R1
            LightManager.SetAllLights(false);

            //for L4R2


        }

        //L1R1, L1R2:
        [TestCase("building1", "building1")]


        public void CheckBuildingIDConstructorTest1(string buildingID, string output)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID );
            //Act
            var ID = buildingController1.GetBuildingID();
            //Asset
            Assert.AreEqual(buildingID, output);

        }

        [TestCase("building2", "result")]

        public void CheckBuildingIDConstructorTest2(string buildingID, string output)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
            //Act
            var ID = buildingController1.GetBuildingID();
            //Asset
            Assert.AreNotEqual(buildingID, output);

        }

        //L1R3:
        [TestCase("BUILDING3", "building3")]

        public void CheckcaseSensitive(string buildingID, string output)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
            //Act
            var ID = buildingController1.GetBuildingID();

            //Assert
            Assert.AreEqual(output, ID);
        }

        //L1R4:
        [Test]

        public void checkSetIDfunction()
        {
            //Arrange 
            var buildingController1 = new BuildingController("building1");

            //Act
            buildingController1.SetBuildingID("BUILDING2");
            var ID = buildingController1.GetBuildingID();

            //Assert
            Assert.AreEqual("building2", ID);
        }

        //L1R5, L1R6
        [TestCase("building1", "out of hours")]

        public void currentStateDefualt_outofhours(string buildingID, string result)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
            //Act
            string currentState = buildingController1.GetCurrentState();
            //Assert 
            Assert.AreEqual(result, currentState);
        }

        //L1R5, L1R6
        [TestCase("building1", "out of hours")]
        [TestCase("building2", " out of hours ")]
        [TestCase("building3", "     out of hours")]
        [TestCase("building4", "out of hours      ")]

        public void currentStateDefualtOutofhoursSpacesensitive(string buildingID, string result)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
            //Act
            string currentState = buildingController1.GetCurrentState();
            //Assert 
            Assert.AreEqual(result.Trim(), currentState.Trim());
        }

        //L1R5 ,L1R6, L1R7
        [TestCase("building1", "out of hours")]

        public void checkValidstateOutofHoursBool(string buildingID, string result)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
            //Act

            string getCurrentState = buildingController1.GetCurrentState();
            //Assert
            //Assert.IsTrue(output);
            Assert.AreEqual(getCurrentState, result);


        }


        //L1R7


        [TestCase("building1", "closed", "closed")]
        [TestCase("building2", "out of hours", "out of hours")]
        [TestCase("building3", "fire drill", "fire drill")]
        [TestCase("building4", "fire alarm", "fire alarm")]
        // [TestCase("building5", "open", "open")]

        public void checkallValidStateAllBool(string buildingID, string state, string standardState)
        {
            //Arrange 
            var buildingController1 = new BuildingController(buildingID);
            //Act
            bool output = buildingController1.SetCurrentState(state);
            //Assert
            Assert.IsTrue(output);
            Assert.AreEqual(standardState, buildingController1.GetCurrentState());

            //Assert.AreEqual(state, standardState);
        }


        //L1R7
        [TestCase("building", "open", true)]
        [TestCase("building", "closed", true)]
        [TestCase("building", "in use", false)] 
        [TestCase("building", "Dismiss", false)]
        [TestCase("building", "out of order ", false)]

        public void checkstateInvalid_(string buildingID, string state, bool result)
        {
            //Arrange 
            var buildingController1 = new BuildingController(buildingID );
           
            //Act
            bool output = buildingController1.SetCurrentState(state);
            
            //Assert 

            Assert.AreEqual(output, result);

        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        //L2R1
        // Here this test case is written to ensure the normal states of the given diagram 
        [TestCase("building1", "closed", "out of hours" , true)]
        [TestCase("building1", "out of hours", "open", true)]
        [TestCase("building1", "open", "out of hours", true)]
        [TestCase("building1", "out of hours", "out of hours", true)]


        public void currentStateChangesAsTranstionDiagram1(string buildingID, string currentState, string otherState , bool result)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
            buildingController1.SetCurrentState(currentState);

            //Act

            bool checkState = buildingController1.SetCurrentState(otherState);

            //Assert
            Assert.AreEqual(result, checkState);

        }

        //Here the states outer from the normal super state is tested. as mentioned in the PDF 
        // if initial state is open and goes to fire alarm it should come back to open 
        // open -> fire alarm -> open 

        [TestCase ("building1" , "open" , "fire alarm" , "open" , true)]
        [TestCase ("building1" , "open" , "fire drill" , "open" , true)]
        [TestCase("building1", "close", "fire alarm", "close", true)]
        [TestCase("building1", "close", "fire drill", "close", true)]


        public void stateChangeAheadof_normalOperation_historyState (string buildingID , string historyState , string currentState , string TobeState , bool result )
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID);
           
            //Act
            buildingController1.SetCurrentState(historyState);
            buildingController1.SetCurrentState(currentState);
            bool checkState = buildingController1 .SetCurrentState(TobeState);

            //Assert
            Assert.AreEqual (result, checkState);
        }


        //L2R2 : 


        [TestCase("building1" , "open" , "open", true)]
        [TestCase("building1", "close", "close", true)]
        [TestCase("building1", "out of hours", "out of hours", true)]

        public void ReturnsTrueifanAttemptisMadetoChangeThestateTotheSameState(string buildingID , string currentState , string newState , bool result)
        { 
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            buildingController1.SetCurrentState(currentState);
            bool checkState = buildingController1.SetCurrentState(newState);

            //Assert
            Assert.AreEqual(checkState, result);
        }


        //L2R3:

        [TestCase("building1", "OPEN", "open")]
        [TestCase("building2", "clOSed", "closed")]
        [TestCase("building3", "out of HOURS", "out of hours")]


        public void stateInitialization_accrodingtotheNormal_operationState(string buildingID, string state, string result)
        {
            //Arrange
            var buidlingController1 = new BuildingController(buildingID, state);


            //Act
            string setState = buidlingController1.GetCurrentState();

            //Assert
            Assert.AreEqual(setState, result);
        }


        //L2R3
        [TestCase("building1", "fire alarm")]


        public void checkArgumentException_ifstate_notEqualtoNormalStates_Passed(string buildingID, string state)
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentException>(() => new BuildingController(buildingID, state));

        }



        //L3R1
        // checking if the setCurrent state works in the 3rd constructor 
        [TestCase("building1", "open", true)]
        [TestCase("building2", "closed", true)]
        [TestCase("building3", "out of hours", true)]
        [TestCase("building4", "fire alarm", true)]
        [TestCase("building5", "fire drill", true)]

        public void setState_whenValidStates_arePassed(string buildingID, string state, bool result)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act

            bool settoState = buildingController1.SetCurrentState(state);
            //Assert
            Assert.AreEqual(result, settoState);
        }

        [TestCase("building1" , "out of hours" )]

        public void checkiftheCurrentStateisSet_toOutofHours ( string buildingID, string setStateInConstructor )
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            string GetSetState = buildingController1.GetCurrentState();

            //Assert
            Assert.AreEqual(GetSetState, setStateInConstructor);
        }

        //L3R2 : interfaces - getStatus()

        //CorrectGetStatus 
        //interface1 - IlightManager

        [TestCase ("building1","Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]

        public void checkiftheGetStatusof_LightreturnsString ( string buildingID,string outputResult ) 
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            string output = LightManager.GetStatus();

            //Assert
            Assert.AreEqual(outputResult, output);
        }

        //interface2 - IfireAlarmManager

        [TestCase("building1", "FireAlarm,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]

        public void checkiftheGetStatusof_FireAlarmreturnsString(string buildingID, string outputResult)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            string output = FireAlarmManager.GetStatus();

            //Assert
            Assert.AreEqual(outputResult, output);
        }

        //interface3 - IfireAlarmManager

        [TestCase("building1", "Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]

        public void checkiftheGetStatusof_DoorreturnsString(string buildingID, string outputResult)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            string output = DoorManager.GetStatus();

            //Assert
            Assert.AreEqual(outputResult, output);
        }

        //Faulty GetStatus

        //interface1 - IlightManager

        [TestCase("building1", "Lights,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,")]

        public void checkiftheGetStatusof_LightFaulty_returnsString(string buildingID, string outputResult)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManagerFault, FireAlarmManagerFault, DoorManagerFault, WebServiceFault, EmailServiceFault);

            //Act
            string output = LightManagerFault.GetStatus();

            //Assert
            Assert.AreEqual(outputResult, output);
        }

        //interface2 - IfireAlarmManager

        [TestCase("building1", "FireAlarm,OK,OK,FAULT,OK,OK,OK,OK,FAULT,OK,OK,")]

        public void checkiftheGetStatusof_FireAlarmFaulty_returnsString(string buildingID, string outputResult)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManagerFault, FireAlarmManagerFault, DoorManagerFault, WebServiceFault, EmailServiceFault);

            //Act
            string output = FireAlarmManagerFault.GetStatus();

            //Assert
            Assert.AreEqual(outputResult, output);
        }

        //interface3 - IfireAlarmManager

        [TestCase("building1", "Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]

        public void checkiftheGetStatusof_DoorFaulty_returnsString(string buildingID, string outputResult)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManagerFault, FireAlarmManagerFault, DoorManagerFault, WebServiceFault, EmailServiceFault);

            //Act
            string output = DoorManager.GetStatus();

            //Assert
            Assert.AreEqual(outputResult, output);
        }


        //L3R3
        //Testing the GetStatusReport() from the constructor 

        [TestCase ("building1","Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,FireAlarm,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]

        public void checkGetStatusReport(string buildingID, string outputResult) 
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);
            //Act

            var output = buildingController1.GetStatusReport();
            //Assert
            Assert.AreEqual(outputResult, output);
        }

        //L3R5 - returns true state
        [TestCase ("building1" , "open" )]

        public void OpenAlldoors_WorksProperly_OpenState(string buildingID , string state )
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            bool result = buildingController1.SetCurrentState(state);
            bool doorState = DoorManager.OpenAllDoors();

            //Assert
            Assert.AreEqual(result, doorState);

        }

        //L3R4 - returns false state

        [TestCase("building1", "open")]

        public void OpenAlldoors_DoesntWorkAccordinly_OpenState(string buildingID, string state)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManagerFault, FireAlarmManagerFault, DoorManagerFault, WebServiceFault, EmailServiceFault);

            //Act
            bool result = buildingController1.SetCurrentState(state);
            bool doorState = DoorManagerFault.OpenAllDoors();

            //Assert
            Assert.AreEqual(!result, doorState);

        }

        //L4R1
        // for the checking of doors 
        [TestCase("building1" , "closed")]

        public void lockAllDoors_worksAccordingly (string buildingID, string state) 
        {
            //Arrange

            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            bool result = buildingController1.SetCurrentState(state);
            bool doorState = DoorManager.LockAllDoors();

            //Assert
            Assert.AreEqual (result, doorState);


        }

        // for the checking of lights 

        [TestCase("building1", "closed")]

        public void SetallLights_worksAccordingly_toswitchallLights_closed(string buildingID, string state)
        {
            //Arrange

            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            bool result = buildingController1.SetCurrentState(state);
            bool lightState = LightManager.SetAllLights();

            //Assert
            Assert.AreNotEqual(result, lightState); // here the result becomes true as the state is sucessfully set and thus we use not equal to 


        }

        //L4R2

        [TestCase("building1", "fire alarm")]

        public void OpenAlldoors_WorksProperly_FireAlarmState(string buildingID, string state)
        {
            //Arrange
            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            bool result = buildingController1.SetCurrentState(state);
            bool doorState = DoorManager.OpenAllDoors();

            //Assert
            Assert.AreEqual(result, doorState);

        }


        [TestCase("building1", "closed")]

        public void SetallLights_worksAccordingly_toswitchallLights_FIreAlarmState(string buildingID, string state)
        {
            //Arrange

            var buildingController1 = new BuildingController(buildingID, LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            //Act
            bool result = buildingController1.SetCurrentState(state);
            bool lightState = LightManager.SetAllLights();

            //Assert
            Assert.AreEqual(result, !lightState); // here the lightstate becomes false as the value is given as false when intialising it thus i use not equal to


        }
    }


}