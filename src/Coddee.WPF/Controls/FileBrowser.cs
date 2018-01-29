// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coddee.WPF.Commands;
using Microsoft.Win32;

namespace Coddee.WPF.Controls
{

    public class FileBrowser : Control
    {
        static FileBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileBrowser), new FrameworkPropertyMetadata(typeof(FileBrowser)));
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
                                                        "Title",
                                                        typeof(string),
                                                        typeof(FileBrowser),
                                                        new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TitleWidthProperty = DependencyProperty.Register(
                                                        "TitleWidth",
                                                        typeof(double),
                                                        typeof(FileBrowser),
                                                        new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
                                                        "FilePath",
                                                        typeof(string),
                                                        typeof(FileBrowser),
                                                        new FrameworkPropertyMetadata(default(string),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, FilePathPropertyChanged));


        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register(
                                                        "FileFilter",
                                                        typeof(string),
                                                        typeof(FileBrowser),
                                                        new PropertyMetadata(default(string)));

        public static readonly DependencyPropertyKey IsValidFilePropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsValidFile",
                                                                      typeof(bool),
                                                                      typeof(FileBrowser),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsValidFileProperty = IsValidFilePropertyKey.DependencyProperty;

        public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(
                                                        "InitialDirectory",
                                                        typeof(string),
                                                        typeof(FileBrowser),
                                                        new PropertyMetadata(default(string)));

        public static readonly DependencyPropertyKey FileNamePropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "FileName",
                                                                      typeof(string),
                                                                      typeof(FileBrowser),
                                                                      new PropertyMetadata(default(string)));

        public static readonly DependencyProperty FileNameProperty = FileNamePropertyKey.DependencyProperty;

        public string FileName
        {
            get { return (string) GetValue(FileNameProperty); }
            protected set { SetValue(FileNamePropertyKey, value); }
        }

        private static void FilePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileBrowser fileBrowser)
                fileBrowser.OnFilePathPropertyChanged((string)e.NewValue);
        }

        public FileBrowser()
        {
            BrowseCommand = new RelayCommand(Browse);
        }
        

        public ICommand BrowseCommand { get; }

        public string InitialDirectory
        {
            get { return (string)GetValue(InitialDirectoryProperty); }
            set { SetValue(InitialDirectoryProperty, value); }
        }

        public bool IsValidFile
        {
            get { return (bool)GetValue(IsValidFileProperty); }
            protected set { SetValue(IsValidFilePropertyKey, value); }
        }

        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public double TitleWidth
        {
            get { return (double)GetValue(TitleWidthProperty); }
            set { SetValue(TitleWidthProperty, value); }
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        private void OnFilePathPropertyChanged(string newValue)
        {
            if (File.Exists(newValue))
            {
                IsValidFile = true;
                FileName = Path.GetFileName(newValue);
            }
        }

        private void Browse()
        {
            var dlg = new OpenFileDialog
            {
                Filter = FileFilter,
                InitialDirectory = InitialDirectory
            };
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                FilePath = dlg.FileName;
            }
        }
    }
}
