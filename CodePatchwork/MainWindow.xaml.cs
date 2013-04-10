/*
    Copyright (C) 2013 Duncan Sung W. Kim
	
    This file is part of Code Patchwork.

    Code Patchwork is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Code Patchwork is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Code Patchwork.  If not, see <http://www.gnu.org/licenses/>.
	
    If you want to contact the author, you can use github.com's Issues page
    at <https://github.com/DuncanSungWKim/CodePatchwork/issues>
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;


using SharpSvn;
using SharpSvn.UI;
using Ionic.Zip;


namespace CodePatchwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
                                    , System.Windows.Forms.IWin32Window
    {
        public MainWindow()
        {
            InitializeComponent();

            m_commitDataGridCtrlr.DataGrid = m_commitDataGrid;

            InitReposView();
        }


        private void InitReposView()
        {
            m_repos.Load( this, m_commitDataGridCtrlr );
 
            m_reopsView.DataContext = m_repos ;
        }


        public IntPtr Handle
        {
            get
            {
                var interopHelper = new System.Windows.Interop.WindowInteropHelper(this);
                return interopHelper.Handle;
            }
        }


        private void Menu_AddNewRepo(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if( System.Windows.Forms.DialogResult.OK != dlg.ShowDialog() )
                return ;

            Repo repo = new Repo(this) {
                Name = dlg.SelectedPath,
                Path = dlg.SelectedPath,
                CommitDataConsumer = m_commitDataGridCtrlr
            };

            m_repos.Add( repo );
        }


        private void Menu_CreatePatches(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            string PatchSaveFolder = Properties.Settings.Default.PatchSaveFolder;
            dlg.SelectedPath = PatchSaveFolder;
            if ( System.Windows.Forms.DialogResult.OK != dlg.ShowDialog() )
                return;

            if ( 0 != String.Compare(PatchSaveFolder, dlg.SelectedPath, 
                                     StringComparison.InvariantCultureIgnoreCase) )
            {
                Properties.Settings.Default.PatchSaveFolder = dlg.SelectedPath;
                Properties.Settings.Default.Save();
            }


            WaitMessagebox waitMsgbox = new WaitMessagebox(this);

            CreatePackage( dlg.SelectedPath );

            waitMsgbox.Close();
        }


        private void ReopsView_KeyUp(object sender, System.Windows.Input.KeyEventArgs a_e)
        {
            if (Key.Delete == a_e.Key)
            {
                 object val =  m_reopsView.SelectedValue;
                 m_repos.Remove(val as Repo);
            }
        }


        private void CreatePackage( string a_selectedPath )
        {
            object selected = m_reopsView.SelectedValue;
            Repo repo = selected as Repo;
            if( null != repo )
            {
                string pkgName = App.NAME + App.CreateDateTimeSuffix();
                string tmpFolder = System.IO.Path.GetTempPath();
                tmpFolder = System.IO.Path.Combine(tmpFolder, pkgName);
                if (Directory.Exists(tmpFolder))
                    Directory.Delete(tmpFolder,true);
                Directory.CreateDirectory(tmpFolder);

                var commits = m_commitDataGridCtrlr.GetCheckedCommits();
                repo.CreatePatches(commits, tmpFolder);

                HashSet<string> pathsForCopy = m_commitDataGridCtrlr.RecordCommits( tmpFolder, repo.GetPathFromRoot() );
                repo.CopyFiles(pathsForCopy, tmpFolder);

                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(tmpFolder);
                    string zipFilePath = System.IO.Path.Combine(a_selectedPath, pkgName + ".zip");
                    zip.Save(zipFilePath);
                }
            }
        }


        private void Menu_OpenPatchPackage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            string PatchOpenFolder = Properties.Settings.Default.PatchOpenFolder;
            dlg.InitialDirectory = PatchOpenFolder;
            dlg.Filter = "Patch Package|*.zip";
            if ( true != dlg.ShowDialog() )
                return;

            string folderPath = Path.GetDirectoryName(dlg.FileName);
            if ( 0 != String.Compare( PatchOpenFolder, folderPath,
                                      StringComparison.InvariantCultureIgnoreCase) )
            {
                Properties.Settings.Default.PatchOpenFolder = folderPath;
                Properties.Settings.Default.Save();
            }

            string zipFileName = Path.GetFileNameWithoutExtension(dlg.FileName);
            string extractFolder = System.IO.Path.GetTempPath();
            extractFolder = Path.Combine(extractFolder, zipFileName);
            if (Directory.Exists(extractFolder))
                Directory.Delete(extractFolder, true);
            Directory.CreateDirectory(extractFolder);

            using ( ZipFile zipFile = ZipFile.Read(dlg.FileName) )
            {
                foreach( ZipEntry ze in zipFile )
                {
                    ze.Extract(extractFolder, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }


        private CommitDataGridCtrlr m_commitDataGridCtrlr = new CommitDataGridCtrlr();
        private Repos m_repos = new Repos() ;
    }
}
