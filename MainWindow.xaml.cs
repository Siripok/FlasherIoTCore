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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //<Button Content="id" Margin="5,3,3,3" VerticalAlignment="Top" Width="22"  Click="cmdId_Clicked" Background="#b9e6ae"/>
    //MessageBox.Show("sender " + sender.ToString() + "\ne(sourse) " + e.RoutedEvent.ToString()); отладка
    public class FlahInfo
    {
        public string Path { get; set; }
        public string Port { get; set; }

        public string[] portNames { get; set; }

        public bool chk { get; set; }
        public int elem { get; set; }


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
        //string idfPath = ""; //на конце пути должен быть слэш "\"
        List<FlahInfo> spisflash = new List<FlahInfo>();
        public MainWindow()
        {

            InitializeComponent();
            ShowPorts();
            // MessageBox.Show(Properties.Settings.Default.spifflash);

            spisflash = JsonConvert.DeserializeObject<List<FlahInfo>>(Properties.Settings.Default.spifflash);
            /*
                        spisflash.Add(new FlahInfo("Nekiy Put", "COM5") { portNames= portNames });
                        spisflash.Add(new FlahInfo("", "") { portNames = portNames });
                        spisflash.Add(new FlahInfo("", "") { portNames = portNames });
                        spisflash.Add(new FlahInfo("666", "COM144") { portNames = portNames });
                        spisflash.Add(new FlahInfo("666", "") { portNames = portNames });
                        //MessageBox.Show(spisflash[0].Path);
            */
            lb.ItemsSource = spisflash;

            user = Environment.UserName;

            if (Properties.Settings.Default.espPath != "" && Properties.Settings.Default.espPath != null)
            {
                txt_espressif.Text = Properties.Settings.Default.espPath;

            }

            else
                MessageBox.Show("Укажите путь до esp папки X:\\yourPath\\ESP\\ на конце должен быть \\ - бэкслеш", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public string GetIdfPath()
        {
            return txt_espressif.Text.Trim().Trim('\\') + "\\";
        }
        public void ShowPorts()
        {
            if (portNames != null)
            {
                Array.Clear(portNames, 0, portNames.Length);//очистка массива, если он не пустой 
                //SelectIndexMinus1();                             

            }

            portNames = SerialPort.GetPortNames();//получить список всех доступнх портов

            if (portNames.Length > 0)
            {
                //lb.ItemsSource = portNames;

            }
            else MessageBox.Show("Необнаружено ни одного устройства.", "Подключите устройства!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        string[] MACiCOM = new string[2];

        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            //MessageBox.Show("fghh");
            string MACpath = @$"C:\Users\{user}\AppData\Local\Temp\espMACiCOM.txt";

            foreach (string line in File.ReadLines(MACpath))
            {
                if (line.Contains("MAC: ")) MACiCOM[0] = line; //MAC
                if (line.Contains("Serial port ")) MACiCOM[1] = line; //COM

            }


            MACiCOM[0] = MACiCOM[0].Remove(0, 5);//MAC 
            MACiCOM[0] = MACiCOM[0].Replace(":", ""); // удаляем : из МАС адреса

            MACiCOM[1] = MACiCOM[1].Remove(0, 12).Trim();//COM     



            //CreateEsDevice("giulia-novars-smart-realtime", "europe-west1", "atest-registry", MACiCOM[0], "ec_public.pem");

            MessageBox.Show($"{MACiCOM[0]}\n{MACiCOM[1]}");//{ MACiCOM[1]}

            //Array.Clear(MACiCOM, 0, MACiCOM.Length);


        }
        public void ChipId(string PORT)
        {
            try
            {

                var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /c \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"{GetIdfPath()}components\\esptool_py\\esptool\\esptool.py\" --p {PORT} chip_id >C:\\Users\\{user}\\AppData\\Local\\Temp\\espMACiCOM.txt\"\"");//chip_id |clip

                //в bat файле
                //esptool.py --chip esp32 --port COM3 erase_flash
                //call C:\Windows\system32\cmd.exe /k ""C:\Users\Siripok\.espressif\idf_cmd_init.bat" &"python" "C:\esp\components\esptool_py\esptool\esptool.py" --port COM3 chip_id |clip""
                startInfo.WorkingDirectory = GetIdfPath();
                Process process = new Process();
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(myProcess_Exited);
                process.Start();


            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message + $"\n{GetIdfPath()}", "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmdId_Clicked(object sender, RoutedEventArgs e)
        {

            Button cmd = (Button)sender;
            if (cmd.DataContext is FlahInfo)
            {

                FlahInfo msg = (FlahInfo)cmd.DataContext;

                //MessageBox.Show(msg.Port);
                if (msg.Port != "")
                    ChipId(msg.Port);
                else MessageBox.Show($"Не выбран COM порт!", "?", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
        private void cmdErase_Clicked(object sender, RoutedEventArgs e)
        {

            Button cmd = (Button)sender;
            if (cmd.DataContext is FlahInfo)
            {

                FlahInfo msg = (FlahInfo)cmd.DataContext;
                //MessageBox.Show(msg.Port);
                if (msg.Port != "")
                    erase_flash(msg.Port);
                else MessageBox.Show($"Не выбран COM порт!", "?", MessageBoxButton.OK, MessageBoxImage.Warning);


            }

        }


        private void cmdFlash_Clicked(object sender, RoutedEventArgs e)
        {

            Button cmd = (Button)sender;
            if (cmd.DataContext is FlahInfo)
            {

                FlahInfo msg = (FlahInfo)cmd.DataContext;

                if (msg.Port != "")
                {
                    if (msg.Path != "" && msg.Path != "Ничего не выбрано!")
                    {

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

        public void erase_flash(string PORT) // очистка платы от предыдузей прошивки
        {
            try
            {
                var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /k \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"{GetIdfPath()}components\\esptool_py\\esptool\\esptool.py\" --chip esp32 --port {PORT} erase_flash\"\"");
                //в bat файле
                //esptool.py --chip esp32 --port COM3 erase_flash
                //call C:\Windows\system32\cmd.exe /k ""C:\Users\Siripok\.espressif\idf_cmd_init.bat" &"python" "C:\esp\components\esptool_py\esptool\esptool.py" --chip esp32 --port COM3 erase_flash""
                startInfo.WorkingDirectory = GetIdfPath();
                Process.Start(startInfo);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message + $"\n{GetIdfPath()}", "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void flashtool(string binPath, string PORT)//прошивка платы bin файлом
        {
            try
            {
                var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /c \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"{GetIdfPath()}components\\esptool_py\\esptool\\esptool.py\" --chip ESP32 -p {PORT} -b 921600 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 {binPath} --erase-all >C:\\Users\\{user}\\AppData\\Local\\Temp\\espMACiCOM.txt\"\"");
                //в bat файле
                //esptool.py --chip esp32 -p COM3 -b 115200 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 garage.bin --erase-all
                //call C:\Windows\system32\cmd.exe /k ""C:\Users\Siripok\.espressif\idf_cmd_init.bat" &"python" "C:\esp\components\esptool_py\esptool\esptool.py" --chip ESP32 -p COM3 -b 921600 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 yourbin.bin --erase-all""
                startInfo.WorkingDirectory = GetIdfPath();
                Process process = new Process();
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(myProcess_Exited);
                process.Start();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message + $"\n{GetIdfPath()}", "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show (cb1.SelectedItem as String);
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Будут очищены пути к bin файлам, а так же выбранные COM-порты.\nПродолжить?", "Очистить все поля?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                spisflash = null;
                spisflash = new List<FlahInfo>();
                // spisflash.Clear();

                ShowPorts();
                for(int i=0;i<5;i++)
                {                    
                    spisflash.Add(new FlahInfo("", "") { portNames = portNames });
                    spisflash[i].elem = i + 1;
                }
               // spisflash.Add(new FlahInfo("", "") { portNames = portNames });
              //  spisflash.Add(new FlahInfo("", "") { portNames = portNames });
                //spisflash.Add(new FlahInfo("", "") { portNames = portNames });
               // spisflash.Add(new FlahInfo("", "") { portNames = portNames });
                lb.ItemsSource = spisflash;
                lb.Items.Refresh();
                
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ShowPorts();
            if (spisflash != null)
            {

                for (int i = 0; i < spisflash.Count; i++)
                {
                    spisflash[i].portNames = portNames;
                    spisflash[i].Port = "";
                    
                }

            }
            
            lb.Items.Refresh();
        }

        /*private void checkBox_Clicked(object sender, RoutedEventArgs e)
        {

            CheckBox cmd = (CheckBox)sender;
            if (cmd.IsChecked == true) //если выбран прошиваем, иначе нет
            {
                if (cmd.DataContext is FlahInfo) {

                    FlahInfo msg = (FlahInfo)cmd.DataContext;

                    if (msg.Port != "")
                    {
                        if (msg.Path != "" && msg.Path != "Ничего не выбрано!")
                        {
                            //flashtool(msg.Path, msg.Port);
                        }
                        else cmdOpen_Clicked(sender, e);
                    }
                    else MessageBox.Show($"Не выбран COM порт", "?", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }

        }*/

        private void flash_all_Click(object sender, RoutedEventArgs e)
        {
            int u = 0;
            for (int i = 0; i < spisflash.Count; i++) 
            {
                if (spisflash[i].chk == true) u++;
            }

            if (u == 0) //если ничего не выбрано
                MessageBox.Show("Для записи необходимо выбрать хотя бы один элемент.\nВыберите нужные элементы поставив галочку в чекбоксе.", "Отметьте нужне элемент!", MessageBoxButton.OK, MessageBoxImage.Information);

            else
            {
                if (MessageBox.Show($"Вы уверены что хотите прошить несколько плат сразу?", "Прошить?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool b = true;
                    for (int i = 0; i < spisflash.Count && b == true; i++)
                    {
                        if (spisflash[i].chk == true)
                        {
                            if (spisflash[i].Path != "" && spisflash[i].Path != "Ничего не выбрано!" && spisflash[i].Port != "")
                                flashtool(spisflash[i].Path, spisflash[i].Port);
                            else if (MessageBox.Show($"Незаполнены некоторе поля у {spisflash[i].elem} элемента!\nНажмите <ОК> чтобы пропустить этот элемент.\nНажмите <Отмена> чтобы выйти.", $"Ошибка! Элемент №{spisflash[i].elem}", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                                b = false;
                        }

                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(spisflash);

            Properties.Settings.Default.spifflash = json;
            Properties.Settings.Default.espPath = GetIdfPath();
            Properties.Settings.Default.Save();

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
              MessageBox.Show( deviceId + " - " + (e.Error.Code == 409 ? "Уже существует" : e.Message) + Environment.NewLine);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }


    }

  
}
//Created 02/08/2021 -- ...