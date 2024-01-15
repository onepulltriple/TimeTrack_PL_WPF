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
    /// Interaction logic for ManageIntervalsPerProjectWindow.xaml
    /// </summary>
    public partial class ManageIntervalsPerProjectWindow : Window, INotifyPropertyChanged
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

        private List<IntervalsAndTasksPerProject> _listOfIntervalsAndTasksPerProjects;
        public List<IntervalsAndTasksPerProject> ListOfIntervalsAndTasksPerProjects
        {
            get { return _listOfIntervalsAndTasksPerProjects; }
            set
            {
                _listOfIntervalsAndTasksPerProjects = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfIntervalsAndTasksPerProjects)));
            }
        }

        private IntervalsAndTasksPerProject _selectedIntervalsAndTasksPerProject;
        public IntervalsAndTasksPerProject SelectedIntervalsAndTasksPerProject
        {
            get { return _selectedIntervalsAndTasksPerProject; }
            set
            {
                _selectedIntervalsAndTasksPerProject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIntervalsAndTasksPerProject)));
            }
        }

        private IntervalsAndTasksPerProject _tempIntervalsAndTasksPerProject;
        public IntervalsAndTasksPerProject TempIntervalsAndTasksPerProject
        {
            get { return _tempIntervalsAndTasksPerProject; }
            set
            {
                _tempIntervalsAndTasksPerProject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TempIntervalsAndTasksPerProject)));
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////
        public ManageIntervalsPerProjectWindow(Project HandedOverProject)
        {
            InitializeComponent();
            DataContext = this;
            _context = new();
            SelectedProject = HandedOverProject;

            LoadAllIntervalsAndTasksPerProjects();
            LoadAllProjectTasks();
        }

        private void LoadAllProjectTasks()
        {
            ListOfProjectTasks = new(_context.Tasks
                .Where(task => task.Project == SelectedProject)
                .OrderBy(task => task.Name)
                .Include(task => task.Intervals)
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
            DP01.IsEnabled = false;
            UI07.IsEnabled = false;
            CB01.IsEnabled = false;
            //UI09.IsEnabled = false;
            //UI10.IsEnabled = false;
            //UI11.IsEnabled = false;
            //UI12.IsEnabled = false;
        }

        private void OpenUserInputFields()
        {
            UI01.IsEnabled = true;
            UI02.IsEnabled = true;
            UI03.IsEnabled = true;
            UI04.IsEnabled = true;
            UI05.IsEnabled = true;
            DP01.IsEnabled = true;
            UI07.IsEnabled = true;
            CB01.IsEnabled = true;
            //UI09.IsEnabled = true;
            //UI10.IsEnabled = true;
            //UI11.IsEnabled = true;
            //UI12.IsEnabled = true;
        }

        private void ResetButtons()
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

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SAVEButton01.IsEnabled ||
                EDITButton01.IsEnabled && DELEButton01.IsEnabled)
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

        private void CB01SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // when the selected item is switched to null, do nothing further
            if (CB01.SelectedItem == null)
                return;

            // open remaining user input fields, since a task has been selected
            //OpenUserInputFields02();

            //// fill in user input fields based on the selected interval
            //DP0201.SelectedDate = SelectedInterval.StartTimeActual;
            //EnteredStartTime = SelectedInterval.StartTimeActual.ToString("HH:mm");
            //if (SelectedInterval.EndTimeActual != null)
            //{
            //    EnteredEndTime = SelectedInterval.EndTimeActual?.ToString("HH:mm");
            //}
        }

        private void ButtonsInManageMode()
        {
            BACKButton01.Visibility = Visibility.Hidden;
            CANCButton01.Visibility = Visibility.Visible;

            CREAButton01.IsEnabled = false;
            EDITButton01.IsEnabled = true;
            SAVEButton01.IsEnabled = false;
            DELEButton01.IsEnabled = true;
        }

        private void ButtonsInEditMode()
        {
            BACKButton01.Visibility = Visibility.Hidden;
            CANCButton01.Visibility = Visibility.Visible;
                
            CREAButton01.IsEnabled = false;
            EDITButton01.IsEnabled = false;
            SAVEButton01.IsEnabled = true;
            DELEButton01.IsEnabled = false;

            DG01.IsEnabled = false;
        }

        private bool PerformChecksOnUserInput()
        {
            // block if a required field is empty
            //if (TempTask.Name == null)
            //{
            //    MessageBox.Show("Please give the task a name.");
            //    return false;
            //}

            // block if no task has been selected
            if (SelectedTask == null)
            {
                MessageBox.Show("Please select a task.");
                return false;
            }


            // block if a required field is empty
            if (CB01.SelectedItem == null ||
                DP01.SelectedDate == null ||
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
                DP01.SelectedDate.Value.Year,
                DP01.SelectedDate.Value.Month,
                DP01.SelectedDate.Value.Day,
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
                TempInterval.StartTimeActual < Interval.EndTimeActual &&
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
                    DP01.SelectedDate.Value.Year,
                    DP01.SelectedDate.Value.Month,
                    DP01.SelectedDate.Value.Day,
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
                    TempInterval.EndTimeActual > Interval.StartTimeActual &&
                    TempInterval.EndTimeActual <= Interval.EndTimeActual &&
                    TempInterval.Id != Interval.Id
                    );

                if (EndTimeConflict != null)
                {
                    MessageBox.Show("The end time conflicts with an existing interval. Please adjust the end time.");
                    return false;
                }

                // check that StartTimeActual and EndTimeActual do not occur before and after an existing interval, respectively 
                Interval IntervalOverlappedConflict = _context.Intervals
                    .FirstOrDefault(Interval =>
                    TempInterval.StartTimeActual <= Interval.StartTimeActual &&
                    TempInterval.EndTimeActual >= Interval.EndTimeActual &&
                    TempInterval.Id != Interval.Id
                    );

                if (IntervalOverlappedConflict != null)
                {
                    MessageBox.Show("The entered times encompass an existing interval. Please adjust the start and/or end times.");
                    return false;
                }
            }

            // if all checks have been passed
            return true;
        }

        private void LoadAllIntervalsAndTasksPerProjects()
        {
            ListOfIntervalsAndTasksPerProjects = new(_context.IntervalsAndTasksPerProjects
                .Where(intervalsandtasksperproject => intervalsandtasksperproject.ProjectId == SelectedProject.Id)
                .OrderBy(intervalsandtasksperproject => intervalsandtasksperproject.ActualStartTime)
                .ToArray()
                );

            ResetButtons();

            CloseUserInputFields();
        }

        private void CREAButtonClicked(object sender, RoutedEventArgs e)
        {
            ButtonsInEditMode();

            OpenUserInputFields();

            // create a new task and a new interval
            //TempTask = new();
            TempInterval = new();

            // clear user inputs fields which are not bound to the temp interval
            EnteredStartTime = DateTime.Now.ToString("HH:mm");
            EnteredEndTime = null;
            DP01.SelectedDate = DateTime.Now;


            //CB01.SelectedItem = ListOfProjectTasks.Select(task => task).Where(interval)
            //TempIntervalsAndTasksPerProject = new();
            //TempIntervalsAndTasksPerProject.ProjectId = SelectedProject.Id;

        }

        private void EDITButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedIntervalsAndTasksPerProject == null)
            {
                MessageBox.Show("Please select an interval to edit.");
                return;
            }

            ButtonsInEditMode();

            OpenUserInputFields();

            // create two temp objects based on the components of the composite object
            //TempTask = TransferPropertiesFromCompositeToTask(new Entities.Task(), SelectedIntervalsAndTasksPerProject);
            TempInterval = TransferPropertiesFromCompositeToInterval(new Interval(), SelectedIntervalsAndTasksPerProject);
            SelectedTask = ListOfProjectTasks.FirstOrDefault(task => task.Id == SelectedIntervalsAndTasksPerProject.TaskId);
            SelectedInterval = SelectedTask.Intervals.FirstOrDefault(interval => interval.Id == SelectedIntervalsAndTasksPerProject.IntervalId);

            // fill out user inputs fields which are not bound to the temp compnents objects
            EnteredStartTime = TempInterval.StartTimeActual.ToString("HH:mm");
            DP01.SelectedDate = TempInterval.StartTimeActual;
            if (TempInterval.EndTimeActual != null)
            {
                EnteredEndTime = TempInterval.EndTimeActual?.ToString("HH:mm");
            }
            CB01.SelectedItem = SelectedTask;
        }

        private void SAVEButtonClicked(object sender, RoutedEventArgs e)
        {
            bool ChecksWerePassed = PerformChecksOnUserInput();

            if (!ChecksWerePassed)
            {
                return;
            }

            // if checks were passed, transfer properties of CB01 to the temp interval
            TempInterval.TaskId = SelectedTask.Id;

            //when editing an existing interval
            if (SelectedIntervalsAndTasksPerProject != null)
            {
                //SelectedIntervalsAndTasksPerProject.ProjectId = SelectedProject.Id;
                //SelectedIntervalsAndTasksPerProject = TransferIntervalsAndTasksPerProjectProperties(SelectedIntervalsAndTasksPerProject, TempIntervalsAndTasksPerProject);

                //SelectedTask = TransferTaskProperties(SelectedTask, TempTask);
                //_context.Update(SelectedTask);

                SelectedInterval = TransferIntervalProperties(SelectedInterval, TempInterval);
                _context.Update(SelectedInterval);
                _context.SaveChanges();
                LoadAllIntervalsAndTasksPerProjects();
                return;
            }

            // when editing a new interval
            _context.Add(TempInterval);
            _context.SaveChanges();
            LoadAllIntervalsAndTasksPerProjects();
            return;
        }

        private void DELEButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedIntervalsAndTasksPerProject == null)
            {
                MessageBox.Show("Please select an interval.");
                return;
            }

            SelectedTask = ListOfProjectTasks.FirstOrDefault(task => task.Id == SelectedIntervalsAndTasksPerProject.TaskId);
            SelectedInterval = SelectedTask.Intervals.FirstOrDefault(interval => interval.Id == SelectedIntervalsAndTasksPerProject.IntervalId);

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
                LoadAllIntervalsAndTasksPerProjects();
            }

            ResetButtons();

            // clear CB01 
            CB01.SelectedItem = null;
        }

        private Entities.Task TransferPropertiesFromCompositeToTask(Entities.Task taskToFill, IntervalsAndTasksPerProject origin)
        {
            taskToFill.Id = origin.TaskId;
            taskToFill.Name = origin.TaskName;
            taskToFill.Description = origin.TaskDescription;

            return taskToFill;
        }

        private Interval TransferPropertiesFromCompositeToInterval(Interval IntervalToFill, IntervalsAndTasksPerProject origin)
        {
            IntervalToFill.Id = origin.IntervalId;
            IntervalToFill.StartTimeActual = origin.ActualStartTime;
            IntervalToFill.EndTimeActual = origin.ActualEndTime;

            return IntervalToFill;
        }
        private Entities.Task TransferTaskProperties(Entities.Task toFill, Entities.Task origin)
        {
            toFill.Id = origin.Id;
            toFill.Name = origin.Name;
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
