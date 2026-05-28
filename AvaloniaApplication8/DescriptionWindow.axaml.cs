using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication8.Context;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication8;

public partial class DescriptionWindow : Window
{
    int idtask{get;set;}
    public DescriptionWindow()
    {
        InitializeComponent();
    }

    public DescriptionWindow(int taskid)
    {
        InitializeComponent();
        idtask = taskid;
        PostgresContext  dbContext = new PostgresContext();
        var sd = dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == idtask);
        Box.Text = sd.Result.Description;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Button_OnClick2(object? sender, RoutedEventArgs e)
    {
        PostgresContext db = new PostgresContext();
        string descr = "Пустое описание";
        if (Box.Text != null)
        {
            descr = Box.Text;
        }
        var task = db.Tasks.FirstOrDefaultAsync(t => t.Id == idtask);
        task.Result.Description = descr;
        db.SaveChanges();
        this.Close();
    }
}