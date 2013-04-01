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
using System.Collections.ObjectModel;
using System.IO;
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

            xElem.Add(
                new XElement("Repo",
                    new XElement("Name", a_newRepo.Name),
                    new XElement("Path", a_newRepo.Path)
                    )
                );

            xDoc.Save( GetFilePath() );

            base.Add( a_newRepo ) ;
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


    #region Constants
        private const string REPOS_XML_FILENAME = "Repos.xml"; 
    #endregion
    }
}
