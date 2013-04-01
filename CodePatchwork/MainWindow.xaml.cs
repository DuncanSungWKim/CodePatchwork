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

using SharpSvn;
using SharpSvn.UI;


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
            Repos.GetXDocument();
 
            m_reopsView.DataContext = new { Repos = m_repos };
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


        private CommitDataGridCtrlr m_commitDataGridCtrlr = new CommitDataGridCtrlr();
        private Repos m_repos = new Repos() ;
    }
}
