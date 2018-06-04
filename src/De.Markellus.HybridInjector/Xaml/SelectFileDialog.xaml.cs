using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using De.Markellus.HybridInjector.Misc;

namespace De.Markellus.HybridInjector.Xaml
{
    /// <summary>
    /// Interaction logic for SelectFileDialog.xaml
    /// </summary>
    public partial class SelectFileDialog : UserControl
    {
        public FileSystemTemplate FileSystem { get; set; }

        public SelectFileDialog()
        {
            //Creates the File system template.
            FileSystem = new FileSystemTemplate(
                null, // Set the root folder to null to show ANY logical drive
                new List<string>()
                {
                    "*" + "dll"
                },
                new List<string>()
                {
                    "*" // Show ANY directory
                });

            InitializeComponent();
        }
    }

    public enum IOItemType : short
    {
        Folder,
        File
    }

    /// <summary>
    /// Contains a template source with full file system
    /// </summary>
    public class FileSystemTemplate : BaseClass
    {
        public ObservableCollection<IOItem> Root
        {
            get { return GetValue<ObservableCollection<IOItem>>("Root"); }
            set { SetValue("Root", value); }
        }

        public string SelectedPath
        {
            get { return GetValue<string>("SelectedPath"); }
            internal set { SetValue("SelectedPath", value); }
        }

        public FileSystemTemplate()
        {
            Root = new ObservableCollection<IOItem>();

            foreach (string drive in Directory.GetLogicalDrives())
                Root.Add(new IOItem(drive, IOItemType.Folder, null, new List<Wildcard>(), new List<Wildcard>(), this));
        }

        public FileSystemTemplate(string root = null, List<string> fileWildcard = null, List<string> dirWildcard = null)
        {
            Root = new ObservableCollection<IOItem>();
            List<Wildcard> _fileWildcard = new List<Wildcard>();
            List<Wildcard> _dirWildcard = new List<Wildcard>();

            if (fileWildcard != null)
                foreach (string wc in fileWildcard)
                    _fileWildcard.Add(new Wildcard(wc));
            if (dirWildcard != null)
                foreach (string wc in dirWildcard)
                    _dirWildcard.Add(new Wildcard(wc));

            if (root == null)
                foreach (string drive in Directory.GetLogicalDrives())
                    Root.Add(new IOItem(drive, IOItemType.Folder, null, _fileWildcard, _dirWildcard, this));
            else
            {
                foreach (string dir in Directory.GetDirectories(root))
                    Root.Add(new IOItem(dir, IOItemType.Folder, null, _fileWildcard, _dirWildcard, this));
                foreach (string file in Directory.GetFiles(root))
                    Root.Add(new IOItem(file, IOItemType.Folder, null, _fileWildcard, _dirWildcard, this));
            }
        }

        public IOItem AddItem(FileSystemInfo pathInfo, IOItemType type)
        {
            string[] segments = pathInfo.FullName.Split(Path.DirectorySeparatorChar);

            ObservableCollection<IOItem> currentItemList = Root;
            IOItem lastItem = null;

            //enumerate through all items in the list
            for (int i = 0; i < segments.Length - 1; i++)
            {
                foreach (IOItem item in currentItemList)
                {
                    string name = item.DisplayName.Trim('\\');
                    if (item.DisplayName.Trim('\\') == segments[i])
                    {
                        //item found, set currentItemList one path higher
                        currentItemList = item.Children;
                        lastItem = item;
                    }
                }
            }
            //reload the children (refresh TreeView UI)
            lastItem.SetChildren();
            //return the new IOItem
            return lastItem.Children.Single(i => i.DisplayName == segments[segments.Length - 1]);
        }

    }

    /// <summary>
    /// Displays an Item with folder or file icon
    /// </summary>
    public class IOItem : BaseClass
    {
        private static class NativeMethods
        {
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFileInfo psfi, uint cbFileInfo, uint uFlags);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DestroyIcon(IntPtr hIcon);

            public const uint SHGFI_ICON = 0x000000100;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            public const uint SHGFI_OPENICON = 0x000000002;
            public const uint SHGFI_SMALLICON = 0x000000001;
            public const uint SHGFI_LARGEICON = 0x000000000;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            public const uint FILE_ATTRIBUTE_FILE = 0x00000100;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFileInfo
        {
            public IntPtr hIcon;

            public int iIcon;

            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        private enum IconState : short
        {
            Undefined,
            Open,
            Close
        }

        private enum IconSize : short
        {
            Small,
            Large
        }

        private List<Wildcard> fileWildcard;
        private List<Wildcard> dirWildcard;
        private FileSystemTemplate _template;

        public FileSystemInfo ItemInfo { get; private set; }

        public string DisplayName
        {
            get { return ItemInfo.Name; }
        }

        public string FullPath
        {
            get { return ItemInfo.FullName; }
        }

        public BitmapSource Icon { get; private set; }

        public IOItemType Type { get; private set; }

        public bool CanHaveChildren
        {
            get
            {
                if (Type == IOItemType.Folder)
                    return true;
                else
                    return false;
            }
        }

        public bool IsFile
        {
            get
            {
                if (Type == IOItemType.File)
                    return true;
                else
                    return false;
            }
        }

        public bool IsSelected
        {
            get { return GetValue<bool>("IsSelected"); }
            set
            {
                SetValue("IsSelected", value);
                _template.SelectedPath = this.FullPath;
            }
        }

        public bool IsExpanded
        {
            get { return GetValue<bool>("IsExpanded"); }
            set { SetValue("IsExpanded", value); }
        }

        public IOItem Parent { get; private set; }

        public ObservableCollection<IOItem> Children
        {
            get
            {
                if (GetValue<ObservableCollection<IOItem>>("Children") == null)
                    SetChildren();
                return GetValue<ObservableCollection<IOItem>>("Children");
            }
            private set { SetValue("Children", value); }
        }

        internal IOItem(string path, IOItemType type, IOItem parent, List<Wildcard> fileWildcard, List<Wildcard> dirWildcard, FileSystemTemplate template)
        {
            switch (type)
            {
                case IOItemType.Folder:
                    ItemInfo = new DirectoryInfo(path);
                    break;
                case IOItemType.File:
                    ItemInfo = new FileInfo(path);
                    break;
            }

            Icon i = GetIcon(path, type);
            this.Icon = Imaging.CreateBitmapSourceFromHIcon(i.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(i.Width, i.Height));
            this.Type = type;
            this.Parent = parent;
            this.fileWildcard = fileWildcard;
            this.dirWildcard = dirWildcard;
            _template = template;
        }

        public void SetChildren()
        {
            if (ItemInfo is DirectoryInfo)
            {
                Children = new ObservableCollection<IOItem>();

                try
                {
                    foreach (string dir in Directory.GetDirectories(FullPath))
                        foreach (Wildcard wc in dirWildcard)
                            if (wc.IsMatch(dir))
                                Children.Add(new IOItem(dir, IOItemType.Folder, this, fileWildcard, dirWildcard, _template));
                    foreach (string file in Directory.GetFiles(FullPath))
                        foreach (Wildcard wc in fileWildcard)
                            if (wc.IsMatch(file))
                                Children.Add(new IOItem(file, IOItemType.File, this, fileWildcard, dirWildcard, _template));
                }
                catch { }
            }
        }

        public static Icon GetIcon(string path, IOItemType type)
        {
            return GetIcon(path, type, IconSize.Large, IconState.Undefined);
        }

        private static Icon GetIcon(string path, IOItemType type, IconSize size, IconState state)
        {
            var flags = (uint)(NativeMethods.SHGFI_ICON | NativeMethods.SHGFI_USEFILEATTRIBUTES);
            var attribute = (uint)(object.Equals(type, IOItemType.Folder) ? NativeMethods.FILE_ATTRIBUTE_DIRECTORY : NativeMethods.FILE_ATTRIBUTE_FILE);

            if (object.Equals(type, IOItemType.Folder) && object.Equals(state, IconState.Open))
            {
                flags += NativeMethods.SHGFI_OPENICON;
            }
            if (object.Equals(size, IconSize.Small))
            {
                flags += NativeMethods.SHGFI_SMALLICON;
            }
            else
            {
                flags += NativeMethods.SHGFI_LARGEICON;
            }
            var shfi = new SHFileInfo();
            var res = NativeMethods.SHGetFileInfo(path, attribute, out shfi, (uint)Marshal.SizeOf(shfi), flags);

            if (object.Equals(res, IntPtr.Zero)) throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            try
            {
                System.Drawing.Icon.FromHandle(shfi.hIcon);
                return (Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            }
            catch
            {
                throw;
            }
            finally
            {
                NativeMethods.DestroyIcon(shfi.hIcon);
            }
        }
    }

}
