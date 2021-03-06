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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CodePatchwork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string GetDataFolderPath()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appDataFolder, App.NAME);
            if ( ! Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }


        internal static string CreateDateTimeSuffix()
        {
            DateTime dt = DateTime.Now.ToLocalTime();
            return String.Format("{0:yyyy-MM-ddTHHmmsszz}", dt);
        }


        internal static void CopyFileToFolder( string a_sourFilePath, string a_destFolder )
        {
            List<string> steps = a_sourFilePath.Split( new char[] {'\\', '/'} ).ToList();

            if (steps.Count <= 0)
                return;

            int iLast = steps.Count - 1;
            string fileName = steps[iLast];
            steps.RemoveAt(iLast);

            string stepPath = a_destFolder;
            foreach( string step in steps )
            {
                stepPath = Path.Combine(stepPath, step);
                if ( ! Directory.Exists(stepPath))
                    Directory.CreateDirectory(stepPath);
            }

            string filePath = Path.Combine(stepPath, fileName);
            File.Copy(a_sourFilePath, filePath);
        }


        #region Constants
        public const string NAME = "CodePatchwork" ;
        #endregion
    }
}
