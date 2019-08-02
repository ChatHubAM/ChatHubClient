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
using System.Data.SqlClient;
using Newtonsoft.Json;
using Clients;
using System.Net;
using System.Net.Sockets;

namespace ChatHubWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            string username = SignInUsernameBox.Text;
            string password = SignInPasswordBox.Password;

            if(username == null || password == null)
            {
                MessageBox.Show("Please enter username and password");
            }

            LogInClient login = new LogInClient(username, password);
            var token = await login.Authorize();

            string startupPath = Environment.CurrentDirectory;
            var accessTokenDeserialised = JsonConvert.DeserializeObject<AuthServerResponse>(token);

            if (accessTokenDeserialised.access_token == null)
            {
                MessageBox.Show("Please enter the correct username or passsword!");
            }
            else
            {
                System.IO.File.WriteAllText(startupPath + "\\token.txt", token);

                ChatWindow chatWindow = new ChatWindow(SignInUsernameBox.CurrentText);
                chatWindow.Show();
                this.Close();
            }
            LoginButton.IsEnabled = true;
        }

        private async void  RegistrationButtonClick(object sender, RoutedEventArgs e)
        {
            RegistrationButton.IsEnabled = false;
            User newUser = new User();
            newUser.Email = SignUpEmailBox.CurrentText;
            newUser.FullName = SignUpNameBox.CurrentText;
            newUser.Password = SignUpPasswordBox.Password;
            newUser.Username = SignUpUsernameBox.CurrentText;
            newUser.IPLocal = GetLocalIPAddress(); 
            RegistrationClient registration = new RegistrationClient();
            var isRegistered = await registration.RegisterUser(newUser);
            if(isRegistered)
            {
                MessageBox.Show("You are registered");
            }
            else
            {
                MessageBox.Show("Please enter a different username and email");
            }
            RegistrationButton.IsEnabled = true;
        }

        private void SignInUsernameBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (SignInUsernameBox.Text == "Username") SignInUsernameBox.Text = "";
        }
        private void SignInUsernameBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SignInUsernameBox.Text == "Username") SignInUsernameBox.Text = "";
        }
        private void SignInPasswordBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (SignInPasswordBox.Password == "password") SignInPasswordBox.Password = "";
        }
        
        private void SignInPasswordBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SignInPasswordBox.Password == "password") SignInPasswordBox.Password = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignUpNameBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SignUpNameBox.Text == "Name") SignUpNameBox.Text = "";
        }

        private void SignUpNameBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (SignUpNameBox.Text == "Name") SignUpNameBox.Text = "";
        }
        /// <summary>
        /// /////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignUpEmailBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SignUpEmailBox.Text == "Email") SignUpEmailBox.Text = "";
        }

        private void SignUpEmailBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (SignUpEmailBox.Text == "Email") SignUpEmailBox.Text = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignUpUsernameBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (SignUpUsernameBox.Text == "Username") SignUpUsernameBox.Text = "";
        }

        private void SignUpUsernameBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SignUpUsernameBox.Text == "Username") SignUpUsernameBox.Text = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignUpPasswordBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (SignUpPasswordBox.Text == "password") SignUpPasswordBox.Text = "";
        }

        private void SignUpPasswordBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SignUpPasswordBox.Password == "password") SignUpPasswordBox.Password = "";
        }

        private void SignInUsernameBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SignInUsernameBox.Text == "") SignInUsernameBox.Text = "Username";
        }

        private void SignInPasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SignInPasswordBox.Password == "") SignInPasswordBox.Password = "password";
        }


        private void SignUpEmailBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SignUpEmailBox.Text == "") SignUpEmailBox.Text = "Email";
        }

        private void SignUpUsernameBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SignUpUsernameBox.Text == "") SignUpUsernameBox.Text = "Username";
        }

        private void SignUpPasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SignUpPasswordBox.Password == "") SignUpPasswordBox.Password = "password";
        }

        private void SignUpNameBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SignUpNameBox.Text == "") SignUpNameBox.Text = "Name";
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}