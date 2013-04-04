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

                m_client.Log( Path, logArgs, 
                    new EventHandler<SvnLogEventArgs>(m_commitDataConsumer.OnEachLog) );
            }
            catch (Exception e)
            {
            }
        }


        public void CreatePatches(List<long> a_commits)
        {
            if (a_commits.Count <= 0)
                return;

            List<string> patchFiles = new List<string>();
            SvnDiffArgs args = new SvnDiffArgs();
            string tmpFolder = System.IO.Path.GetTempPath();

            foreach( long commit in a_commits )
            {
                SvnRevision oldRev = new SvnRevision(commit - 1);
                SvnRevision rev = new SvnRevision(commit);
                SvnRevisionRange revRg = new SvnRevisionRange(oldRev, rev);

                string tmpFile = System.IO.Path.Combine(tmpFolder, commit.ToString()+".diff");
                using( FileStream stm = new FileStream(tmpFile, FileMode.Create, FileAccess.Write) )
                {
                    m_client.Diff(Path, revRg, args, stm);
                }
                patchFiles.Add(tmpFile);
            }
        }


    #region Constants
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
