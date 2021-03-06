﻿/*
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
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

using SharpSvn;
using SharpSvn.UI;


namespace CodePatchwork
{
    class Repo
        : INotifyPropertyChanged
    {
        public Repo(System.Windows.Forms.IWin32Window a_win)
        {
            SvnUI.Bind(m_client, a_win);
        }


        public string Name
        { get; set; }


        public string Path
        { get; set; }


        private CommitDataConsumer m_commitDataConsumer;
        public CommitDataConsumer CommitDataConsumer {
            set {
                m_commitDataConsumer = value;
            }
        }

        private bool m_selected ;
        public bool IsSelected
        {
            get { return m_selected; }

            set
            {
                if (value != m_selected)
                {
                    m_selected = value;
                    NotifyPropertyChanged("IsSelected");
                }

                if ( ! value)
                    return;

                Directory.SetCurrentDirectory(Path);

                LoadLog();
            }
        }


        private void LoadLog()
        {
            m_commitDataConsumer.Clear();

            try
            {
                SvnLogArgs logArgs = new SvnLogArgs() ;
                logArgs.Start = SvnRevision.Head ;
                logArgs.Limit = FETCH_COUNT;

                m_client.Log( ".", logArgs, 
                    new EventHandler<SvnLogEventArgs>(m_commitDataConsumer.OnEachLog) );
            }
            catch (Exception e)
            {
            }
        }


        public void CreatePatches(List<long> a_commits, string a_destFolder)
        {
            if (a_commits.Count <= 0)
                return;

            Directory.SetCurrentDirectory(Path);

            List<string> patchFiles = new List<string>();
            SvnDiffArgs args = new SvnDiffArgs();

            foreach( long commit in a_commits )
            {
                SvnRevision oldRev = new SvnRevision(commit - 1);
                SvnRevision rev = new SvnRevision(commit);
                SvnRevisionRange revRg = new SvnRevisionRange(oldRev, rev);

                string tmpFile = System.IO.Path.Combine(a_destFolder, commit.ToString() + ".diff");
                using( FileStream stm = new FileStream(tmpFile, FileMode.Create, FileAccess.Write) )
                {
                    m_client.Diff(".", revRg, args, stm);
                }
                patchFiles.Add(tmpFile);
            }


        }


        public string GetPathFromRoot()
        {
            SvnInfoEventArgs info;
            m_client.GetInfo(Path, out info);
            string s = info.Uri.ToString().Remove( 0, info.RepositoryRoot.ToString().Length-1 );
            return s ;
        }


        public void CopyFiles(HashSet<string> a_paths, string a_destFolderPath)
        {
            string rootFolder = System.IO.Path.Combine(a_destFolderPath, FILES_FOLDER);
            Directory.CreateDirectory(rootFolder);

            foreach (string path in a_paths)
            {
                string path2 = path.Replace("/", @"\");
                if (!File.Exists(path2))
                    continue;
                App.CopyFileToFolder(path2, rootFolder);
            }
        }

    #region Constants
        private const string FILES_FOLDER = "FILES";
        private const int FETCH_COUNT = 30;
    #endregion


        private SvnClient m_client = new SvnClient();


    #region Candidates for the prospective base class

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    #endregion
    }
}
