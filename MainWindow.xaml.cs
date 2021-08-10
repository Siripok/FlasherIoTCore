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
using System.IO.Ports;//
using System.Diagnostics;// Process.Start, File.Exists
using Newtonsoft.Json;
using System.IO;
using Google.Apis.CloudIot.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.CloudIot.v1.Data;

using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
 

    public class FlahInfo
    {
        public string Path { get; set; }
        public string Port { get; set; }

        public string[] portNames { get; set; }

        public bool chk { get; set; } //chekbox
        public int elem { get; set; } //№ элемента в списке
        public string flMAC { get; set; } //flashMAC
        public string status { get; set; } 
       


        public FlahInfo(string Path, string Port)
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
        public string[] projects;
        char k;
        List<FlahInfo> spisflash = new List<FlahInfo>();


        Properties.Settings Parameters = Properties.Settings.Default;


        public MainWindow()
        {

            InitializeComponent();
            ShowPorts();
            GetProjects();


            spisflash = JsonConvert.DeserializeObject<List<FlahInfo>>(Parameters.spifflash);

            lb.ItemsSource = spisflash;

            user = Environment.UserName;

            if (Parameters.select != -1)
                cbProjectNames.SelectedIndex = Parameters.select;

            if (Parameters.pref != null)
                txt_pref.Text = Parameters.pref;

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

            if (Parameters.espPath != "" && Parameters.espPath != null)
            {
                txt_espressif.Text = Parameters.espPath;

                if (Directory.Exists(txt_espressif.Text))
                    idfPath = GetIdfPath();
                else MessageBox.Show("Не удалось найти ESP-папку! Попробуйте другой путь!", ":(", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            else
                MessageBox.Show("Укажите путь до esp папки\nX:\\yourPath\\ESP\\ на конце должен быть \\ - бэкслеш", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void GetProjects()
        {
            if (Parameters.prj != "" && Parameters.prj != null)
            {
                if (projects != null) Array.Clear(projects, 0, projects.Length);

                string g = Parameters.prj;
                projects = g.Split("\r" + "\n", StringSplitOptions.RemoveEmptyEntries);

                cbProjectNames.ItemsSource = projects;

            }
            cbProjectNames.Items.Refresh();
        }

        public string GetIdfPath()
        {

            //return @"C:\esp\";
            return txt_espressif.Text.Trim().Trim('\\') + "\\";

        }

        public void ShowPorts()
        {
            if (portNames != null)
            {
                Array.Clear(portNames, 0, portNames.Length);//очистка массива, если он не пустой 
            }

            portNames = SerialPort.GetPortNames();//получить список всех доступнх портов

            if (portNames.Length == 0)
                MessageBox.Show("Необнаружено ни одного устройства.", "Подключите устройства!", MessageBoxButton.OK, MessageBoxImage.Information);

            //lb.ItemsSource = portNames;

        }

        string[] MACiCOM = new string[2];

        private void myProcess_Exited(object sender, System.EventArgs e, String port, bool multik = false, int num = 0)
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
                //   if (MACiCOM[0] != "") //коннектимся если есть МАС адрес
                //     CreateEsDevice(cbProjectNames.SelectedItem.ToString(), "europe-west1", "atest-registry", MACiCOM[0], "ec_public.pem");

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

        public string OpenFile() // выбор bin файла
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

        }

        public void erase_flash(string PORT) // очистка платы от предыдoщей прошивки
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

        }



        public void flashtool(string binPath, string PORT, bool allflash = false, int num = 0) //прошивка платы bin файлом
        {
            if (spisflash[num].chk == true || allflash == false)
            {
                try
                {
                    var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /{k} \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"\"{idfPath}components\\esptool_py\\esptool\\esptool.py\"\" --chip ESP32 --p {PORT} -b 921600 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 \"{binPath}\" --erase-all >C:\\Users\\{user}\\AppData\\Local\\Temp\\espMACi{PORT}.txt\"\"");
                    startInfo.WorkingDirectory = idfPath;
                    Process process = new Process();

                    if (chk_noClose.IsChecked == false)
                    {
                        startInfo.CreateNoWindow = true;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Exited += new EventHandler((sender, e) => myProcess_Exited(sender, e, PORT, allflash, num));

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
                spisflash = new List<FlahInfo>();
                // spisflash.Clear();

                ShowPorts();
                for (int i = 0; i < BoardsMAX; i++)
                {
                    spisflash.Add(new FlahInfo("", "") { portNames = portNames });
                    spisflash[i].elem = i + 1;
                }

                lb.ItemsSource = spisflash;
                lb.Items.Refresh();

            }

            DeleteMACiCOM();
        }

        public void DeleteMACiCOM()
        {
            for (int i = 0; i < 20; i++)
            {
                if (File.Exists($@"C:\Users\{user}\AppData\Local\Temp\espMACiCOM{i}.txt"))
                    File.Delete($@"C:\Users\{user}\AppData\Local\Temp\espMACiCOM{i}.txt");
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ShowPorts();
            GetProjects();
            if (spisflash != null)
            {
                for (int i = 0; i < spisflash.Count; i++)
                {
                    spisflash[i].portNames = portNames;
                    spisflash[i].Port = ""; //чтобы порты оставались, даже если устр-во было отключено, нужно закоментировать
                }
            }

            if (projects != null)
            {
                cbProjectNames.ItemsSource = projects;
            }


            lb.Items.Refresh();
        }


        private void flash_all_Click(object sender, RoutedEventArgs e)
        {
            string str = "";
            bool error = false;
            for (int i = 0; i < spisflash.Count; i++) //проверки
            {
                if (spisflash[i].Port == "" && spisflash[i].chk == true)
                {
                    str += $"[{spisflash[i].elem}] элемент - заполните COM порт!\n";
                    error = true;
                }

                if ((spisflash[i].Path == "" || spisflash[i].Path == "Ничего не выбрано!") && spisflash[i].chk == true)
                {
                    str += $"[{spisflash[i].elem}] элемент - заполните bin-путь!\n";
                    error = true;
                }
            }

            if (error == true) MessageBox.Show("Список проблем:\n\n" + str, "Ошибки при заполнении!", MessageBoxButton.OK, MessageBoxImage.Warning);

            if (error == false)
                flashtool(spisflash[0].Path, spisflash[0].Port, true, 0);

        }

        private void cmdErase_Clicked(object sender, RoutedEventArgs e)
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
        }

        private void cmdLog_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button cmd = (Button)sender;
                if (cmd.DataContext is FlahInfo)
                {
                    FlahInfo msg = (FlahInfo)cmd.DataContext;

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
            w = 0;
            Button cmd = (Button)sender;
            if (cmd.DataContext is FlahInfo)
            {
                FlahInfo msg = (FlahInfo)cmd.DataContext;

                if (msg.Port != "")
                {
                    if (msg.Path != "" && msg.Path != "Ничего не выбрано!")
                    {
                        w = msg.elem - 1;
                        flashtool(msg.Path, msg.Port);
                    }

                    else cmdOpen_Clicked(sender, e);
                }
                else MessageBox.Show($"Не выбран COM порт", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


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

        private void Window_Closed(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(spisflash);

            Parameters.spifflash = json;
            Parameters.noClosing = (bool)chk_noClose.IsChecked;
            Parameters.espPath = GetIdfPath();
            Parameters.select = cbProjectNames.SelectedIndex;
            Parameters.pref = txt_pref.Text;
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
            GetProjects();
        }


        DispatcherTimer timer;
        private void ChekLog(string PORT)
        {
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
    }

}
//Created 02/08/2021 -- ...
//v 1.0