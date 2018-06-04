using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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
            return ((ProcessItem)Selection.SelectedItem).Process;
        }
    }

    public class SelectProcessDialogViewModel : BaseClass
    {
        public ObservableCollection<ProcessItem> Root
        {
            get { return GetValue<ObservableCollection<ProcessItem>>("Root"); }
            set { SetValue("Root", value); }
        }

        DispatcherTimer _timProcessWatcher;

        public SelectProcessDialogViewModel()
        {
            Root = new ObservableCollection<ProcessItem>();

            _timProcessWatcher = new DispatcherTimer();
            _timProcessWatcher.Interval = TimeSpan.FromMilliseconds(2000);
            _timProcessWatcher.Tick += GetProcesses;
            _timProcessWatcher.Start();
            GetProcesses(null, new EventArgs());
        }

        private void GetProcesses(object sender, EventArgs e)
        {
            List<Process> processes = new List<Process>(Process.GetProcesses());

            foreach (Process p in processes)
            {
                if (p.MainWindowTitle == "")
                {
                    continue;
                }

                bool contains = false;
                foreach (ProcessItem i in Root)
                {
                    if (i.Process.Id == p.Id)
                    {
                        contains = true;
                    }
                }

                if (!contains)
                {
                    try
                    {
                        Root.Add(new ProcessItem(p));
                    }
                    catch { }
                }
            }

            List<ProcessItem> old = new List<ProcessItem>();

            foreach (ProcessItem p in Root)
            {
                try
                {
                    if (p.Process.HasExited)
                    {
                        old.Add(p);
                    }
                }
                catch { }
            }

            foreach (ProcessItem p in old)
            {
                Root.Remove(p);
            }
        }
    }


    public class ProcessItem : BaseClass
    {
        DispatcherTimer _timProcessWatcher;

        public Process Process
        {
            get { return GetValue<Process>("Process"); }
            set { SetValue("Process", value); }
        }

        public string Title
        {
            get { return GetValue<string>("Title"); }
            set { SetValue("Title", value); }
        }

        public BitmapSource Icon
        {
            get { return GetValue<BitmapSource>("Icon"); }
            set { SetValue("Icon", value); }
        }

        public ProcessItem(Process p)
        {
            Process = p;

            Icon i = System.Drawing.Icon.ExtractAssociatedIcon(p.MainModule.FileName);
            this.Icon = Imaging.CreateBitmapSourceFromHIcon(i.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(i.Width, i.Height));
            this.Title = "Loading...";

            _timProcessWatcher = new DispatcherTimer();
            _timProcessWatcher.Interval = TimeSpan.FromMilliseconds(1000 / 5);
            _timProcessWatcher.Tick += Refresh;
            _timProcessWatcher.Start();
        }

        private void Refresh(object sender, EventArgs e)
        {
            try
            {
                string title = Process.MainWindowTitle;
                if (title.Length > 47)
                {
                    title = title.Substring(0, 44) + "...";
                }
                this.Title = title;
            }
            catch { }
        }
    }
}
