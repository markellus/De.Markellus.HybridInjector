using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using De.Markellus.HybridInjector.Misc;

namespace De.Markellus.HybridInjector.Xaml
{
    /// <summary>
    /// Interaction logic for SelectProcessDialog.xaml
    /// </summary>
    public partial class SelectProcessDialog : UserControl
    {
        public SelectProcessDialog()
        {
            InitializeComponent();
        }

        public Process GetSelected()
        {
            return ((ProcessItem) Selection.SelectedItem).Process;
        }
    }

    public class SelectProcessDialogViewModel : BaseClass
    {
        public ObservableCollection<ProcessItem> Root
        {
            get => GetValue<ObservableCollection<ProcessItem>>("Root");
            set => SetValue("Root", value);
        }

        public SelectProcessDialogViewModel()
        {
            Root = new ObservableCollection<ProcessItem>();

            List<Process> processes = new List<Process>(Process.GetProcesses());

            foreach (Process p in processes)
            {
                try
                {
                    if (p.MainWindowTitle != "" && p.MainModule.FileName != "")
                    {
                        Root.Add(new ProcessItem(p, this));
                    }
                }
                catch
                {
                }
            }

            ManagementEventWatcher startWatch =
                new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(OnProcessCreated);
            startWatch.Start();
        }

        private void OnProcessCreated(object sender, EventArrivedEventArgs e)
        {
            try
            {
                Process p = Process.GetProcessById(Convert.ToInt32((uint)e.NewEvent.Properties["ProcessID"].Value));
                //Wait until process is loaded
                Thread.Sleep(2000);
                if (p.MainWindowTitle != "" && p.MainModule.FileName != "")
                {
                    DispatchService.Invoke(() => { Root.Add(new ProcessItem(p, this)); });
                }
            }
            catch(Exception ex)
            {
#if (DEBUG)
                //MessageBox.Show(ex.Message);
#endif
            }
        }
    }

    public class ProcessItem : BaseClass
    {
        DispatcherTimer _timProcessWatcher;
        private SelectProcessDialogViewModel _baseModel;

        public Process Process
        {
            get => GetValue<Process>("Process");
            set => SetValue("Process", value);
        }

        public string Title
        {
            get => GetValue<string>("Title");
            set => SetValue("Title", value);
        }

        public BitmapSource Icon
        {
            get => GetValue<BitmapSource>("Icon");
            set => SetValue("Icon", value);
        }

        public ProcessItem(Process p, SelectProcessDialogViewModel baseModel)
        {
            Process    = p;
            _baseModel = baseModel;

            Icon i = System.Drawing.Icon.ExtractAssociatedIcon(p.MainModule.FileName);
            this.Icon = Imaging.CreateBitmapSourceFromHIcon(i.Handle, System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(i.Width, i.Height));
            this.Title = "Loading...";

            _timProcessWatcher = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(200)};
            _timProcessWatcher.Tick += Refresh;
            _timProcessWatcher.Start();
        }

        private void OnExit(object sender, EventArgs e)
        {
            _baseModel.Root.Remove(this);
            _timProcessWatcher.Stop();
        }

        private void Refresh(object sender, EventArgs e)
        {
            try
            {
                if (Process.HasExited)
                {
                    _baseModel.Root.Remove(this);
                    _timProcessWatcher.Stop();
                    return;
                }
                string title = Process.MainWindowTitle;
                if (title.Length > 47)
                {
                    title = title.Substring(0, 44) + "...";
                }

                this.Title = title;
            }
            catch
            {
            }
        }
    }
}

