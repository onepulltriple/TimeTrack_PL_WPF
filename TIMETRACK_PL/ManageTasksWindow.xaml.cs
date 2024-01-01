using Microsoft.EntityFrameworkCore;
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
        public List<Entities.Task> ListOfProjectTasks
        {
            get { return _listOfTasks; }
            set
            {
                _listOfTasks = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfProjectTasks)));
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

        private Interval _selectedInterval;
        public Interval SelectedInterval
        {
            get { return _selectedInterval; }
            set
            {
                _selectedInterval = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedInterval)));
            }
        }

        private Interval _tempInterval;
        public Interval TempInterval
        {
            get { return _tempInterval; }
            set
            {
                _tempInterval = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TempInterval)));
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

        public ManageTasksWindow(Project HandedOverProject)
        {
            InitializeComponent();
            DataContext = this;
            _context = new();
            SelectedProject = HandedOverProject;

            LoadAllProjectTasks();
            //LoadAllIntervals();
        }

        private void LoadAllProjectTasks()
        {
            ListOfProjectTasks = new(_context.Tasks
                .Where(task => task.Project == SelectedProject)
                .OrderBy(task => task.Name)
                .Include(task => task.Intervals)
                .ToArray()
                );

            ResetButtons01();

            CloseUserInputFields01();
        }

        //private void LoadAllProjects()
        //{
        //    ListOfProjects = new(_context.Projects
        //        .OrderBy(project => project.Number)
        //        .ToArray()
        //        );
        //}

        private void LoadAllIntervals()
        {
            ListOfIntervals = new(_context.Intervals
                .OrderBy(interval => interval.StartTimeActual)
                .ToArray()
                );
        }

        private void CloseUserInputFields01()
        {
            UI0101.IsEnabled = false;
            UI0102.IsEnabled = false;
            UI0103.IsEnabled = false;
            UI0104.IsEnabled = false;

            GRID01.IsEnabled = false;
        }

        private void CloseUserInputFields02()
        {
            UI0201.IsEnabled = false;
            CB0201.IsEnabled = false;
            UI0203.IsEnabled = false;
            DP0201.IsEnabled = false;
            UI0205.IsEnabled = false;
            UI0206.IsEnabled = false;
            UI0207.IsEnabled = false;
            UI0208.IsEnabled = false;

            //GRID01.IsEnabled = false;
        }

        private void OpenUserInputFields01()
        {
            UI0101.IsEnabled = true;
            UI0102.IsEnabled = true;
            UI0103.IsEnabled = true;
            UI0104.IsEnabled = true;

            GRID01.IsEnabled = true;
            DG02.IsEnabled = true;
        }

        private void OpenUserInputFields02()
        {
            UI0203.IsEnabled = true;
            DP0201.IsEnabled = true;
            UI0205.IsEnabled = true;
            UI0206.IsEnabled = true;
            UI0207.IsEnabled = true;
            UI0208.IsEnabled = true;

            //GRID01.IsEnabled = true;
            //DG02.IsEnabled = true;
        }

        private void ResetButtons01()
        {
            DG01.UnselectAll();
            DG01.IsEnabled = true;

            BACKButton01.Visibility = Visibility.Visible;
            CANCButton01.Visibility = Visibility.Hidden;

            CREAButton01.IsEnabled = true;
            EDITButton01.IsEnabled = false;
            SAVEButton01.IsEnabled = false;
            DELEButton01.IsEnabled = false;
        }

        private void ResetButtons02()
        {
            DG02.UnselectAll();
            DG02.IsEnabled = true;

            BACKButton02.Visibility = Visibility.Visible;
            CANCButton02.Visibility = Visibility.Hidden;
                       
            CREAButton02.IsEnabled = true;
            EDITButton02.IsEnabled = false;
            SAVEButton02.IsEnabled = false;
            DELEButton02.IsEnabled = false;
        }

        private void CancelButton01Clicked(object sender, RoutedEventArgs e)
        {
            if (SAVEButton01.IsEnabled ||
                EDITButton01.IsEnabled && DELEButton01.IsEnabled)
            {
                ResetButtons01();
                CloseUserInputFields01();
                return;
            }

            CRUDWindowWPF.ReturnToMainWindowAndClose(this);
        }
        private void CancelButton02Clicked(object sender, RoutedEventArgs e)
        {
            if (SAVEButton02.IsEnabled ||
                EDITButton02.IsEnabled && DELEButton02.IsEnabled)
            {
                ResetButtons02();
                CloseUserInputFields02();
                return;
            }

            CRUDWindowWPF.ReturnToMainWindowAndClose(this);
        }

        private void BackButton01Clicked(object sender, RoutedEventArgs e)
        {
            CRUDWindowWPF.ReturnToMainWindowAndClose(this);
        }

        private void BackButton02Clicked(object sender, RoutedEventArgs e)
        {
            CRUDWindowWPF.ReturnToMainWindowAndClose(this);
        }

        private void DG01SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonsInManageMode01();
        }

        private void DG02SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonsInManageMode02();
        }

        private void CB0201SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // when the selected item is switched to null, do nothing further
            if (CB0201.SelectedItem == null)
                return;

            // open remaining user input fields, since a task has been selected
            OpenUserInputFields02();

            //// fill in user input fields based on the selected interval
            //DP0201.SelectedDate = SelectedInterval.StartTimeActual;
            //EnteredStartTime = SelectedInterval.StartTimeActual.ToString("HH:mm");
            //if (SelectedInterval.EndTimeActual != null)
            //{
            //    EnteredEndTime = SelectedInterval.EndTimeActual?.ToString("HH:mm");
            //}
        }

        private void ButtonsInManageMode01()
        {
            BACKButton01.Visibility = Visibility.Hidden;
            CANCButton01.Visibility = Visibility.Visible;

            CREAButton01.IsEnabled = false;
            EDITButton01.IsEnabled = true;
            SAVEButton01.IsEnabled = false;
            DELEButton01.IsEnabled = true;
        }

        private void ButtonsInManageMode02()
        {
            BACKButton02.Visibility = Visibility.Hidden;
            CANCButton02.Visibility = Visibility.Visible;
                       
            CREAButton02.IsEnabled = false;
            EDITButton02.IsEnabled = true;
            SAVEButton02.IsEnabled = false;
            DELEButton02.IsEnabled = true;
        }

        private void ButtonsInEditMode01()
        {
            BACKButton01.Visibility = Visibility.Hidden;
            CANCButton01.Visibility = Visibility.Visible;
                
            CREAButton01.IsEnabled = false;
            EDITButton01.IsEnabled = false;
            SAVEButton01.IsEnabled = true;
            DELEButton01.IsEnabled = false;

            DG01.IsEnabled = false;
        }

        private void ButtonsInEditMode02()
        {
            BACKButton02.Visibility = Visibility.Hidden;
            CANCButton02.Visibility = Visibility.Visible;
                       
            CREAButton02.IsEnabled = false;
            EDITButton02.IsEnabled = false;
            SAVEButton02.IsEnabled = true;
            DELEButton02.IsEnabled = false;

            DG02.IsEnabled = false;
        }

        private void CREAButton01Clicked(object sender, RoutedEventArgs e)
        {
            ButtonsInEditMode01();

            TempTask = new();
        }

        private void EDITButton01Clicked(object sender, RoutedEventArgs e)
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Please select a task.");
                return;
            }

            ButtonsInEditMode01();

            OpenUserInputFields01();

            TempTask = TransferTaskProperties(new Entities.Task(), SelectedTask);
        }

        private void SAVEButton01Clicked(object sender, RoutedEventArgs e)
        {
            bool ChecksWerePassed = PerformChecksOnUserInput01();

            // when editing an existing task
            if (ChecksWerePassed && SelectedTask != null)
            {
                SelectedTask = TransferTaskProperties(SelectedTask, TempTask);
                _context.Update(SelectedTask);
                _context.SaveChanges();
                LoadAllProjectTasks();
                return;
            }

            // when editing a new task
            if (ChecksWerePassed)
            {
                _context.Add(TempTask);
                _context.SaveChanges();
                LoadAllProjectTasks();
                return;
            }
        }

        private void DELEButton01Clicked(object sender, RoutedEventArgs e)
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
                // check if the Task contains appearances
                List<Interval> IntervalsLoggedForTask = new(
                    SelectedTask.Intervals.ToArray()
                    );

                // if the Task contains appearances
                if (!IntervalsLoggedForTask.IsNullOrEmpty())
                {
                    int countOfIntervals = IntervalsLoggedForTask.Count();

                    string messageToUser =
                        $"The selected task has\n" +
                        $"{countOfIntervals} intervals logged.\n" +
                        $"Deleting this task will delete all of its logged intervals.\n" +
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
                    LoadAllProjectTasks();
                }
            }

            ResetButtons01();
        }

        private void CREAButton02Clicked(object sender, RoutedEventArgs e)
        {
            ButtonsInEditMode02();

            // open combo box only to select an event first
            CB0201.SelectedItem = null;
            UI0201.IsEnabled = true;
            CB0201.IsEnabled = true;

            TempInterval = new();

            // clear user inputs fields which are not bound to the temp interval
            DP0201.SelectedDate = DateTime.Now;
            EnteredStartTime = null;
            EnteredEndTime = null;
        }

        private void EDITButton02Clicked(object sender, RoutedEventArgs e)
        {
            if (SelectedInterval == null)
            {
                MessageBox.Show("Please select an interval.");
                return;
            }

            ButtonsInEditMode02();
            UI0201.IsEnabled = true;
            CB0201.IsEnabled = true;
            OpenUserInputFields02();

            TempInterval = TransferIntervalProperties(new Interval(), SelectedInterval);

            // fill out user inputs fields which are not bound to the temp interval
            DP0201.SelectedDate = TempInterval.StartTimeActual;
            EnteredStartTime = TempInterval.StartTimeActual.ToString("HH:mm");
            if (SelectedInterval.EndTimeActual != null)
            {
                EnteredEndTime = TempInterval.EndTimeActual?.ToString("HH:mm");
            }
            CB0201.SelectedItem = TempInterval.Task;
        }

        private void SAVEButton02Clicked(object sender, RoutedEventArgs e)
        {
            bool ChecksWerePassed = PerformChecksOnUserInput02();

            // if checks were passed, transfer properties of CB0201 to the temp interval
            if (ChecksWerePassed)
            {
                TempInterval.TaskId = SelectedTask.Id;
            }

            // when editing an existing interval
            if (ChecksWerePassed && SelectedInterval != null)
            {
                SelectedInterval = TransferIntervalProperties(SelectedInterval, TempInterval);
                _context.Update(SelectedInterval);
                _context.SaveChanges();
                LoadAllIntervals();
                return;
            }

            // when editing a new interval
            if (ChecksWerePassed)
            {
                _context.Add(TempInterval);
                _context.SaveChanges();
                LoadAllIntervals();
                return;
            }
        }

        private void DELEButton02Clicked(object sender, RoutedEventArgs e)
        {
            if (SelectedInterval == null)
            {
                MessageBox.Show("Please select an interval.");
                return;
            }

            MessageBoxResult answer01 = MessageBoxResult.No;

            answer01 = MessageBox.Show(
                "Are you sure you want to delete the selected interval?",
                "Confirm interval deletion.",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer01 == MessageBoxResult.Yes)
            {
                // proceed with interval deletion
                _context.Remove(SelectedInterval);
                _context.SaveChanges();
                LoadAllIntervals();
            }

            ResetButtons02();
        }

        private bool PerformChecksOnUserInput01()
        {
            // block if a required field is empty
            if (TempTask.Name == null)
            {
                MessageBox.Show("Please give the task a name.");
                return false;
            }

            // if all checks have been passed
            return true;
        }

        private bool PerformChecksOnUserInput02()
        {
            // block if a required field is empty
            if (CB0201.SelectedItem == null ||
                DP0201.SelectedDate == null ||
                EnteredStartTime == null )
            {
                MessageBox.Show("Please fill out a start date and start time and ensure a task is selected.");
                return false;
            }

            // block if a non-parsable start time has been entered
            bool IsStartTimeUseable = DateTime.TryParse(EnteredStartTime, out DateTime useableStartTime);

            if (!IsStartTimeUseable)
            {
                MessageBox.Show("Please enter a useable start time, e.g. 14:02 or 2:02 PM.");
                return false;
            }

            // assemble StartTime
            TempInterval.StartTimeActual = new DateTime(
                DP0201.SelectedDate.Value.Year,
                DP0201.SelectedDate.Value.Month,
                DP0201.SelectedDate.Value.Day,
                useableStartTime.Hour,
                useableStartTime.Minute,
                useableStartTime.Second
                );

            // block if StartTimeActual is earlier than the SQL database minimum 
            if (UserInputValidation.SQLDatabaseChecks.IsLowerEqualThanSQLDatabaseMinimum(TempInterval.StartTimeActual))
            {
                MessageBox.Show("Please enter a later start time.");
                return false;
            }


            // check for time conflicts (excluding the interval being edited, if applicable)
            // check that StartTimeActual does not occur during an existing interval 
            Interval StartTimeConflict = _context.Intervals
                .FirstOrDefault(Interval =>
                TempInterval.StartTimeActual >= Interval.StartTimeActual &&
                TempInterval.StartTimeActual <= Interval.EndTimeActual &&
                TempInterval.Id != Interval.Id
                );

            if (StartTimeConflict != null)
            {
                MessageBox.Show("The start time conflicts with an existing interval. Please adjust the start time.");
                return false;
            }


            // if an end time was given
            if (EnteredEndTime != null)
            {
                // block if a non-parsable end time has been entered
                bool IsEndTimeUseable = DateTime.TryParse(EnteredEndTime, out DateTime useableEndTime);

                if (!IsEndTimeUseable)
                {
                    MessageBox.Show("Please enter a useable end time, e.g. 14:02 or 2:02 PM.");
                    return false;
                }

                // assemble EndTime
                TempInterval.EndTimeActual = new DateTime(
                    DP0201.SelectedDate.Value.Year,
                    DP0201.SelectedDate.Value.Month,
                    DP0201.SelectedDate.Value.Day,
                    useableEndTime.Hour,
                    useableEndTime.Minute,
                    useableEndTime.Second
                    );

                // block if StartTime occurs after EndTime
                if (TempInterval.StartTimeActual >= TempInterval.EndTimeActual)
                {
                    MessageBox.Show("Please ensure that the start time occurs before the end time.");
                    return false;
                }

                // block if EndTimeActual is earlier than the SQL database minimum 
                if (UserInputValidation.SQLDatabaseChecks.IsLowerEqualThanSQLDatabaseMinimum(TempInterval.EndTimeActual ?? DateTime.Now))
                {
                    MessageBox.Show("Please enter a later end time.");
                    return false;
                }

                // check for time conflicts (excluding the interval being edited, if applicable)
                // check that EndTimeActual does not occur during an existing interval
                Interval EndTimeConflict = _context.Intervals
                    .FirstOrDefault(Interval =>
                    TempInterval.EndTimeActual >= Interval.StartTimeActual &&
                    TempInterval.EndTimeActual <= Interval.EndTimeActual &&
                    TempInterval.Id != Interval.Id
                    );

                if (EndTimeConflict != null)
                {
                    MessageBox.Show("The end time conflicts with an existing interval. Please adjust the end time.");
                    return false;
                }

                // check that StartTimeActual and EndTimeActual do not occur before and after an existing interval, respectively 
                Interval EventOverlappedConflict = _context.Intervals
                    .FirstOrDefault(Interval =>
                    TempInterval.StartTimeActual <= Interval.StartTimeActual &&
                    TempInterval.EndTimeActual >= Interval.EndTimeActual &&
                    TempInterval.Id != Interval.Id
                    );

                if (EventOverlappedConflict != null)
                {
                    MessageBox.Show("The entered times encompass an existing interval. Please adjust the start and/or end times.");
                    return false;
                }
            }

            // if all checks have been passed
            return true;
        }

        private Entities.Task TransferTaskProperties(Entities.Task toFill, Entities.Task origin)
        {
            toFill.Id = origin.Id;
            toFill.Description = origin.Description;
            toFill.ProjectId = origin.ProjectId;

            return toFill;
        }

        private Interval TransferIntervalProperties(Interval toFill, Interval origin)
        {
            toFill.Id = origin.Id;
            toFill.StartTimeActual = origin.StartTimeActual;
            toFill.EndTimeActual = origin.EndTimeActual;
            toFill.StartTimeRounded = origin.StartTimeRounded;
            toFill.EndTimeRounded = origin.EndTimeRounded;
            toFill.TaskId = origin.TaskId;

            return toFill;
        }
    }
}
