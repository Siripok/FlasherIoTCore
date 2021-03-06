using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;//SerialPort
using System.Diagnostics;// Process.Start, File.Exists

using Newtonsoft.Json;
using System.IO;
using Google.Apis.CloudIot.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.CloudIot.v1.Data;
using System.Windows.Threading;
using System.Reflection;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public class FlashInfo
    {
        public string Path { get; set; }
        public string Port { get; set; }

        public string[] portNames { get; set; }
        public List<string> DirectoryProject { get; set; }



        public bool chk { get; set; } //chekbox
        public int elem { get; set; } //№ элемента в списке
        public string flMAC { get; set; } //flashMAC
        public string status { get; set; }







        public FlashInfo(string Path, string Port)
        {
            this.Path = Path;
            this.Port = Port;
        }
    }

    public partial class MainWindow : Window
    {
        string[] portNames; // порты СОМ
        string user; //пользователь
        string idfPath = @""; //на конце пути должен быть слэш "\"
        const int BoardsMAX = 5;// кол-во строк т.е подключаемых плат
        public string[] projects;//project from 2win
        public List<string> DirectoryProject;

        char k;
        List<FlashInfo> spisflash = new List<FlashInfo>();


        Properties.Settings Parameters = Properties.Settings.Default;

        public MainWindow()
        {
            InitializeComponent();
            ShowPorts();
            GetDirectoryProject();

            spisflash = JsonConvert.DeserializeObject<List<FlashInfo>>(Parameters.spifflash);

            lb.ItemsSource = spisflash;

            user = Environment.UserName;




            if (Parameters.erase == true)
                chk_erase.IsChecked = true;



            if (Parameters.noClosing == true)
            {
                chk_noClose.IsChecked = true;
                k = 'k';
            }
            else
            {
                chk_noClose.IsChecked = false;
                k = 'c';
            }

        }

        public void GetDirectoryProject()
        {
            log.Add("Получим директорию проектов");
            DirectoryProject = new List<string>();
            // MessageBox.Show(Directory.GetCurrentDirectory() + "\\projects");
            if (!Directory.Exists("projects"))
                Directory.CreateDirectory("projects");

            foreach (string asas in Directory.GetDirectories("projects"))
            {
                DirectoryProject.Add(asas.Replace("projects\\", ""));
            }
            //irectoryProject = Directory.GetDirectories("projects").;

            lb.Items.Refresh();
        }




        public void ShowPorts()
        {
            log.Add("Обновим порты");
            if (portNames != null)
            {
                Array.Clear(portNames, 0, portNames.Length);//очистка массива, если он не пустой 
            }

            portNames = SerialPort.GetPortNames();//получить список всех доступнх COM портов

            if (portNames.Length == 0)
                MessageBox.Show("Необнаружено ни одного устройства.", "Подключите устройства!", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        string[] MACiCOM = new string[2];

        private void myProcess_Exited(object sender, System.EventArgs e, string binPath, string port, bool multik = false, int num = 0)
        {

            string MACpath = @$"C:\Users\{user}\AppData\Local\Temp\espMACi{port}.txt";

            if (num != 0)
                w = num;//если прошиваем несколько сразу
            //иначе оставляем w, если прошиваем по одной w=w;


            foreach (string line in File.ReadLines(MACpath))
            {
                if (line.Contains("A fatal error occurred:"))
                {
                    if (line.Contains("Serial port COM"))
                    {
                        spisflash[w].status = "Не удалось подключится к плате :(";
                    }

                }

                if (line.Contains("MAC: "))
                {
                    MACiCOM[0] = line; //MAC
                    MACiCOM[0] = MACiCOM[0].Remove(0, 5);//MAC 
                    MACiCOM[0] = MACiCOM[0].Replace(":", "");// удаляем : из МАС адреса

                    spisflash[w].flMAC = MACiCOM[0];
                }

                if (line.Contains("Serial port "))
                {
                    MACiCOM[1] = line; //COM
                    MACiCOM[1] = MACiCOM[1].Remove(0, 12).Trim();//COM     
                }

            }


            // MessageBox.Show(MACiCOM[0]);
            this.Dispatcher.Invoke(() => //Предотвращает ошибку: Вызывающий поток не может получить доступ к этому объекту, поскольку он принадлежит другому потоку.
            {
                timerWorking = false;
                lb.Items.Refresh();
                if (MACiCOM[0] != "") //коннектимся если есть МАС адрес
                    CreateEsDevice(getProject(binPath), "europe-west1", "atest-registry", getPrefix(binPath) + MACiCOM[0], "ec_public.pem");

            });

            //MessageBox.Show($"{MACiCOM[0]}\n{MACiCOM[1]}\n{spisflash[num].status}");


            if (multik)
            {
                if (spisflash.Count - 1 >= num + 1)
                {
                    flashtool(spisflash[num + 1].Path, spisflash[num + 1].Port, true, num + 1);
                }
            }
        }
        bool timerWorking = false;

        /*public string OpenFile() // выбор bin файла
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "";
            dlg.DefaultExt = ".bin";
            dlg.Filter = "Binary files |*.bin";
            Nullable<bool> result = dlg.ShowDialog();//? 
            if (result == true)
            {
                string filename = dlg.FileName;
                return filename;
            }

            return "Ничего не выбрано!";

        }*/

        /*public void erase_flash(string PORT) // очистка платы от предыдoщей прошивки
        {
            try
            {

                var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /{k} \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"\"{idfPath}components\\esptool_py\\esptool\\esptool.py\"\" --chip esp32 --port {PORT} erase_flash\"\"");
                startInfo.WorkingDirectory = idfPath;
                Process.Start(startInfo);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message + $"\n{idfPath}", "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }*/



        public void flashtool(string binPath, string PORT, bool allflash = false, int num = 0) //прошивка платы bin файлом
        {

            if (spisflash[num].chk == true || allflash == false)
            {
                try //921600 //115200
                    //--flash_mode dio
                {
                    string files = Directory.GetCurrentDirectory() + "\\projects\\" + binPath + "\\" + binPath;
                    //MessageBox.Show($"bin {binPath}\nDirectory.GetCurrentDirectory(){Directory.GetCurrentDirectory()}");

                    // MessageBox.Show($"files {files}\n"+$"{binPath}");
                    //var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /{k} \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"\"{idfPath}components\\esptool_py\\esptool\\" +
                    //   $"esptool.py\"\" --chip ESP32 -p {PORT} -b 921600 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 \"{binPath}\" --erase-all " +
                    //   $">C:\\Users\\{user}\\AppData\\Local\\Temp\\espMACi{PORT}.txt\"\"");

                    string erase = "";
                    if (chk_erase.IsChecked == true)
                        erase = "--erase-all "; //erase_flash
                



                    Console.WriteLine(files);
                    var startInfo = new ProcessStartInfo($@"C:\Windows\system32\cmd.exe",
                       @$"/{k} C:\Users\{user}\AppData\Local\Arduino15\packages\esp32\tools\esptool_py\2.6.1/esptool.exe --chip esp32 --port {PORT} --baud 921600 --before default_reset --after hard_reset write_flash -z --flash_mode dio {erase}--flash_freq 80m --flash_size detect 0xe000 C:\Users\{user}\AppData\Local\Arduino15\packages\esp32\hardware\esp32\1.0.4-rc1/tools/partitions/boot_app0.bin 0x1000 C:\Users\{user}\AppData\Local\Arduino15\packages\esp32\hardware\esp32\1.0.4-rc1/tools/sdk/bin/bootloader_dio_80m.bin 0x10000 {files}.ino.bin 0x8000 {files}.ino.partitions.bin" +
                      $">C:\\Users\\{user}\\AppData\\Local\\Temp\\espMACi{PORT}.txt\"\"");



                    startInfo.WorkingDirectory = idfPath;
                    Process process = new Process();

                    if (chk_noClose.IsChecked == false)
                    {
                        startInfo.CreateNoWindow = true;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Exited += new EventHandler((sender, e) => myProcess_Exited(sender, e, binPath, PORT, allflash, num));

                    process.Start();
                    ChekLog(PORT);


                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message + $"\n{idfPath}", "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //   5      3
                if (spisflash.Count - 1 >= num + 1)
                {
                    flashtool(spisflash[num + 1].Path, spisflash[num + 1].Port, true, num + 1);
                }
            }

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Будут очищены пути к bin файлам, а так же выбранные COM-порты.\nПродолжить?", "Очистить все поля?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                spisflash = null;
                spisflash = new List<FlashInfo>();
                // spisflash.Clear();

                ShowPorts();
                for (int i = 0; i < BoardsMAX; i++)
                {
                    spisflash.Add(new FlashInfo("", "") { portNames = portNames, DirectoryProject = DirectoryProject });
                    spisflash[i].elem = i + 1;
                }

                lb.ItemsSource = spisflash;
                lb.Items.Refresh();
                DeleteMACiCOM();
            }

        }

        public void DeleteMACiCOM()
        {
            for (int i = 0; i < 20; i++)
            {
                //   if (File.Exists($@"C:\Users\{user}\AppData\Local\Temp\espMACiCOM{i}.txt"))
                //     File.Delete($@"C:\Users\{user}\AppData\Local\Temp\espMACiCOM{i}.txt");
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ShowPorts();
            CloseMonitor();
            GetDirectoryProject();
            if (spisflash != null)
            {
                for (int i = 0; i < spisflash.Count; i++)
                {
                    spisflash[i].portNames = portNames;
                    spisflash[i].Port = ""; //чтобы порты оставались, даже если устр-во было отключено, нужно закоментировать эту строку

                    spisflash[i].DirectoryProject = DirectoryProject;
                    //spisflash[i].Path = "";
                }
            }


            lb.Items.Refresh();
        }


        private void flash_all_Click(object sender, RoutedEventArgs e)
        {
            CloseMonitor();
            string message = "";
            bool error = false;
            for (int i = 0; i < spisflash.Count; i++) //проверки
            {
                if (spisflash[i].Port == "" && spisflash[i].chk == true)
                {
                    message += $"[{spisflash[i].elem}] элемент - заполните COM порт!\n";
                    error = true;
                }

                if ((spisflash[i].Path == "" || spisflash[i].Path == "Ничего не выбрано!") && spisflash[i].chk == true)
                {
                    message += $"[{spisflash[i].elem}] элемент - заполните bin-путь!\n";
                    error = true;
                }
            }

            if (error == true) MessageBox.Show("Список проблем:\n\n" + message, "Ошибки при заполнении!", MessageBoxButton.OK, MessageBoxImage.Warning);

            if (error == false)
                flashtool(spisflash[0].Path, spisflash[0].Port, true, 0);

        }

        /*private void cmdErase_Clicked(object sender, RoutedEventArgs e)
        {

            Button cmd = (Button)sender;
            if (cmd.DataContext is FlahInfo)
            {
                FlahInfo msg = (FlahInfo)cmd.DataContext;

                if (msg.Port != "")
                    erase_flash(msg.Port);
                else
                    MessageBox.Show($"Не выбран COM порт!", "?", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }*/

        private void cmdLog_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button cmd = (Button)sender;
                if (cmd.DataContext is FlashInfo)
                {
                    FlashInfo msg = (FlashInfo)cmd.DataContext;

                    string open = @$"C:\Users\{user}\AppData\Local\Temp\espMACi{msg.Port}.txt";
                    if (File.Exists(open))
                    {
                        var startInfo = new ProcessStartInfo("explorer.exe", $"\"{open}\"");
                        Process process = new Process();
                        process.StartInfo = startInfo;
                        if (msg.Port != "") process.Start();
                        else MessageBox.Show("Выбериет COM порт!");
                    }
                    else
                        MessageBox.Show("Не удалось найти лог!", "Log file not found!", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        int w;//узнаёт какой элемент был выбран чтобы присвоить ему мак адрес
        private void cmdFlash_Clicked(object sender, RoutedEventArgs e)
        {
            CloseMonitor();
            w = 0;
            Button cmd = (Button)sender;
            if (cmd.DataContext is FlashInfo)
            {
                FlashInfo msg = (FlashInfo)cmd.DataContext;

                if (msg.Port != "")
                {
                    if (msg.Path != "")
                    {

                        w = msg.elem - 1;
                        flashtool(msg.Path, msg.Port);
                    }

                }
                else MessageBox.Show($"Не выбран COM порт", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /*
        private void cmdOpen_Clicked(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is FlahInfo)
            {
                FlahInfo msg = (FlahInfo)cmd.DataContext;
                msg.Path = OpenFile();
                lb.Items.Refresh();
            }
        }
        */

        private void Window_Closed(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(spisflash);

            Parameters.spifflash = json;
            Parameters.erase = (bool)chk_erase.IsChecked;
            Parameters.noClosing = (bool)chk_noClose.IsChecked;


            Parameters.Save();

            //  MessageBox.Show(jsonString);
        }

        public static CloudIotService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud IoT Core Service scope
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudIotService.Scope.CloudPlatform // Used for IoT + PubSub + IAM
                    //CloudIotService.Scope.Cloudiot // Can be used if not accessing Pub/Sub
                });
            }
            return new CloudIotService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
        }

        public static object CreateEsDevice(string projectId, string cloudRegion, string registryId, string deviceId, string keyPath)
        {
            //MessageBox.Show(projectId);
            //MessageBox.Show(cloudRegion);
            //MessageBox.Show(registryId);
            //MessageBox.Show(deviceId);

            var cloudIot = CreateAuthorizedClient();
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";

            try
            {
                String keyText = File.ReadAllText(keyPath);
                Device body = new Device()
                {
                    Id = deviceId
                };
                body.Credentials = new List<DeviceCredential>();
                body.Credentials.Add(new DeviceCredential()
                {
                    PublicKey = new PublicKeyCredential()
                    {
                        Key = keyText,
                        Format = "ES256_PEM"
                    },
                });

                var device = cloudIot.Projects.Locations.Registries.Devices.Create(body, parent).Execute();
                Console.WriteLine("Device created: ");
                Console.WriteLine($"{device.Id}");
                Console.WriteLine($"\tBlocked: {device.Blocked == true}");
                Console.WriteLine($"\tConfig version: {device.Config.Version}");
                Console.WriteLine($"\tName: {device.Name}");
                Console.WriteLine($"\tState:{device.State}");

                MessageBox.Show(deviceId + " зарегистрирован");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(deviceId + " - " + (e.Error.Code == 409 ? "Уже существует" : e.Message) + Environment.NewLine);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }


        private void chk_noClose_Click(object sender, RoutedEventArgs e)
        {
            if (chk_noClose.IsChecked == true)
            {

                k = 'k';  //no closing console
            }

            else
            {
                k = 'c'; //closing console
            }
        }

        private void project_Click(object sender, RoutedEventArgs e)
        {
            Window1 nw = new Window1();
            nw.ShowDialog();

        }


        DispatcherTimer timer;

        private void ChekLog(string PORT)
        {
            CloseMonitor();
            txt_log.Clear();

            string open = @$"C:\Users\{user}\AppData\Local\Temp\espMACi{PORT}.txt";
            if (File.Exists(open))
                File.Delete(open);

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler((sender, e) => dispatcherTimer_Tick(sender, e, PORT));
            timer.Interval = new TimeSpan(0, 0, 1);
            timerWorking = true;
            timer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e, string PORT)
        {

            string s = ""; //

            string open = @$"C:\Users\{user}\AppData\Local\Temp\espMACi{PORT}.txt";


            if (File.Exists(open))
            {
                FileStream f = new FileStream(open, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//65536, FileOptions.Asynchronous

                StreamReader sr = new StreamReader(f);

                s = sr.ReadToEnd();

                if (!String.IsNullOrWhiteSpace(s))
                {
                    txt_log.Text = s;
                    txt_log.CaretIndex = s.Length;
                    txt_log.ScrollToEnd();
                }

                sr.Close();
                f.Close();

            }
            //Task.Delay(TimeSpan.FromSeconds(1));
            if (timerWorking == false)
                timer.Stop();
        }


        private void chk_all_Click(object sender, RoutedEventArgs e)
        {
            bool b = true;
            // if (chk_all.IsChecked == false) b = false;

            //Работа с элементами
            byte nplus = 0;
            byte nminus = 0;
            for (byte i = 0; i < spisflash.Count; i++)
            {
                if (spisflash[i].chk == true) nplus++;
                else nminus++;
            }

            if (nplus > nminus)
            {
                b = false;
                chk_all.IsChecked = false;
            }
            else
            {
                if (chk_all.IsChecked == true)
                {
                    b = true;
                    chk_all.IsChecked = true;
                }
                else
                {
                    b = false;
                    chk_all.IsChecked = false;
                }
            }



            for (int i = 0; i < spisflash.Count; i++)
            {
                spisflash[i].chk = b;
            }
            lb.Items.Refresh();
        }

        string getPrefix(string nameDir)
        {
            string otvet = "";
            switch (nameDir)
            {
                case "Fan":
                    otvet = "GV_";
                    break;
                case "SmartKitchen":
                    otvet = "GN_";
                    break;

            }
            return otvet;
        }
        string getProject(string nameDir)
        {
            string otvet = "";
            switch (nameDir)
            {
                case "Fan":
                    otvet = "greenvent-632e2";
                    break;
                case "SmartKitchen":
                    otvet = "giulia-novars-smart-realtime";
                    break;
            }
            return otvet;
        }




        SerialPort sp;
        private void Monitor(string PORT, int baud)
        {
            CloseMonitor();
            sp = new SerialPort();
            sp.PortName = PORT;
            sp.BaudRate = baud; //921600  //115200
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.DataBits = 8;
            sp.Handshake = Handshake.None;
            //sp.RtsEnable = true;
            sp.ReadTimeout = 500;
            sp.WriteTimeout = 500;
            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            sp.Open();
            txt_log.Clear();
            txt_log.Text += $"{PORT} {baud} ... \r\n";
            txt_log.Text += "\r\n";
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) //static
        {
            try
            {
                SerialPort mySerialPort = (SerialPort)sender;
                string indata = mySerialPort.ReadExisting();


                Console.WriteLine($"Read: {mySerialPort}");
                //Debug.WriteLine(indata);
                this.Dispatcher.Invoke(() =>
                {
                    txt_log.Text += indata;
                    txt_log.CaretIndex = txt_log.Text.Length;
                    txt_log.ScrollToEnd();

                });
            }
            catch (TimeoutException err)
            {
                MessageBox.Show(err.ToString(), "TimeoutException?");
            }
        }

        private void CloseMonitor()
        {
            if (sp != null)
            {
                sp.Close();
            }

        }

        private void cmdMonitor_Clicked(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is FlashInfo)
            {
                FlashInfo msg = (FlashInfo)cmd.DataContext;

                if (msg.Port != "")
                {

                    Monitor(msg.Port, 115200);

                }
                else MessageBox.Show($"Не выбран COM порт", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void cmd_Settings_Clicked(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is FlashInfo)
            {
                FlashInfo msg = (FlashInfo)cmd.DataContext;

                if (msg.flMAC !=null)
                {
                    Settings st = new Settings(msg.elem - 1);
                    st.Title = $"Настройка {msg.elem} элемента: " + msg.flMAC;                   
                    st.Show();
                  
                }
                else MessageBox.Show($"Без MAC адреса нельзя произвести настройку устройства!", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static class log
        {
            public static void Add(string str = "")
            {

                string localpath = (new FileInfo(Assembly.GetExecutingAssembly().Location).Directory).ToString();
                string path = localpath + @"\TreeCadN.log";

                if (File.Exists(path))
                {
                    string save_log = Environment.NewLine + DateTime.Now + " - " + str;
                    File.AppendAllText(path, save_log, System.Text.Encoding.UTF8);
                }
            }

        }

    }
}
//Created 02/08/2021 -- ...
//v 1.0