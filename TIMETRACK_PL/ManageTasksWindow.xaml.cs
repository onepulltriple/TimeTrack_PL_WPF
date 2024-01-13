using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TIMETRACK_PL.CRUD_logic;
using TIMETRACK_PL.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace TIMETRACK_PL
{
    /// <summary>
    /// Interaction logic for ManageTasksWindow.xaml
    /// </summary>
    public partial class ManageTasksWindow : Window, INotifyPropertyChanged
    {
        #region Class Members

        readonly TimetrackPlContext _context;

        public event PropertyChangedEventHandler PropertyChanged;

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

        private Entities.Task _selectedTask;
        public Entities.Task SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTask)));
            }
        }

        private Entities.Task _tempTask;
        public Entities.Task TempTask
        {
            get { return _tempTask; }
            set
            {
                _tempTask = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TempTask)));
            }
        }

        private string _enteredStartTime;
        public string EnteredStartTime
        {
            get { return _enteredStartTime; }
            set
            {
                _enteredStartTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnteredStartTime)));
            }
        }

        private string _enteredEndTime;
        public string EnteredEndTime
        {
            get { return _enteredEndTime; }
            set
            {
                _enteredEndTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnteredEndTime)));
            }
        }

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

        #endregion

        public ManageTasksWindow()
        {
            InitializeComponent();
            DataContext = this;
            _context = new();

            LoadAllTasks();
            LoadAllProjects();
        }

        private void LoadAllTasks()
        {
            ListOfTasks = new(_context.Tasks
                .OrderBy(task => task.Name)
                .Include(task => task.Intervals)
                .ToArray()
                );

            ResetButtons();

            CloseUserInputFields();
        }

        private void LoadAllProjects()
        {
            ListOfProjects = new(_context.Projects
                .Where(project => project.IsArchived != true)
                .OrderBy(project => project.Name)
                .ToArray()
                );
        }

        private void CloseUserInputFields()
        {
            UI01.IsEnabled = false;
            CB01.IsEnabled = false;
            UI03.IsEnabled = false;
            UI04.IsEnabled = false;
            UI05.IsEnabled = false;
            UI06.IsEnabled = false;
        }

        private void OpenUserInputFields()
        {
            UI01.IsEnabled = true;
            CB01.IsEnabled = true;
            UI03.IsEnabled = true;
            UI04.IsEnabled = true;
            UI05.IsEnabled = true;
            UI06.IsEnabled = true;
    }

        private void ResetButtons()
        {
            DG01.UnselectAll();
            DG01.IsEnabled = true;

            BACKButton.Visibility = Visibility.Visible;
            CANCButton.Visibility = Visibility.Hidden;

            CREAButton.IsEnabled = true;
            EDITButton.IsEnabled = false;
            SAVEButton.IsEnabled = false;
            DELEButton.IsEnabled = false;
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SAVEButton.IsEnabled ||
                EDITButton.IsEnabled && DELEButton.IsEnabled)
            {
                ResetButtons();
                CloseUserInputFields();
                return;
            }

            CRUDWindowWPF.ReturnToMainWindowAndClose(this);
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            CRUDWindowWPF.ReturnToMainWindowAndClose(this);
        }

        private void DG01SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonsInManageMode();
        }

        private void ButtonsInManageMode()
        {
            BACKButton.Visibility = Visibility.Hidden;
            CANCButton.Visibility = Visibility.Visible;

            CREAButton.IsEnabled = false;
            EDITButton.IsEnabled = true;
            SAVEButton.IsEnabled = false;
            DELEButton.IsEnabled = true;
        }

        private void ButtonsInEditMode()
        {
            BACKButton.Visibility = Visibility.Hidden;
            CANCButton.Visibility = Visibility.Visible;

            CREAButton.IsEnabled = false;
            EDITButton.IsEnabled = false;
            SAVEButton.IsEnabled = true;
            DELEButton.IsEnabled = false;

            DG01.IsEnabled = false;
        }

        private void CREAButtonClicked(object sender, RoutedEventArgs e)
        {
            ButtonsInEditMode();

            OpenUserInputFields();
            CB01.SelectedItem = null;

            TempTask = new();
        }

        private void EDITButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Please select a task.");
                return;
            }

            ButtonsInEditMode();

            OpenUserInputFields();

            TempTask = TransferProperties(new Entities.Task(), SelectedTask);

            // fill out user inputs fields which are not bound to the temp task
            CB01.SelectedItem = TempTask.Project;
        }

        private void SAVEButtonClicked(object sender, RoutedEventArgs e)
        {
            bool ChecksWerePassed = PerformChecksOnUserInput();

            // if checks were passed, transfer properties of CB01 to the temp task
            if (ChecksWerePassed)
            {
                TempTask.ProjectId = SelectedProject.Id;
            }

            // when editing an existing event
            if (ChecksWerePassed && SelectedTask != null)
            {
                SelectedTask = TransferProperties(SelectedTask, TempTask);
                _context.Update(SelectedTask);
                _context.SaveChanges();
                LoadAllTasks();
                return;
            }

            // when editing a new event
            if (ChecksWerePassed)
            {
                _context.Add(TempTask);
                _context.SaveChanges();
                LoadAllTasks();
                return;
            }
        }

        private void DELEButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Please select a task.");
                return;
            }

            MessageBoxResult answer01 = MessageBoxResult.No;
            MessageBoxResult answer02 = MessageBoxResult.No;

            answer01 = MessageBox.Show(
                "Are you sure you want to delete the selected task?",
                "Confirm task deletion.",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer01 == MessageBoxResult.Yes)
            {
                // check if the task contains appearances
                List<Interval> IntervalsBookedForTask = new(
                    SelectedTask.Intervals.ToArray()
                    );

                // if the task contains appearances
                if (!IntervalsBookedForTask.IsNullOrEmpty())
                {
                    int countOfIntervals = IntervalsBookedForTask.Count();

                    string messageToUser =
                        $"The selected task has {countOfIntervals} intervals booked.\n" +
                        $"Deleting this task will delete all of its booked intervals.\n" +
                        $"Are you sure you want to delete the selected task?";

                    // ask follow up question
                    answer02 = MessageBox.Show(messageToUser,
                        "If you do this, you will get what you deserve.",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                }
                else
                {
                    // task is empty and can be deleted without follow up questions asked
                    answer02 = MessageBoxResult.Yes;
                }

                if (answer01 == MessageBoxResult.Yes &&
                    answer02 == MessageBoxResult.Yes)
                {
                    // proceed with task deletion (cascading delete in SQL is used)
                    _context.Remove(SelectedTask);
                    _context.SaveChanges();
                    LoadAllTasks();
                }
            }

            ResetButtons();
        }

        private bool PerformChecksOnUserInput()
        {
            // block if any field is empty
            if (TempTask.Name == null ||
                TempTask.Description == null ||
                CB01.SelectedItem == null)
            {
                MessageBox.Show("Please assign the task to project and fill out a name and a description.");
                return false;
            }

            // if all checks have been passed
            return true;
        }

        private Entities.Task TransferProperties(Entities.Task toFill, Entities.Task origin)
        {
            toFill.Id = origin.Id;
            toFill.Name = origin.Name;
            toFill.Description = origin.Description;
            toFill.ProjectId = origin.ProjectId;

            toFill.Project = origin.Project;

            return toFill;
        }
    }
}
