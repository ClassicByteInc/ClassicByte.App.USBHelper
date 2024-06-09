using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ClassicByte.App.USBHelper
{
    internal static class Program
    {

        internal static String TargetPath { get; set; }

        internal static String OutPutPath { get; set; }

        public static String Name => "ClassicByte.App.USBHelper";

        public static XmlDocument Config { get; }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            if (args.Length != 0)
            {
                if (args[0].ToLower()=="init")
                {
                    RegExecution();
                }
            }
            if (File.Exists($"{Environment.GetEnvironmentVariable("CLASSICBYTEWORKSPACE")}\\AppData\\{Name}\\__"))
            {
                File.WriteAllLines($"{Environment.GetEnvironmentVariable("CLASSICBYTEWORKSPACE")}\\AppData\\{Name}\\__",new string[] { "D:\\", $"{Environment.GetEnvironmentVariable("CLASSICBYTEWORKSPACE")}\\AppData\\{Name}\\__" });
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Runner runner = new Runner(OnArrive);
            StartListener(runner);
            Application.Run(new MainForm());
            Process.GetCurrentProcess().Exited += Program_Exited;
        }

        private static void Program_Exited(object sender, EventArgs e)
        {
            new Task(() =>
            {
            }).Start();
        }

        private static void UsbWatcher_DeviceArrival(object sender, UsbWatcher.UsbEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        /// <summary>
        /// 当事件发生时执行的委托
        /// </summary>
        internal delegate void Runner(EventArrivedEventArgs e);


        /// <summary>
        /// 查询所有设备的插拔事件
        /// </summary>
        /// <param name="runner"></param>
        internal static void StartListener(Runner runner)
        {
            try
            {
                //查询所有设备的插拔事件
                #region 第一种查询方法
                //Win32_DeviceChangeEvent  Win32_VolumeChangeEvent
                ManagementEventWatcher watcher = new ManagementEventWatcher();
                WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent  WHERE EventType = 2 or EventType = 3");
                watcher.EventArrived += (s, e) =>
                {
                    runner(e);

                };
                watcher.Query = query;
                watcher.Start();
                #endregion

                #region 第二种查询方法
                //// Full query string specified to the constructor
                //WqlEventQuery q = new WqlEventQuery("SELECT * FROM Win32_ComputerShutdownEvent");

                //// Only relevant event class name specified to the constructor
                //// Results in the same query as above.
                //WqlEventQuery query = new WqlEventQuery("Win32_ComputerShutdownEvent");

                //Console.WriteLine(query.QueryString);

                //ConnectionOptions connectionOptions = new ConnectionOptions();
                //connectionOptions.EnablePrivileges = true;//启用用户特权

                //ManagementScope managementScope = new ManagementScope("root\\CIMV2", connectionOptions);

                //WqlEventQuery wqlEventQuery = new WqlEventQuery();
                //wqlEventQuery.EventClassName = "Win32_DeviceChangeEvent";
                //wqlEventQuery.Condition = "EventType = 2 or EventType = 3";
                //wqlEventQuery.WithinInterval = TimeSpan.FromSeconds(1);

                //ManagementEventWatcher watcher = new ManagementEventWatcher(managementScope, wqlEventQuery);
                ////watcher.EventArrived += Watcher_EventArrived;
                //watcher.EventArrived += (sender, e) =>
                //{
                //    var txt = "";
                //    foreach (var p in e.NewEvent.Properties)
                //    {
                //        txt = "name " + p.Name + " val " + p.Value + "\r\n";
                //        Console.WriteLine(txt);
                //        DeviceManage.Instance.FindDevice();
                //    }
                //};
                //watcher.Start();

                #endregion

                //ServicesManager.Instance.StartServices();
                //Thread.CurrentThread.IsBackground = false;
                //Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void OnArrive(EventArrivedEventArgs e)
        {
            #region 识别U盘


            #endregion
        }

        static void RegExecution()
        {
            System.IO.File.Copy(Process.GetCurrentProcess().MainModule.FileName,$"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\usb.exe");

        }

    }
}
