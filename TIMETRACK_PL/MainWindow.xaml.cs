using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TIMETRACK_PL.Entities;

namespace TIMETRACK_PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Class Members

        readonly TimetrackPlContext _context;

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Project> _listOfProjects;
        public List<Project> ListOfProjects
        {
            get { return _listOfProjects; }
            set
            {
                _listOfProjects = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfProjects)));
            }
        }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProject)));
            }
        }

        private Project _tempProject;
        public Project TempProject
        {
            get { return _tempProject; }
            set
            {
                _tempProject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TempProject)));
            }
        }

        private List<Entities.Task> _listOfTasks;
        public List<Entities.Task> ListOfTasks
        {
            get { return _listOfTasks; }
            set
            {
                _listOfTasks = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfTasks)));
            }
        }

        private List<Interval> _listOfIntervals;
        public List<Interval> ListOfIntervals
        {
            get { return _listOfIntervals; }
            set
            {
                _listOfIntervals = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfIntervals)));
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _context = new();

            LoadAllProjects();
            PopulateContainer();
        }

        private void PopulateContainer()
        {
            for (int i = 0; i < ListOfProjects.Count(); i++)
            {
                Project tempProject = ListOfProjects[i];
                Button newButton = new();
                newButton.Content = ListOfProjects[i].Name;
                newButton.Name = "ProjectButton" + i.ToString("D" + 2);
                newButton.Height = 80;
                newButton.Width = 100;
                newButton.Click += (object sender, RoutedEventArgs e) => 
                    { ProjectButtonClicked(sender, e, tempProject); };
                WP01.Children.Add(newButton);
            }
        }

        private void ProjectButtonClicked(object sender, RoutedEventArgs e, Project project)
        {
            // open up a new CRUD window and hand over the project
            ManageIntervalsPerProjectWindow MTW = new(project);
            MTW.Show();
            this.Close();
        }

        private void LoadAllProjects()
        {
            ListOfProjects = new(_context.Projects
                .OrderBy(project => project.Number)
                .Include(project => project.Tasks)
                .ToArray()
                );

            //ResetButtons();

            //CloseUserInputFields();
        }

        private void LoadAllTasks()
        {
            ListOfTasks = new(_context.Tasks
                .OrderBy(task => task.Name)
                .Include(task => task.Intervals)
                .ToArray()
                );
        }

        private void LoadAllIntervals()
        {
            ListOfIntervals = new(_context.Intervals
                .ToArray()
                );
        }

        private void MPRJButtonClicked(object sender, RoutedEventArgs e)
        {
            ManageProjectsWindow MPW = new();
            MPW.Show();
            this.Close();
        }

        private void MTASButtonClicked(object sender, RoutedEventArgs e)
        {
            ManageTasksWindow MTW = new();
            MTW.Show();
            this.Close();
        }

        private void EXITButtonClicked(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}