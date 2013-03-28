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

using SharpSvn;
using SharpSvn.UI;


namespace CodePatchwork
{
    class Repo
    {
        public Repo(System.Windows.Forms.IWin32Window a_win)
        {
            SvnUI.Bind(m_client, a_win);
        }


        public string Name
        { get; set; }


        public string Uri
        { get; set; }


        public void OnEachLog(object a_sender, SvnLogEventArgs a_log)
        {
            long iRev = a_log.Revision;
            string author = a_log.Author;
            string msg = a_log.LogMessage;
        }

        private bool m_selected ;
        public bool IsSelected
        {
            get { return m_selected; }

            set
            {
                m_selected = true;
                if ( ! value)
                    return;

                LoadLog();
            }
        }


        private void LoadLog()
        {
            try
            {
                m_client.Log(new Uri(this.Uri), new EventHandler<SvnLogEventArgs>(OnEachLog));
            }
            catch (Exception e)
            {
            }
        }


        private SvnClient m_client = new SvnClient();
    }
}
