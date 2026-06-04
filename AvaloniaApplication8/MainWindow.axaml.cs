using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication8.Context;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaApplication8.Models;
using Microsoft.EntityFrameworkCore;
using Task = AvaloniaApplication8.Models.Task;
using System.ComponentModel;

namespace AvaloniaApplication8
{
    
    public partial class MainWindow : Window
    {
        private readonly PostgresContext _context;
        private readonly int _userId;
         public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
         
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _context = new PostgresContext();
            TaskList.ItemsSource = Tasks;
            LoadTasks();
        }

        private async void LoadTasks()
        {
            Tasks.Clear();
            var dbTasks =  _context.Tasks
                .Where(t => t.UserId == _userId)
                .OrderByDescending(t => t.Deadline).Include(tas => tas.Category)
                .ToList();

            foreach (var task in dbTasks)
            {
                var deadline = task.Deadline ?? DateTime.UtcNow;
                bool ready = deadline < DateTime.UtcNow;
                Tasks.Add(new TaskItemViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Category = task.Category?.Name ?? "no category",
                    Deadline = deadline,
                    Priority = task.Priority,
                    IsCompleted = task.Status == "completed",
                    ItemBack = ready? new SolidColorBrush(Colors.Red) : Brushes.Transparent,
                });
            }

            var categoryNames = _context.Categories.Select(c => c.Name).ToList();
            Box.ItemsSource = categoryNames;
        }

        private void AddTask_Click(object? sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text?.Trim();
            if (string.IsNullOrEmpty(title)) return;
            var selectedDate = DeadlinePicker.SelectedDate;
            var deadline = selectedDate.HasValue 
                ? selectedDate.Value.UtcDateTime 
                : DateTime.UtcNow.AddDays(1);
            
            var priority = PriorityBox.SelectedItem?.ToString() ?? "Medium";
            var categoryid = Box.SelectionBoxItem as Category;

            var task = new Models.Task
            {
                UserId = _userId,
                Title = title,
                Description = "Пустое описаниe",
                CategoryId = categoryid?.Id ?? 1,
                Deadline = deadline,
                Priority = priority.ToLower(),
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();
            LoadTasks();

            TitleBox.Text = "";
            DeadlinePicker.SelectedDate = new DateTimeOffset(DateTime.UtcNow.AddDays(1));
            PriorityBox.SelectedIndex = 1;
        }

        private void TaskCheckBox_Changed(object? sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskItemViewModel taskVm)
            {
                var task = _context.Tasks.Find(taskVm.Id);
                if (task != null)
                {
                    task.Status = taskVm.IsCompleted ? "pending" : "completed";
                    task.CompletedAt = taskVm.IsCompleted ? DateTime.UtcNow : null;
                    task.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                }
            }
        }

        private void TaskList_OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            PostgresContext dbContext = new PostgresContext();
            var task = TaskList.SelectedItem as TaskItemViewModel;
            int taskid = task.Id;
            new DescriptionWindow(taskid).ShowDialog(this);
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private async void Button_OnClick2(object? sender, RoutedEventArgs e)
        {
            var task = TaskList.SelectedItem as TaskItemViewModel;
            if (TaskList.SelectedItem == null)
            {
                return;
            }
            int id = task.Id;
            var del =  await _context.Tasks.FindAsync(id);
            if (del != null)
            {
                _context.Tasks.Remove(del);
                await _context.SaveChangesAsync();
                LoadTasks();
            }
        }
    }

    public class TaskItemViewModel
    {
        public IBrush ItemBack { get; set; } = Brushes.Transparent;
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Category{get;set;}
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "Medium";
        public bool IsCompleted { get; set; }
    }
}