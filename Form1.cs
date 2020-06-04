using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.ConfigurationManagement.AdminConsole.AppManFoundation;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement.ExpressionOperators;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.SystemsManagementServer.DesiredConfigurationManagement.Expressions;
using Microsoft.SystemsManagementServer.DesiredConfigurationManagement.Rules;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Add_Application
{
    public partial class Form1 : Form
    {

        //Global Variables
        string serverName;
        int userCollectionFolderID;
        string userCollectionFolderName;
        int machineCollectionFolderID;
        string machineCollectionFolderName;
        string allPrintersCollectionID;
        string allPrintersCollectionName;
        int applicationFolderID;
        string applicationFolderName;
        int objTypeApplication = 6000;
        int objTypeUserCollection = 5001;
        int objTypeDeviceCollection = 5000;
        string sccmUserName;
        string sccmPassword;
        string printerServerName;
        string printerServerUsername;
        string printerServerPassword;
        string siteCode;
        string sPrinterServer;
        string sPrinter;
        string sModel;
        bool bCancel;

        CancellationTokenSource source = new CancellationTokenSource();

        WqlConnectionManager connectionManager = new WqlConnectionManager();

        public Form1()
        {
            string jsonFile = System.Windows.Forms.Application.StartupPath + "\\app_config.json";
            var configFile = ConfigFile.FromJsonFile(jsonFile);

            sccmUserName = configFile.sccmUsername;
            sccmPassword = configFile.sccmPassword;
            serverName = configFile.ServerName;
            connectionManager = Initialize(serverName, sccmUserName, sccmPassword);

            userCollectionFolderName = configFile.userCollectionFolderName;
            IResultObject userCollectionFolder = QueryFolders(connectionManager, userCollectionFolderName, objTypeUserCollection);
            userCollectionFolderID = userCollectionFolder["ContainerNodeID"].IntegerValue;
            //userCollectionFolderID = configFile.UserCollectionFolderId;

            machineCollectionFolderName = configFile.machineCollectionFolderName;
            IResultObject machineCollectionFolder = QueryFolders(connectionManager, machineCollectionFolderName, objTypeDeviceCollection);
            machineCollectionFolderID = machineCollectionFolder["ContainerNodeID"].IntegerValue;
            //machineCollectionFolderID = configFile.MachineCollectionFolderId;

            allPrintersCollectionName = configFile.AllPrintersCollectionName;
            allPrintersCollectionID = configFile.AllPrintersCollectionId;

            applicationFolderName = configFile.applicationFolderName;
            IResultObject applicationFolder = QueryFolders(connectionManager, applicationFolderName, objTypeApplication);
            applicationFolderID = applicationFolder["ContainerNodeID"].IntegerValue;

            printerServerName = configFile.printerServerName;
            printerServerUsername = configFile.printerServerUsername;
            printerServerPassword = configFile.printerServerPassword;
            siteCode = configFile.siteCode;

            InitializeComponent();
            Cancel.Enabled = false;
            Cancel.Visible = false;
            this.CancelButton = Cancel;
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg != "")
                {
                    parameter = arg;
                }
            }
            if (parameter == "remove" || parameter == "/remove" || Remove.Checked == true)
            {
                this.Text = "Remove Printers";
                foreach (Control c in this.Controls)
                {
                    if (c.Name != "List")
                        c.Text = c.Text.Replace("Add", "Remove");
                }
                label2.Text = "Example:Printer Name or Browse";
            }
            updateListBox("--------------------------------------------------------------------CONNECTED-------------------------------------------------------------------------");
            updateListBox("");
        }

        private static AppManWrapper wrapper;
        private static ApplicationFactory factory;

        public static WqlConnectionManager Initialize(string siteServerName, string username, string password)
        {
            Validator.CheckForNull(siteServerName, "siteServerName");
            Log("Connecting to the SMS Provider on computer [{0}].", siteServerName);
            WqlConnectionManager connectionManager = new WqlConnectionManager();
            if (username != "" && password != "")
            {
                connectionManager.Connect(siteServerName, username, password);
            }
            else
            {
                connectionManager.Connect(siteServerName);
            }
            Log("Initializing the ApplicationFactory.");
            factory = new ApplicationFactory();
            wrapper = AppManWrapper.Create(connectionManager, factory) as AppManWrapper;
            return connectionManager;
        }

        public static void Store(Microsoft.ConfigurationManagement.ApplicationManagement.Application application)
        {
            Validator.CheckForNull(application, "application");
            Validator.CheckForNull(wrapper, "wrapper");
            Exception ex = null;
            try
            {
                wrapper.InnerAppManObject = application;
                Log("Initializing the SMS_Application object with the model.");
                factory.PrepareResultObject(wrapper);
                Log("Saving application, Title: [{0}], Scope: [{1}].", application.Title, application.Scope);
                wrapper.InnerResultObject.Put();
            }
            catch (SmsException exception)
            {
                ex = exception;
            }
            catch (Exception exception)
            {
                ex = exception;
            }
            if (ex != null)
            {
                Log("ERROR saving application [{0}].", ex.Message);
                Log(ex);
            }
            else
            {
                Log("Successfully saved application.");
            }
        }

        public static Microsoft.ConfigurationManagement.ApplicationManagement.Application CreateApplication(string title, string description, string Manufacturer, string language)
        {
            Validator.CheckForNull(title, "title");
            Validator.CheckForNull(language, "language");
            Validator.CheckForNull(Manufacturer, "Manufacturer");
            Log("Creating application [{0}].", title);
            Microsoft.ConfigurationManagement.ApplicationManagement.Application app = new Microsoft.ConfigurationManagement.ApplicationManagement.Application { Title = title };
            app.DisplayInfo.DefaultLanguage = language;
            app.Publisher = Manufacturer;
            System.Drawing.Image image = System.Drawing.Image.FromFile(System.Windows.Forms.Application.StartupPath + "\\printericon.png");
            Microsoft.ConfigurationManagement.ApplicationManagement.Icon icon = new Microsoft.ConfigurationManagement.ApplicationManagement.Icon(image);
            app.DisplayInfo.Add(new AppDisplayInfo { Title = title, Description = description, Language = language, Publisher = Manufacturer, Icon = icon });
            return app;
        }

        public static DeploymentType CreateScriptDt(string title, string description, string installCommandLine, string uninstallCommandline, string detectionScript, string contentFolder, Microsoft.ConfigurationManagement.ApplicationManagement.Application application, string printerServer, string printerName)
        {
            Validator.CheckForNull(installCommandLine, "installCommandLine");
            Validator.CheckForNull(uninstallCommandline, "uninstallCommandLine");
            Validator.CheckForNull(title, "title");
            Validator.CheckForNull(detectionScript, "detectionScript");
            Log("Creating Script DeploymentType.");
            ScriptInstaller installer = new ScriptInstaller();
            installer.InstallCommandLine = installCommandLine;
            installer.UninstallCommandLine = uninstallCommandline;
            installer.RequiresLogOn = true;
            installer.UserInstall = true;
            installer.MachineInstall = false;
            installer.UserInteractionMode = UserInteractionMode.Hidden;
            installer.RequiresUserInteraction = true;
            installer.MaxExecuteTime = 15;
            installer.ExecuteTime = 1;
            installer.ExecutionContext = Microsoft.ConfigurationManagement.ApplicationManagement.ExecutionContext.User;
            installer.DetectionScript = new Script { Text = detectionScript, Language = ScriptLanguage.JavaScript.ToString() };

            //Create Detection Method//
            installer.DetectionMethod = DetectionMethod.Enhanced;
            EnhancedDetectionMethod ehd = new EnhancedDetectionMethod();

            // Get value from this registry setting
            RegistrySetting registrySetting = new RegistrySetting(null);
            registrySetting.RootKey = RegistryRootKey.CurrentUser;
            registrySetting.Key = @"Software\Microsoft\Windows NT\CurrentVersion\PrinterPorts";
            registrySetting.Is64Bit = true;
            registrySetting.ValueName = "\\\\" + printerServer + "\\" + printerName;
            registrySetting.CreateMissingPath = false;
            registrySetting.SettingDataType = DataType.String;

            // Add the setting to the EHD
            ehd.Settings.Add(registrySetting);

            // The expected value of the detected registry key
            ConstantValue constValue = new ConstantValue("winspool", DataType.String);

            // Create the reference to the EHD setting
            SettingReference settingRef = new SettingReference(
                application.Scope,
                application.Name,
                application.Version.GetValueOrDefault(),
                registrySetting.LogicalName,
                registrySetting.SettingDataType,
                registrySetting.SourceType,
                false);

            settingRef.MethodType = ConfigurationItemSettingMethodType.Value;

            // Create a collection of operands, these will be compared in an Expression
            CustomCollection<ExpressionBase> operands = new CustomCollection<ExpressionBase>();
            operands.Add(settingRef);
            operands.Add(constValue);

            // Expression to verify that all of the operands meet the ExpressionOperator value            
            Expression exp = new Expression(ExpressionOperator.Contains, operands);

            // Create the rule
            Microsoft.SystemsManagementServer.DesiredConfigurationManagement.Rules.Rule rule = new Microsoft.SystemsManagementServer.DesiredConfigurationManagement.Rules.Rule("MyRuleId", NoncomplianceSeverity.None, null, exp);

            // Only add content if specified and exists.
            if (Directory.Exists(contentFolder) == true)
            {
                Content content = ContentImporter.CreateContentFromFolder(contentFolder);
                if (content != null)
                {
                    installer.Contents.Add(content);
                }
            }
            DeploymentType dt = new DeploymentType(installer, ScriptInstaller.TechnologyId, NativeHostingTechnology.TechnologyId);
            dt.Title = "Printer";
            ehd.Rule = rule;
            installer.EnhancedDetectionMethod = ehd;
            return dt;
        }

        public static void Log(Exception exception)
        {
            Log("ERROR: [{0}] ", exception.Message);
            Log("Stack: [{0}]", exception.StackTrace);
            if (exception.InnerException != null)
            {
                Log(exception.InnerException);
            }
        }

        public static void Log(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public IResultObject QueryApplications(WqlConnectionManager connection, string printerName)
        {



            try
            {
                string query = "Select * From SMS_Application Where IsLatest=" + ((char)34) + "True" + ((char)34) + " AND LocalizedDisplayName=" + ((char)34) + printerName + ((char)34);
                IResultObject listOfApplications = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject application in listOfApplications)
                {
                    return application;
                }
                return null;

            }

            catch (SmsException ex)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to list applications. Error: " + ex.Message);
                throw;
                //return null;
            }
        }
        public IResultObject QueryCollections(WqlConnectionManager connection, string printerName)
        {

            try
            {
                string query = "Select * From SMS_Collection Where Name=" + ((char)34) + printerName + ((char)34);
                IResultObject listOfCollections = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject col in listOfCollections)
                {
                    return col;
                }
                return null;

            }

            catch (SmsException ex)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to list applications. Error: " + ex.Message);
                throw;
                //return null;
            }
        }

        public IResultObject QueryFolders(WqlConnectionManager connection, string folderName, int ObjectType)
        {
            try
            {
                string query = "Select * from SMS_ObjectContainerNode where Name like '%" + folderName + "%' and ObjectType='" + ObjectType + "'";
                IResultObject listOfCollections = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject col in listOfCollections)
                {
                    return col;
                }
                return null;

            }

            catch (SmsException ex)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to find folder: " + folderName + ". Error: " + ex.Message);
                throw;
                //return null;
            }
        }
        public IResultObject CreateCollection(WqlConnectionManager connection, string printerName)
        {
            int collectionType = 1;
            string limitToCollectionID = "SMS00004";
            string limitToCollectionName = "All Users and User Groups";

            if (parameter == "device")
            {
                collectionType = 2;
                limitToCollectionID = "SMS00001";
                limitToCollectionName = "All Systems";
            }

            try
            {
                //Create Collection Schedule
                IResultObject newInterval = connectionManager.CreateEmbeddedObjectInstance("SMS_ST_RecurInterval");
                newInterval["DaySpan"].IntegerValue = 1;
                newInterval["HourSpan"].IntegerValue = 0;
                newInterval["MinuteSpan"].IntegerValue = 0;
                newInterval["StartTime"].StringValue = ("19700201000000.000000+***");
                newInterval["IsGMT"].BooleanValue = false;
                List<IResultObject> intervalList = new List<IResultObject>();
                intervalList.Add(newInterval);


                // Create a new SMS_Collection object.
                IResultObject newCollection = connectionManager.CreateInstance("SMS_Collection");

                // Populate new collection properties.
                newCollection["Name"].StringValue = printerName;
                newCollection["CollectionType"].IntegerValue = collectionType;
                newCollection["OwnedByThisSite"].BooleanValue = true;
                newCollection["LimitToCollectionID"].StringValue = limitToCollectionID;
                newCollection["LimitToCollectionName"].StringValue = limitToCollectionName;
                newCollection["RefreshType"].IntegerValue = 2;
                newCollection.SetArrayItems("RefreshSchedule", intervalList);
                newCollection.Put();
                newCollection.Get();
                Dictionary<string, object> requestRefreshParameters = new Dictionary<string, object>();
                requestRefreshParameters.Add("IncludeSubCollections", false);
                newCollection.ExecuteMethod("RequestRefresh", requestRefreshParameters);
                return newCollection;
            }

            catch (SmsException ex)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to create collection. Error: " + ex.Message);
                throw;
                //return null;
            }

        }

        public void MoveItem(WqlConnectionManager connection, string itemObjectID, int objectType, int sourceContainerID, int destinationContainerID)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string[] sourceItems = new string[1];
                // Only one item is being moved, the array size is 1.

                sourceItems[0] = itemObjectID;
                parameters.Add("InstanceKeys", sourceItems);
                parameters.Add("ContainerNodeID", sourceContainerID);
                parameters.Add("TargetContainerNodeID", destinationContainerID);
                parameters.Add("ObjectType", objectType);
                connection.ExecuteMethod("SMS_ObjectContainerItem", "MoveMembers", parameters);
            }
            catch (SmsException e)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to move folder item. Error: " + e.Message);
                throw;
            }
        }

        public WqlQueryResultsObject QueryAssignments(WqlConnectionManager connection, string printerName)
        {


            try
            {
                string query = "Select * From SMS_ApplicationAssignment Where ApplicationName=" + ((char)34) + printerName + ((char)34);
                WqlQueryResultsObject listOfAssignments = connection.QueryProcessor.ExecuteQuery(query) as WqlQueryResultsObject;
                return listOfAssignments;
            }

            catch (SmsException ex)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to list applications. Error: " + ex.Message);
                throw;
                //return null;
            }
        }

        public void CreateAssignment(WqlConnectionManager connection, string printerName, IResultObject app, string collectionName, string targetCollectionId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string[] appCIDS = new string[1];
            appCIDS[0] = app["CI_ID"].StringValue;

            IResultObject newAssignment = connection.CreateInstance("SMS_ApplicationAssignment");
            newAssignment["ApplicationName"].StringValue = printerName;
            newAssignment["ApplyToSubTargets"].BooleanValue = false;
            newAssignment["AppModelID"].IntegerValue = app["ModelID"].IntegerValue;
            newAssignment["AssignedCI_UniqueID"].StringValue = app["CI_UniqueID"].StringValue;
            newAssignment["AssignedCIs"].StringArrayValue = appCIDS;
            newAssignment["AssignmentAction"].IntegerValue = 2;
            newAssignment["AssignmentDescription"].StringValue = "";

            //.AssignmentID
            newAssignment["AssignmentName"].StringValue = app["LocalizedDisplayName"].StringValue + "_" + app["LocalizedDisplayName"] + "Install";
            newAssignment["AssignmentType"].IntegerValue = 2;

            //.AssignmentUniqueID
            newAssignment["CollectionName"].StringValue = collectionName; //col["Name"].StringValue;

            //.ContainsExpiredUpdates

            //.CreationTime
            newAssignment["DesiredConfigType"].IntegerValue = 1;
            newAssignment["DisableMomAlerts"].BooleanValue = false;
            newAssignment["DPLocality"].IntegerValue = 80;
            newAssignment["Enabled"].BooleanValue = true;

            //.EnforcementDeadline

            //.EvaluationSchedule

            //.ExpirationTime

            //.LastModificationTime

            //.LastModifiedBy
            newAssignment["LocaleID"].IntegerValue = 1033;
            newAssignment["LogComplianceToWinEvent"].BooleanValue = false;

            //.NonComplianceCriticality
            newAssignment["NotifyUser"].BooleanValue = false;
            newAssignment["OfferFlags"].IntegerValue = 0;
            newAssignment["OfferTypeID"].IntegerValue = 2;
            newAssignment["OverrideServiceWindows"].BooleanValue = false;
            newAssignment["PersistOnWriteFilterDevices"].BooleanValue = false;

            //.PolicyBinding
            newAssignment["Priority"].IntegerValue = 1;
            newAssignment["RaiseMomAlertsOnFailure"].BooleanValue = false;
            newAssignment["RebootOutsideOfServiceWindows"].BooleanValue = false;
            newAssignment["RequireApproval"].BooleanValue = false;
            newAssignment["SendDetailedNonComplianceStatus"].BooleanValue = false;
            newAssignment["SoftDeadlineEnabled"].BooleanValue = false;
            newAssignment["SourceSite"].StringValue = siteCode;
            newAssignment["StartTime"].StringValue = ("19700201000000.000000+***");
            newAssignment["StateMessagePriority"].IntegerValue = 5;
            newAssignment["SuppressReboot"].IntegerValue = 0;
            newAssignment["TargetCollectionID"].StringValue = targetCollectionId; //col["CollectionID"].StringValue;

            //.UpdateDeadline
            newAssignment["UpdateSupersedence"].BooleanValue = false;
            newAssignment["UseGMTTimes"].BooleanValue = true;
            newAssignment["UserUIExperience"].BooleanValue = true;
            newAssignment["WoLEnabled"].BooleanValue = false;

            //.PSComputerName = "SCCM01"
            newAssignment.Put();

        }

        //public void CreateAssignmentAll(WqlConnectionManager connection, string printerName, IResultObject app)
        //{
        //    Dictionary<string, object> parameters = new Dictionary<string, object>();
        //    string[] appCIDS = new string[1];
        //    appCIDS[0] = app["CI_ID"].StringValue;
        //
        //    IResultObject newAssignment = connection.CreateInstance("SMS_ApplicationAssignment");
        //    newAssignment["ApplicationName"].StringValue = printerName;
        //    newAssignment["ApplyToSubTargets"].BooleanValue = false;
        //    newAssignment["AppModelID"].IntegerValue = app["ModelID"].IntegerValue;
        //    newAssignment["AssignedCI_UniqueID"].StringValue = app["CI_UniqueID"].StringValue;
        //    newAssignment["AssignedCIs"].StringArrayValue = appCIDS;
        //    newAssignment["AssignmentAction"].IntegerValue = 2;
        //    newAssignment["AssignmentDescription"].StringValue = "";

            //.AssignmentID
        //    newAssignment["AssignmentName"].StringValue = app["LocalizedDisplayName"].StringValue + "_" + app["LocalizedDisplayName"] + "Install";
        //    newAssignment["AssignmentType"].IntegerValue = 2;

            //.AssignmentUniqueID
        //    newAssignment["CollectionName"].StringValue = allPrintersCollectionName;

            //.ContainsExpiredUpdates

            //.CreationTime
          //  newAssignment["DesiredConfigType"].IntegerValue = 1;
          //  newAssignment["DisableMomAlerts"].BooleanValue = false;
          //  newAssignment["DPLocality"].IntegerValue = 80;
          //  newAssignment["Enabled"].BooleanValue = true;

            //.EnforcementDeadline

            //.EvaluationSchedule

            //.ExpirationTime

            //.LastModificationTime

            //.LastModifiedBy
          //  newAssignment["LocaleID"].IntegerValue = 1033;
          //  newAssignment["LogComplianceToWinEvent"].BooleanValue = false;

            //.NonComplianceCriticality
          //  newAssignment["NotifyUser"].BooleanValue = false;
          //  newAssignment["OfferFlags"].IntegerValue = 0;
          //  newAssignment["OfferTypeID"].IntegerValue = 2;
          //  newAssignment["OverrideServiceWindows"].BooleanValue = false;
          //  newAssignment["PersistOnWriteFilterDevices"].BooleanValue = false;

            //.PolicyBinding
          //  newAssignment["Priority"].IntegerValue = 1;
          //  newAssignment["RaiseMomAlertsOnFailure"].BooleanValue = false;
          //  newAssignment["RebootOutsideOfServiceWindows"].BooleanValue = false;
          //  newAssignment["RequireApproval"].BooleanValue = false;
          //  newAssignment["SendDetailedNonComplianceStatus"].BooleanValue = false;
          //  newAssignment["SoftDeadlineEnabled"].BooleanValue = false;
          //  newAssignment["SourceSite"].IntegerValue = 720;
          //  newAssignment["StartTime"].StringValue = ("19700201000000.000000+***");
          //  newAssignment["StateMessagePriority"].IntegerValue = 5;
          //  newAssignment["SuppressReboot"].IntegerValue = 0;
          //  newAssignment["TargetCollectionID"].StringValue = allPrintersCollectionID;

            //.UpdateDeadline
          //  newAssignment["UpdateSupersedence"].BooleanValue = false;
          //  newAssignment["UseGMTTimes"].BooleanValue = true;
          //  newAssignment["UserUIExperience"].BooleanValue = true;
          //  newAssignment["WoLEnabled"].BooleanValue = false;

            //.PSComputerName = "SCCM01"
          //  newAssignment.Put();



        //}

        public Match findMatch(string text, string exp)
        {
            Match m = Regex.Match(text, exp);
            return m;
        }

        public void ParseText()
        {
            string rgx = @"\\\\.*\\[^/]*";
            Match match = findMatch(textBox1.Text, rgx);
            if (match.Success)
            {
                if (textBox1.Text.Contains("/model"))
                {
                    string[] model = textBox1.Text.Split('/');
                    sModel = model[1].Replace("model", "");
                    sModel = sModel.Replace(" ", "");
                    string[] words = model[0].Split('\\');
                    sPrinterServer = words[2];
                    sPrinter = words[3];
                }
                else
                {
                    string[] words = textBox1.Text.Split('\\');
                    sPrinterServer = words[2];
                    sPrinter = words[3];
                    sModel = "N/A";
                }
                ListViewItem item1 = new ListViewItem(sPrinter);
                item1.SubItems.Add(sPrinterServer);
                item1.SubItems.Add(sModel);
                listView1.Items.Add(item1);
            }
        }

        public async Task ParseCSV()
        {
            if (textBox1.Text.Contains(".csv"))
            {
                await Task.Run(() =>
                {
                    using (TextFieldParser parser = new TextFieldParser(textBox1.Text))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        string[] headerfields = parser.ReadFields();
                        while (!parser.EndOfData)
                        {
                            if (bCancel == true)
                            {
                                listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                                //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                                bCancel = false;
                                return;
                            }
                            //Process row
                            string[] fields = parser.ReadFields();
                            sPrinterServer = fields[0];
                            sPrinter = fields[1];
                            sModel = fields[2];
                            if (sModel == "")
                            {
                                sModel = "N/A";
                            }
                            listView1.Invoke((System.Action)(() =>
                            {
                                ListViewItem item1 = new ListViewItem(sPrinter);
                                item1.SubItems.Add(sPrinterServer);
                                item1.SubItems.Add(sModel);
                                listView1.Items.Add(item1);
                            }));
                        }
                    }
                });
            }
        }

        public ManagementObjectCollection QueryPrinterServer(string printerServer)
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = System.Management.ImpersonationLevel.Impersonate;

            if (printerServerUsername != null && printerServerPassword != null)
            {
                options.Username = printerServerUsername;
                options.Password = printerServerPassword;
            }

            try
            {
                ManagementScope scope = new ManagementScope("\\\\" + printerServer + "\\root\\cimv2", options);
                scope.Connect();
                //Query system for Operating System information
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Printer Where Shared='True'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                ManagementObjectCollection queryCollection = searcher.Get();
                return queryCollection;
            }
            catch (Exception e)
            {
                //listBox1.BeginInvoke(new MyDelegate(updateListBox), "Could not connect to " + printerServer + ": Error:" + e.Message);
                throw;
                //return null;
            }
        }

        public async Task ParseServer()
        {
            if (textBox1.Text != "")
            {
                ManagementObjectCollection queryCollection;
                try
                {
                   queryCollection = QueryPrinterServer(textBox1.Text);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Could not connect to " + textBox1.Text + "\n" + e.Message);
                    return;
                }
                await Task.Run(() =>
                {
                    foreach (ManagementObject m in queryCollection)
                    {
                        if (bCancel == true)
                        {
                            listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                            //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                            bCancel = false;
                            return;
                        }
                        if (m != null)
                        {
                            listView1.Invoke((System.Action)(() =>
                            {
                                ListViewItem item1 = new ListViewItem(m["ShareName"].ToString());
                                item1.SubItems.Add(m["SystemName"].ToString());
                                item1.SubItems.Add(m["DriverName"].ToString());
                                listView1.Items.Add(item1);
                            }));
                        }
                    }
                });
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Textbox Cannot Be Empty!");
            }
        }

        string parameter;
        public void AddPrinter(ListViewItem[] items)
        {
            foreach (ListViewItem item in items)
            {
                //string sPrinter = "Some Printer";
                //string sPrinterServer = "msprintsvr01";
                //string sModel = "TOSHIBA";
                if (bCancel == true)
                {
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                    //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                    bCancel = false;
                    return;
                }

                sPrinter = item.Text;
                sPrinterServer = item.SubItems[1].Text;
                sModel = item.SubItems[2].Text;

                string[] args = Environment.GetCommandLineArgs();
                foreach (string arg in args)
                {
                    if (arg != "")
                    {
                        parameter = arg;
                    }
                }
                string installCommand = "rundll32 printui.dll,PrintUIEntry /in /n \"\\\\" + sPrinterServer + "\\" + sPrinter + "\"";
                string uninstallCommand = "rundll32 printui.dll,PrintUIEntry /dn /n \"\\\\" + sPrinterServer + "\\" + sPrinter + "\"";
                IResultObject collection = QueryCollections(connectionManager, sPrinter);
                if (collection == null)
                {
                    int objectType = objTypeUserCollection; //5001
                    int collectionFolder = userCollectionFolderID;
                    if (parameter == "printserver")
                    {
                        collectionFolder = userCollectionFolderID;
                    }
                    if (parameter == "device")
                    {
                        collectionFolder = machineCollectionFolderID;
                        objectType = objTypeDeviceCollection; //5000
                    }

                    CreateCollection(connectionManager, sPrinter);
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Created Collection... ");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                    collection = QueryCollections(connectionManager, sPrinter);

                    foreach (IResultObject colItem in collection)
                    {
                        if (colItem != null)
                        {
                            MoveItem(connectionManager, colItem["CollectionID"].StringValue, objectType, 0, collectionFolder);
                        }
                    }
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Moved Collection...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                }
                else
                {
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Collection Exists...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                }

                IResultObject application = QueryApplications(connectionManager, sPrinter);
                if (application != null)
                {

                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Application Exists...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }

                    application.Get();
                    ApplicationFactory applicationFactory = new ApplicationFactory();
                    AppManWrapper applicationWrapper = AppManWrapper.WrapExisting(application, applicationFactory) as AppManWrapper;
                    Microsoft.ConfigurationManagement.ApplicationManagement.Application app = applicationWrapper.InnerAppManObject as Microsoft.ConfigurationManagement.ApplicationManagement.Application;
                    app.DeploymentTypes.Clear();
                    app.Publisher = sModel;
                    app.DeploymentTypes.Add(CreateScriptDt(app.Title, app.Description, installCommand, uninstallCommand, "return 0;", null, app, sPrinterServer, sPrinter));
                    Store(app);

                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Modified Application...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }

                }
                else
                {
                    int objectType = objTypeApplication; //6000
                                                         //if (parameter == "printserver")
                                                         //{
                                                         //    applicationFolder = applicationFolderID;
                                                         //}
                                                         //if (parameter == "device")
                                                         //{
                                                         //     applicationFolder = applicationFolderID;
                                                         // }

                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Creating Application...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }

                    Microsoft.ConfigurationManagement.ApplicationManagement.Application newApplication = CreateApplication(sPrinter, "Printer", sModel, System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                    newApplication.DeploymentTypes.Add(CreateScriptDt(newApplication.Title, newApplication.Description, installCommand, uninstallCommand, "return 0;", null, newApplication, sPrinterServer, sPrinter));
                    Store(newApplication);

                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Created Application...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }

                    application = QueryApplications(connectionManager, sPrinter);

                    foreach (IResultObject appItem in application)
                    {
                        if (appItem != null)
                        {
                            MoveItem(connectionManager, appItem["ModelName"].StringValue, objectType, 0, applicationFolderID);
                        }
                    }

                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Moved Application...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }

                }

                WqlQueryResultsObject assignment = QueryAssignments(connectionManager, sPrinter);
                int asnCount = 0;
                foreach (WqlResultObject asn in assignment)
                {
                    asnCount++;
                }
                if (asnCount == 0)
                {
                    collection = QueryCollections(connectionManager, sPrinter);
                    application = QueryApplications(connectionManager, sPrinter);

                    foreach (IResultObject appItem in application)
                    {
                        if (appItem != null)
                        {
                            CreateAssignment(connectionManager, sPrinter, application, collection["Name"].StringValue, collection["CollectionID"].StringValue);
                            if (parameter != "device" && parameter != "printserver")
                            {
                                CreateAssignment(connectionManager, sPrinter, application, allPrintersCollectionName, allPrintersCollectionID);
                            }
                        }
                    }


                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Deployed Application...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }

                }
                else
                {
                    application = QueryApplications(connectionManager, sPrinter);
                    foreach (IResultObject appItem in application)
                    {
                        if (appItem != null)
                        {
                            if (parameter != "device" | parameter != "printserver")
                            {
                                try
                                {
                                    CreateAssignment(connectionManager, sPrinter, application, allPrintersCollectionName, allPrintersCollectionID);

                                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Deployed Application To All Printers...");
                                    if (bCancel == true)
                                    {
                                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                                        bCancel = false;
                                        return;
                                    }

                                }
                                catch
                                {
                                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": All Printers Applicaton Deployment Exists...");
                                    if (bCancel == true)
                                    {
                                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                                        bCancel = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Applicaton Deployment Exists...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                }
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
            }
            //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
            listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------FINISHED--------------------------------------------------------------------------");
            //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
        }

        public void updateListBox(string input)
        {
            listBox1.BeginUpdate();
            listBox1.Items.Add(input);
            listBox1.TopIndex = listBox1.Items.Count - 1;
            listBox1.EndUpdate();

        }

        public WqlQueryResultsObject GetApplications(WqlConnectionManager connection, string printerName)
        {



            try
            {
                string query = string.Format("SELECT * FROM SMS_Application WHERE LocalizedDisplayName=\"{0}\"", printerName);
                WqlQueryResultsObject listOfApplications = connection.QueryProcessor.ExecuteQuery(query) as WqlQueryResultsObject;
                return listOfApplications;

            }

            catch (SmsException ex)
            {
                listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Created Collection... ");
                listBox1.BeginInvoke(new MyDelegate(updateListBox), "Failed to get applications. Error: " + ex.Message);
                throw;
                //return null;
            }
        }

        public delegate void MyDelegate(string myArg2);

        public void RemovePrinter(ListViewItem[] items)
        {
            foreach (ListViewItem i in items)
            {
                //string sPrinter = "Some Printer";
                //string sPrinterServer = "msprintsvr01";
                //string sModel = "TOSHIBA";
                if (bCancel == true)
                {
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                    //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                    bCancel = false;
                    return;
                }

                sPrinter = i.Text;
                sPrinterServer = i.SubItems[1].Text;
                sModel = i.SubItems[2].Text;
                //IResultObject assignment = QueryAssignments(connectionManager, printerName);
                WqlQueryResultsObject assignment = QueryAssignments(connectionManager, sPrinter) as WqlQueryResultsObject;
                if (assignment != null)
                {
                    foreach (WqlResultObject asn in assignment)
                    {
                        if (asn != null)
                        {
                            asn.Delete();
                            listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Removed Deployment...");
                            if (bCancel == true)
                            {
                                listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                                //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                                bCancel = false;
                                return;
                            }
                        }
                        else
                        {
                            listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Deployment(s) Not Found...");
                            if (bCancel == true)
                            {
                                listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                                //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                                bCancel = false;
                                return;
                            }
                        }

                    }
                }
                else
                {
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Deployment(s) Not Found...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                }
                WqlQueryResultsObject applications = GetApplications(connectionManager, sPrinter);
                List<WqlResultObject> applicationList = new List<WqlResultObject>();
                foreach (WqlResultObject application in applications)
                {
                    applicationList.Add(application);
                }
                applicationList.OrderBy(item => item["CIVersion"].LongValue).ToList();
                int count = 0;
                foreach (WqlResultObject application in applicationList)
                {
                    if (application != null)
                    {
                        ////if it is the latest version then it must be retired before deletion 
                        if (application["IsLatest"].BooleanValue == true && application["IsExpired"].BooleanValue == false)
                        {
                            //Dictionary<string, object> list = application.MethodList;
                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            parameters["Expired"] = true;
                            application.ExecuteMethod("SetIsExpired", parameters);
                            application.Get();
                        }

                        ////delete the application 
                        application.Delete();
                        ////listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        count++;
                    }
                }
                if (count > 0)
                {
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Removed Application...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                }
                else
                {
                    listBox1.BeginInvoke(new MyDelegate(updateListBox), sPrinter + ": Application Not Found...");
                    if (bCancel == true)
                    {
                        listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------CANCELED--------------------------------------------------------------------------");
                        //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
                        bCancel = false;
                        return;
                    }
                }
            }
            //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
            listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------FINISHED--------------------------------------------------------------------------");
            //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");
        }

        private async void runButton_Click(object sender, EventArgs e)
        {

            listBox1.BeginInvoke(new MyDelegate(updateListBox), "----------------------------------------------------------------------STARTING--------------------------------------------------------------------------");
            //listBox1.BeginInvoke(new MyDelegate(updateListBox), "");

            textBox1.Enabled = false;
            button2.Enabled = false;
            runButton.Enabled = false;
            Cancel.Enabled = true;
            Cancel.Visible = true;
            checkBox1.Enabled = false;
            Remove.Enabled = false;
            List.Enabled = false;

            if (listView1.Items.Count > 0)
            {
                ListViewItem[] items = new ListViewItem[listView1.Items.Count];
                listView1.Items.CopyTo(items, 0);
                CancellationToken token;
                if (parameter == "remove" || parameter == "/remove" || Remove.Checked == true)
                {
                    await Task.Run(() => RemovePrinter(items),token);
                }
                else
                {
                    await Task.Run(() => AddPrinter(items),token);
                }
            }
            Cancel.Enabled = false; Cancel.Visible = false; Cancel.Text = "Cancel";
            if (!checkBox1.Checked) { button2.Enabled = true; }
            runButton.Enabled = true; runButton.Visible = true;
            textBox1.Enabled = true;
            checkBox1.Enabled = true;
            Remove.Enabled = true;
            List.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openFileDialog1.FileName != "")
                    {
                        textBox1.Text = openFileDialog1.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message;
            string caption;
            // Initializes the variables to pass to the MessageBox.Show method.
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg != "")
                {
                    parameter = arg;
                }
            }
            if (parameter == "remove" || parameter == "/remove" || Remove.Checked == true)
            {
                message = " Example: PrinterName \n Example: PrinterServer \n \n Column 1 of CSV Should Contain The Print Server \n Column 2 of CSV Should Contain The Printer \n Column 3 of CSV Should Contain The Model (If Necessary) \n\n Uncheck the 'Remove' box to switch to add mode \n Check the 'Use Printer Server' box to pull printers from a server";
                caption = "Help!";

            }
            else
            {
                message = " Include all slashes and spaces: \n Example: \\\\PrinterServer\\Printer \n Example: \\\\PrinterServer\\Printer /model Model \n Example: PrinterServer \n \n Column 1 of CSV Should Contain The Print Server \n Column 2 of CSV Should Contain The Printer \n Column 3 of CSV Should Contain The Model (If Necessary) \n\n Check the 'Remove' box to switch to remove mode \n Check the 'Use Printer Server' box to pull printers from a server";
                caption = "Help!";
            }

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {

                // Closes the parent form.

                this.Close();

            }

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (printerServerName != null)
                {
                    this.textBox1.Text = printerServerName;
                    //this.textBox1.ReadOnly = true;
                }
                this.button2.Enabled = false;
                label2.Text = "Example: PrinterServer";
            }
            else
            {
                //this.textBox1.ReadOnly = false;
                if (Remove.Checked)
                {
                    label2.Text = "Example: PrinterName or Browse";
                }
                else
                {
                    label2.Text = "Example: \\\\PrinterServer\\Printer or Browse";
                }
                this.button2.Enabled = true;
            }
        }

        private async void Cancel_Click(object sender, EventArgs e)
        {
            Cancel.Text = "Cancelling...";
            bCancel = true;
        }

        private void Remove_CheckedChanged(object sender, EventArgs e)
        {
            if (Remove.Checked)
            {
                Form1.ActiveForm.Text = "Remove Printers";
                foreach (Control c in ActiveForm.Controls)
                {
                    if (c.Name != "List")
                        c.Text = c.Text.Replace("Add", "Remove");
                }
                if (checkBox1.Checked)
                {
                    label2.Text = "Example: PrinterServer";
                }
                else
                {
                    label2.Text = "Example: PrinterName or Browse";
                }

            }
            else
            {
                Form1.ActiveForm.Text = "Add Printers";
                foreach (Control c in ActiveForm.Controls)
                {
                    if (!(c.Name == "Remove"))
                    {
                        c.Text = c.Text.Replace("Remove", "Add");
                    }
                }
                if (checkBox1.Checked)
                {
                    label2.Text = "Example: PrinterServer";
                }
                else
                {
                    label2.Text = "Example: \\\\PrinterServer\\Printer or Browse";
                }
            }
        }

        private async void List_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            button2.Enabled = false;
            runButton.Enabled = false;
            Cancel.Enabled = true;
            Cancel.Visible = true;
            checkBox1.Enabled = false;
            Remove.Enabled = false;
            List.Enabled = false;

            if (checkBox1.Checked == true)
            {
                await ParseServer();
            }
            else
            {
                if (textBox1.Text.Contains(".csv"))
                {
                    await ParseCSV();
                }
                else
                {
                    ParseText();
                }
            }

            Cancel.Enabled = false;Cancel.Visible = false;Cancel.Text = "Cancel";
            if (!checkBox1.Checked) { button2.Enabled = true;}
            runButton.Enabled = true; runButton.Visible = true;
            textBox1.Enabled = true;
            checkBox1.Enabled = true;
            Remove.Enabled = true;
            List.Enabled = true;

        }

        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void ListBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(Cursor.Position);
            }
        }

        private void RemoveItem_Click(object sender, EventArgs e)
        {
            var items = listView1.SelectedItems;
            foreach (ListViewItem item in items)
            {
                listView1.Items.Remove(item);
            }
        }

        private void RemoveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void ClearToolStripMenu_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
