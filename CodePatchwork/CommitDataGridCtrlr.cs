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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml.Linq;
using System.IO;


using SharpSvn;


namespace CodePatchwork
{
    class CommitDataGridCtrlr : CommitDataConsumer
    {
        DataGrid m_dataGrid ;
        public DataGrid DataGrid {
            set {
                m_dataGrid = value;
                value.DataContext = m_commits;
            }
        }


        private class CommitEntry
        {
            public long Commit { get; set; }
            public string Author { get; set; }
            public string Message { get; set; }
            public DateTime Time { get; set; }

            private bool m_checked ;
            public bool IsChecked {
                get {
                    return m_checked;
                }
                set {
                    m_checked = value;
                }
            }
        }


        public void Clear()
        {
            m_commits.Clear();
        }


        public void OnEachLog(object a_sender, SvnLogEventArgs a_log)
        {
            CommitEntry c = new CommitEntry() ;
            
            c.Commit = a_log.Revision ;
            c.Author = a_log.Author ;
            c.Message = a_log.LogMessage;
            c.Time = a_log.Time;

            m_commits.Add( c );
        }


        public List<long> GetCheckedCommits()
        {
            List<long> commits = new List<long>() ;
            foreach (CommitEntry c in m_commits)
            {
                if (c.IsChecked)
                    commits.Add(c.Commit);
            }

            return commits;            
        }


        public void RecordCommits( string a_destFolderPath )
        {
            XElement xElCommits = new XElement("Commits");
            XDocument xDoc = new XDocument(xElCommits);

            foreach( CommitEntry c in m_commits )
            {
                XElement xCommit = new XElement( "Commit",
                    new XAttribute( "id", c.Commit ),
                    new XElement( "Author", c.Author ),
                    new XElement( "Message", c.Message ),
                    new XElement( "Time", c.Time )
                        );
                xElCommits.Add(xCommit);
            }

            string xmlFilePath = Path.Combine( a_destFolderPath, "Log.xml" );
            xDoc.Save(xmlFilePath);
        }


        private ObservableCollection<CommitEntry> m_commits = new ObservableCollection<CommitEntry>();
    }
}
