using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication8.Context;
using AvaloniaApplication8.Models;
using System;
using System.Linq;

namespace AvaloniaApplication8
{
    public partial class LoginWindow : Window
    {
        private readonly PostgresContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new PostgresContext();
        }

        private void Login_Click(object? sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text?.Trim();
            var password = PasswordBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            var user = _context.Users.FirstOrDefault(u => 
                u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                var mainWindow = new MainWindow(user.Id);
                mainWindow.Show();
                Close();
            }
            else
            {
                ShowError("Неверный логин или пароль");
            }
        }

        private void Register_Click(object? sender, RoutedEventArgs e)
        {
            new RegisterWindow().Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.IsVisible = true;
        }
    }
}