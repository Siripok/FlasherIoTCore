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
        char k;
        List<FlahInfo> spisflash = new List<FlahInfo>();
        public MainWindow()
        {

            InitializeComponent();
            ShowPorts();
            // MessageBox.Show(Properties.Settings.Default.spifflash);

            spisflash = JsonConvert.DeserializeObject<List<FlahInfo>>(Properties.Settings.Default.spifflash);
            
            lb.ItemsSource = spisflash;

            user = Environment.UserName;

            if (Properties.Settings.Default.noClosing == true)
            {
                chk_noClose.IsChecked = true;
                k = 'k';
            }
            else
            {
                chk_noClose.IsChecked = false;
                k = 'c';
            }

            if (Properties.Settings.Default.espPath != "" && Properties.Settings.Default.espPath != null)
            {
                txt_espressif.Text = Properties.Settings.Default.espPath;
                idfPath = GetIdfPath();
            }

            else
                MessageBox.Show("Укажите путь до esp папки\nX:\\yourPath\\ESP\\ на конце должен быть \\ - бэкслеш", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public string GetIdfPath() //?!
        {

            return @"C:\esp\";
            //Предотвращает ошибку: Вызывающий поток не может получить доступ к этому объекту, поскольку он принадлежит другому потоку.
            
            //return txt_espressif.Text.Trim().Trim('\\') + "\\"; //System.InvalidOperationException:
            //"Вызывающий поток не может получить доступ к данному объекту,
            //так как владельцем этого объекта является другой поток."
            //если прошиваем несколько!

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

        private void myProcess_Exited(object sender, System.EventArgs e, String port, bool multik=false, int num=0 )
        {
           
            string MACpath = @$"C:\Users\{user}\AppData\Local\Temp\espMACi{port}.txt";

            if (num != 0) 
                w = num;//если прошиваем несколько сразу
            //иначе оставляем w если прошиваем по одной w=w;


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
                                                             // 
                    spisflash[w].flMAC = MACiCOM[0];
                }

                if (line.Contains("Serial port "))
                {
                    MACiCOM[1] = line; //COM
                    MACiCOM[1] = MACiCOM[1].Remove(0, 12).Trim();//COM     
                }

                this.Dispatcher.Invoke(() => //Предотвращает ошибку: Вызывающий поток не может получить доступ к этому объекту, поскольку он принадлежит другому потоку.
                {
                    lb.Items.Refresh();

                    //if(spisflash[w].flMAC!="") //коннектимся если есть МАС адрес
                    //CreateEsDevice("giulia-novars-smart-realtime", "europe-west1", "atest-registry", MACiCOM[0], "ec_public.pem");
                });
            }
                        

            //MessageBox.Show($"{MACiCOM[0]}\n{MACiCOM[1]}\n{spisflash[num].status}");
                   

            if (multik)
            {
                if (spisflash.Count - 1 >= num + 1)
                {
                    flashtool(spisflash[num + 1].Path, spisflash[num + 1].Port, true, num + 1);
                }
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

        public void erase_flash(string PORT) // очистка платы от предыдoщей прошивки
        {
            try
            {
                
                var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /{k} \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"{GetIdfPath()}components\\esptool_py\\esptool\\esptool.py\" --chip esp32 --port {PORT} erase_flash\"\"");
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

        public void flashtool(string binPath, string PORT, bool allflash=false, int num=0) //прошивка платы bin файлом
        {
            if (spisflash[num].chk == true || allflash==false)
            {
                try
                {
                    var startInfo = new ProcessStartInfo(@"C:\Windows\system32\cmd.exe", $" /{k} \"\"C:\\Users\\{user}\\.espressif\\idf_cmd_init.bat\" &\"python\" \"{GetIdfPath()}components\\esptool_py\\esptool\\esptool.py\" --chip ESP32 -p {PORT} -b 921600 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 \"{binPath}\" --erase-all >C:\\Users\\{user}\\AppData\\Local\\Temp\\espMACi{PORT}.txt\"\"");
                    //в bat файле
                    //esptool.py --chip esp32 -p COM3 -b 115200 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 garage.bin --erase-all
                    //call C:\Windows\system32\cmd.exe /k ""C:\Users\Siripok\.espressif\idf_cmd_init.bat" &"python" "C:\esp\components\esptool_py\esptool\esptool.py" --chip ESP32 -p COM3 -b 921600 --after hard_reset write_flash --flash_size 4MB --flash_mode dio 0x00000 yourbin.bin --erase-all""
                    startInfo.WorkingDirectory = GetIdfPath();
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Exited += new EventHandler((sender, e) => myProcess_Exited(sender, e, PORT, allflash, num));
                    process.Start();
                   
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message + $"\n{GetIdfPath()}", "Что-то сломалось!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //   5      3
                if (spisflash.Count - 1 >=num+1)
                { 
                flashtool(spisflash[num+1].Path, spisflash[num + 1].Port, true, num + 1);
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
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ShowPorts();
            if (spisflash != null)
            {
                for (int i = 0; i < spisflash.Count; i++)
                {
                    spisflash[i].portNames = portNames;
                    spisflash[i].Port = ""; //чтобы порты оставались, даже если устр-во было отключено, нужно закоментировать
                }
            }
            
            lb.Items.Refresh();
        }


        private void flash_all_Click(object sender, RoutedEventArgs e)
        {

            flashtool(spisflash[0].Path, spisflash[0].Port, true, 0);


            /*
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
            }*/
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

            Properties.Settings.Default.spifflash = json;
            Properties.Settings.Default.noClosing = (bool) chk_noClose.IsChecked;
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

     
        private void chk_noClose_Click(object sender, RoutedEventArgs e)
        {
            if (chk_noClose.IsChecked == true) 
            {
               
                k = 'k'; 
            }
            //no closing
            else
            {
                k = 'c'; //cloing
            }


        }

        private void project_Click(object sender, RoutedEventArgs e)
        {
            Window1 nf = new Window1();
            nf.ShowDialog();
        }
    }

}
//Created 02/08/2021 -- ...
//v 1.0