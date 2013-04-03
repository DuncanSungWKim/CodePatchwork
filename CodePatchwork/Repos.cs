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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;


namespace CodePatchwork
{
    class Repos
        : ObservableCollection<Repo>
    {
        new public void Add( Repo a_newRepo )
        {
            XDocument xDoc = GetXDocument();
            XElement xElem = xDoc.Element("Repos");

            // Check the same path.
            var repos = SameRepos(xDoc, a_newRepo.Path);
            if (repos.Count() > 0)
                return;


            xElem.Add(
                new XElement("Repo",
                    new XElement("Name", a_newRepo.Name),
                    new XElement("Path", a_newRepo.Path)
                    )
                );

            xDoc.Save( GetFilePath() );

            base.Add( a_newRepo ) ;
        }


        new public void Remove(Repo a_repo)
        {
            if (null == a_repo)
                return;

            XDocument xDoc = GetXDocument();

            XElement[] repos = SameRepos(xDoc, a_repo.Path).ToArray<XElement>();
            foreach( XElement xElem in repos )
            {
                xElem.Remove() ;
            }

            xDoc.Save( GetFilePath() );

            base.Remove(a_repo);
        }


        public void Load( System.Windows.Forms.IWin32Window a_win, CommitDataConsumer a_commitDataConsumer )
        {
            XDocument xDoc = GetXDocument();

            var repos = from r in xDoc.Descendants("Repo")
                        select new Repo(a_win) {
                            Name = r.Element("Name").Value,
                            Path = r.Element("Path").Value,
                            CommitDataConsumer = a_commitDataConsumer
                        };

            foreach( Repo repo in repos )
            {
                base.Add(repo) ;
            }
        }


        public static XDocument GetXDocument()
        {
            string xmlFilePath = GetFilePath();

            XDocument xDoc;

            if ( ! File.Exists(xmlFilePath))
            {
                xDoc = new XDocument( new XElement("Repos") );
                xDoc.Save(xmlFilePath);
                return xDoc;
            }

            try
            {
                xDoc = XDocument.Load(xmlFilePath);
            }
            catch (Exception e)
            {
                throw;
            }
 
            return xDoc;
        }


        private static string GetFilePath()
        {
            string appDataFolder = App.GetDataFolderPath();
            return Path.Combine(appDataFolder, REPOS_XML_FILENAME);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<XElement> SameRepos(XDocument a_xDoc, string a_path)
        {
            return from r in a_xDoc.Descendants("Repo")
                   where r.Element("Path").Value == a_path
                   select r;
        }


    #region Constants
        private const string REPOS_XML_FILENAME = "Repos.xml"; 
    #endregion
    }
}
