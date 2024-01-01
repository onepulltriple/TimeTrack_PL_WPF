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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using TIMETRACK_PL.Entities;
using TIMETRACK_PL.CRUD_logic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace TIMETRACK_PL
{
    /// <summary>
    /// Interaction logic for ManageProjects.xaml
    /// </summary>
    public partial class ManageProjectsWindow : Window, INotifyPropertyChanged
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
        public ManageProjectsWindow()
        {
            InitializeComponent();
            DataContext = this;
            _context = new();

            LoadAllProjects();
        }

        private void LoadAllProjects()
        {
            ListOfProjects = new(_context.Projects
                .OrderBy(project => project.Number)
                .Include(project => project.Tasks)
                .ToArray()
                );

            ResetButtons();

            CloseUserInputFields();
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


        private void CloseUserInputFields()
        {
            UI01.IsEnabled = false;
            UI02.IsEnabled = false;
            UI03.IsEnabled = false;
            UI04.IsEnabled = false;
            UI05.IsEnabled = false;
            UI06.IsEnabled = false;
            UI07.IsEnabled = false;
            CB01.IsEnabled = false;
        }

        private void OpenUserInputFields()
        {
            UI01.IsEnabled = true;
            UI02.IsEnabled = true;
            UI03.IsEnabled = true;
            UI04.IsEnabled = true;
            UI05.IsEnabled = true;
            UI06.IsEnabled = true;
            UI07.IsEnabled = true;
            CB01.IsEnabled = true;
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

            TempProject = new();

            // clear user inputs fields which are not bound to the temp project
            CB01.IsChecked = false;
        }

        private void EDITButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedProject == null)
            {
                MessageBox.Show("Please select a project.");
                return;
            }

            ButtonsInEditMode();
            OpenUserInputFields();

            TempProject = TransferProperties(new Project(), SelectedProject);

            // fill out user inputs fields which are not bound to the temp project
            CB01.IsChecked = TempProject.IsArchived;
        }

        private void SAVEButtonClicked(object sender, RoutedEventArgs e)
        {
            bool ChecksWerePassed = PerformChecksOnUserInput();

            // when editing an existing project
            if (ChecksWerePassed && SelectedProject != null)
            {
                TempProject.IsArchived = CB01.IsChecked.Value;

                SelectedProject = TransferProperties(SelectedProject, TempProject);
                _context.Update(SelectedProject);
                _context.SaveChanges();
                LoadAllProjects();
                return;
            }

            // when editing a new project
            if (ChecksWerePassed)
            {
                TempProject.IsArchived = CB01.IsChecked.Value;

                _context.Add(TempProject);
                _context.SaveChanges();
                LoadAllProjects();
                return;
            }
        }

        private void DELEButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedProject == null)
            {
                MessageBox.Show("Please select a project.");
                return;
            }

            MessageBoxResult answer01 = MessageBoxResult.No;
            MessageBoxResult answer02 = MessageBoxResult.No;

            answer01 = MessageBox.Show(
                "Are you sure you want to delete the selected project?",
                "Confirm project deletion.",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer01 == MessageBoxResult.Yes)
            {
                // check if the project contains tasks
                List<Entities.Task> TasksAssignedToProject = new(
                    SelectedProject.Tasks.ToArray()
                    );

                // if the project contains tasks           
                if (!TasksAssignedToProject.IsNullOrEmpty())
                {
                    int countOfTasks = TasksAssignedToProject.Count();

                    string messageToUser =
                        $"The selected project has\n" +
                        $"{countOfTasks} tasks assigned to it.\n" +
                        $"Deleting this project will delete all of its assigned tasks.\n" +
                        $"Are you sure you want to delete the selected project?";

                    // ask follow up question
                    answer02 = MessageBox.Show(messageToUser,
                        "If you do this, you will get what you deserve.",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                }
                else
                {
                    // project is empty and can be deleted without follow up questions asked
                    answer02 = MessageBoxResult.Yes;
                }

                if (answer01 == MessageBoxResult.Yes &&
                    answer02 == MessageBoxResult.Yes)
                {
                    // proceed with project deletion (cascading delete in SQL is used)
                    _context.Remove(SelectedProject);
                    _context.SaveChanges();
                    LoadAllProjects();
                }
            }

            ResetButtons();
        }

        private bool PerformChecksOnUserInput()
        {
            // block if a required field is empty
            if (TempProject.Name == null ||
                TempProject.Number == null )
            {
                MessageBox.Show("Please fill out at least the project name and number.");
                return false;
            }

            // if all checks have been passed
            return true;
        }

        private Project TransferProperties(Project toFill, Project origin)
        {
            toFill.Id = origin.Id;
            toFill.Name = origin.Name;
            toFill.Number = origin.Number;
            toFill.Description = origin.Description;
            toFill.IsArchived = origin.IsArchived;

            return toFill;
        }
    }
}
