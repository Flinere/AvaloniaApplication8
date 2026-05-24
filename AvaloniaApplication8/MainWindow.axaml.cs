using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication8.Context;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        private void LoadTasks()
        {
            Tasks.Clear();
            var dbTasks = _context.Tasks
                .Where(t => t.UserId == _userId)
                .OrderByDescending(t => t.Deadline)
                .ToList();

            foreach (var task in dbTasks)
            {
                var deadline = task.Deadline ?? DateTime.UtcNow;
                
                Tasks.Add(new TaskItemViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Deadline = deadline,
                    Priority = task.Priority,
                    IsCompleted = task.Status == "completed"
                });
            }
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

            var task = new Models.Task
            {
                UserId = _userId,
                Title = title,
                Description = "",
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
                    task.Status = taskVm.IsCompleted ? "completed" : "pending";
                    task.CompletedAt = taskVm.IsCompleted ? DateTime.UtcNow : null;
                    task.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                }
            }
        }
    }

    public class TaskItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "Medium";
        public bool IsCompleted { get; set; }
    }
}