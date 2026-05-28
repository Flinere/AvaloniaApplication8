using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Text.RegularExpressions;
using AvaloniaApplication8.Context;
using AvaloniaApplication8.Models;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication8;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        error.Text = "";
        string name = Box.Text;
        string mail = Box2.Text;
        string password = Box3.Text;
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
        {
            error.Text = "Заполните все";
            return;
        }

        if (Box.Text.Length > 50)
        {
            error.Text = "Имя не может быть больше 50";
            return;
        }

        password = password.Replace(" ", "");
        string pattern2 = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
        Match isMatch = Regex.Match(Box2.Text, pattern2, RegexOptions.IgnoreCase);
        if (!isMatch.Success)
        {
            error.Text = "Почта не правильной формы";
            return;
        }

        PostgresContext bd = new PostgresContext();
        var newus = new User()
        {
            Username = name,
            Email = mail,
            PasswordHash = password,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        bd.Users.Add(newus);
        bd.SaveChanges();
        new LoginWindow().Show();
        this.Close();
    }

    private void Button_OnClick1(object? sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        this.Close();
    }
}