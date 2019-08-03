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
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using System.Data.SqlClient;
using System.Threading;
using ChatHubWPF.Clients;
using Newtonsoft.Json;
using System.IO;
using Clients;
using Telerik.WinControls.Data;
using ChatHubWPF.Models;

namespace ChatHubWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public string Username { get; }
        List<ActiveUserInfo> Contacts { get; set; } = new List<ActiveUserInfo>(); 
        public string AccessToken { get; set; }

        public ChatWindow(string UserName)
        { 
            string currentPath = Environment.CurrentDirectory + "\\token.txt";
            string accesstoken = File.ReadAllText(currentPath);
            var accessTokenDeserialised = JsonConvert.DeserializeObject<AuthServerResponse>(accesstoken);
            AccessToken = accessTokenDeserialised.access_token;

            Username = UserName;
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameUser.Content = Username;

            ActiveUserClient activeUsers = new ActiveUserClient();
            var activeUserInfo = await activeUsers.ActiveUsers(AccessToken);

            if (activeUserInfo != null)
            {
                foreach (var user in activeUserInfo)
                {
                    Contacts.Add(user);

                    ContactsList.Items.Add(user.Username);
                }
            }

            var dueTime = TimeSpan.FromSeconds(1);
            var interval = TimeSpan.FromSeconds(1);
            await CallAPIPeriodicAsync(CallingAPIs, dueTime, interval, CancellationToken.None);
        }

        private async void CallingAPIs()
        {
            MessageClient messageClient = new MessageClient(AccessToken);

            MessageToFrom messageNotif = new MessageToFrom
            {
                To = Username
            };

            for (int i = 0; i < Contacts.Count(); i++)
            {
                if (Contacts[i] != null)
                {
                    messageNotif.From = Contacts[i].Username;
                    var unreadMessages = await messageClient.UnreadMessages(messageNotif);
                    if (unreadMessages != null && unreadMessages.Count() != 0)
                    {
                        ContactsList.Items[i] = Contacts[i].Username + " (" + unreadMessages.Count() + ")";
                    }
                    if(unreadMessages.Count() == 0)
                    {
                        ContactsList.Items[i] = Contacts[i].Username;
                    }
                }
            }
        }

        private void MessageTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if(MessageTextbox.Text == "Type your message here...")
            MessageTextbox.Text = "";
        }

        private void MessageTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if(MessageTextbox.Text == "")
            MessageTextbox.Text = "Type your message here...";
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
        private void ButtonMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void MessageTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if ((MessageTextbox.Text == "") || (MessageTextbox.Text == "Type your message here...") || e.Key != Key.Enter)
                return;

            MessageClient messageClient = new MessageClient(AccessToken);
            Message newMessage = new Message
            {
                MessageText = MessageTextbox.Text,
                From = Username
            };

            foreach (var contact in Contacts)
            {
                if (ContactsList.SelectedItem.ToString().Contains(contact.Username))
                {
                    newMessage.To = contact.Username;
                }
            }

            var isSent = await messageClient.SendMessage(newMessage);

            if (isSent)
            {
                MessageTextbox.Text = string.Empty;
                ChatMessages.Items.Add(newMessage.From + ": " + newMessage.MessageText);
            }
        }


        private async void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            if ((MessageTextbox.Text == "") || (MessageTextbox.Text == "Type your message here..."))
                return;

            MessageClient messageClient = new MessageClient(AccessToken);
            Message newMessage = new Message
            {
                MessageText = MessageTextbox.Text,
                From = Username
            };

            foreach (var contact in Contacts)
            {
                if(ContactsList.SelectedItem.ToString().Contains(contact.Username))
                {
                    newMessage.To = contact.Username;
                }
            }

            var isSent = await messageClient.SendMessage(newMessage);

            if (isSent)
            {
                MessageTextbox.Text = string.Empty;
                ChatMessages.Items.Add(newMessage.From + ": " + newMessage.MessageText);
            }
        }


        private async void RadGridView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChatMessages.Items.Clear();

            ActiveUserClient activeUsers = new ActiveUserClient();
            var activeUserInfo = await activeUsers.ActiveUsers(AccessToken);

            MessageToFrom message = new MessageToFrom
            {
                To = Username
            };

            MessageClient messageClient = new MessageClient(AccessToken);

            foreach (var contact in activeUserInfo)
            {
                if (ContactsList.SelectedItem.ToString().Contains(contact.Username))
                {
                    ContactsList.SelectedItem = contact.Username;
                    message.From = contact.Username;
                    
                    await  messageClient.MakeMessageRead(message);
                    break;
                }
            }

            var messages = await messageClient.ReadAllMessages(message);


            foreach (var mess in messages)
            {
                ChatMessages.Items.Add(mess.From + ": " + mess.MessageText);
            }

            MessageTextbox.Text = "Type your message here...";
        }

        private void ChatMessages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) ChatMessages.Items.Remove(ChatMessages.SelectedItem);
        }

        private void EndVideoCallButtonClick(object sender, RoutedEventArgs e)
        {
            //EndVideoCallButton.IsEnabled = false;
            //StartVideoCallButton.IsEnabled = true;
            VideoFrame.Visibility = Visibility.Collapsed;

            ChatMessages.Visibility = Visibility.Visible;
            MessageTextbox.Visibility = Visibility.Visible;
        }

        private void StartVideoCallButtonClick(object sender, RoutedEventArgs e)
        {
            //Call videoCallService
            StartVideoCallButton.IsEnabled = false;
            //EndVideoCallButton.IsEnabled = true;
            //VideoFrame.Visibility = Visibility.Visible;
            ChatMessages.Visibility = Visibility.Collapsed;
            MessageTextbox.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
        //    HttpClient httpClient = new HttpClient();
        //    await httpClient.DeleteAsync(messageapi);
               ChatMessages.Items.Clear();
        }

        private void SearchTextBoxMouseEnter(object sender, MouseEventArgs e)
        {
            if (SearchTexBox.Text == "Search user")
                SearchTexBox.Text = "";
        }

        private void SearchTextBoxMouseLeave(object sender, MouseEventArgs e)
        {
            if (SearchTexBox.Text == "")
                SearchTexBox.Text = "Search user";
        }
        private void SearchTexBoxTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private async void RadGridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            ChatMessages.Items.Clear();
            
            ActiveUserClient activeUsers = new ActiveUserClient();
            var activeUserInfo = await activeUsers.ActiveUsers(AccessToken);

            MessageToFrom message = new MessageToFrom();
            message.To = Username;

            foreach (var contact in activeUserInfo)
            {
                if (contact.Username.Contains(ContactsList.SelectedItem.ToString()))
                {
                    message.From = contact.Username;
                }
            }

            MessageClient messageClient = new MessageClient(AccessToken);
            var messages = await messageClient.ReadAllMessages(message);
            foreach (var mess in messages)
            {
                ChatMessages.Items.Add(mess.From + ": " + mess.MessageText);
            }
        }

        private void ChatMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                
        }

        private static async Task CallAPIPeriodicAsync(Action onTick,
                                           TimeSpan dueTime,
                                           TimeSpan interval,
                                           CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }
    }
}   