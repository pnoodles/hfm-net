﻿/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Castle.Core.Logging;

namespace HFM.Core
{
   public interface IWebsiteDeployer
   {
      void DeployWebsite(IEnumerable<string> htmlFilePaths, IEnumerable<string> xmlFilePaths, IEnumerable<SlotModel> slots);
   }

   public class WebsiteDeployer : IWebsiteDeployer
   {
      private ILogger _logger;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger ?? (_logger = NullLogger.Instance); }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly IPreferenceSet _prefs;
      private readonly INetworkOps _networkOps;
      
      public WebsiteDeployer(IPreferenceSet prefs, INetworkOps networkOps)
      {
         _prefs = prefs;
         _networkOps = networkOps;
      }
   
      public void DeployWebsite(IEnumerable<string> htmlFilePaths, IEnumerable<string> xmlFilePaths, IEnumerable<SlotModel> slots)
      {
         Debug.Assert(_prefs.Get<bool>(Preference.WebGenerationTaskEnabled));

         var webGenType = _prefs.Get<WebDeploymentType>(Preference.WebDeploymentType);
         var webRoot = _prefs.Get<string>(Preference.WebDeploymentRoot);
         var copyHtml = _prefs.Get<bool>(Preference.WebGenCopyHtml);
         var copyXml = _prefs.Get<bool>(Preference.WebGenCopyXml);
         
         if (webGenType.Equals(WebDeploymentType.Ftp))
         {
            var server = _prefs.Get<string>(Preference.WebGenServer);
            var port = _prefs.Get<int>(Preference.WebGenPort);
            var username = _prefs.Get<string>(Preference.WebGenUsername);
            var password = _prefs.Get<string>(Preference.WebGenPassword);

            if (copyHtml && htmlFilePaths != null)
            {
               FtpHtmlUpload(server, port, webRoot, username, password, htmlFilePaths, slots);
            }
            if (copyXml && xmlFilePaths != null)
            {
               FtpXmlUpload(server, port, webRoot, username, password, xmlFilePaths);
            }
         }
         else // WebGenType.Path
         {
            // Create the web folder (just in case)
            if (!Directory.Exists(webRoot))
            {
               Directory.CreateDirectory(webRoot);
            }

            if (copyHtml && htmlFilePaths != null)
            {
               var cssFile = _prefs.Get<string>(Preference.CssFile);
               // Copy the CSS file to the output directory
               string cssFilePath = Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName), cssFile);
               if (File.Exists(cssFilePath))
               {
                  File.Copy(cssFilePath, Path.Combine(webRoot, cssFile), true);
               }

               foreach (string filePath in htmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }

               if (_prefs.Get<bool>(Preference.WebGenCopyFAHlog))
               {
                  foreach (var slot in slots)
                  {
                     string cachedFahlogPath = Path.Combine(_prefs.CacheDirectory, slot.Settings.CachedFahLogFileName());
                     if (File.Exists(cachedFahlogPath))
                     {
                        File.Copy(cachedFahlogPath, Path.Combine(webRoot, slot.Settings.CachedFahLogFileName()), true);
                     }
                  }
               }
            }
            if (copyXml && xmlFilePaths != null)
            {
               foreach (string filePath in xmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }
            }
         }
      }

      private void FtpHtmlUpload(string server, int port, string ftpPath, string username, string password, IEnumerable<string> htmlFilePaths, IEnumerable<SlotModel> slots)
      {
         var sw = Stopwatch.StartNew();

         try
         {
            // Get the FTP Type
            var ftpMode = _prefs.Get<FtpMode>(Preference.WebGenFtpMode);

            // Upload CSS File
            _networkOps.FtpUploadHelper(server, port, ftpPath, Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName),
               _prefs.Get<string>(Preference.CssFile)), username, password, ftpMode);

            // Upload each HTML File
            foreach (string filePath in htmlFilePaths)
            {
               _networkOps.FtpUploadHelper(server, port, ftpPath, filePath, username, password, ftpMode);
            }

            if (_prefs.Get<bool>(Preference.WebGenCopyFAHlog))
            {
               int maximumLength = _prefs.Get<bool>(Preference.WebGenLimitLogSize)
                                    ? _prefs.Get<int>(Preference.WebGenLimitLogSizeLength) * 1024
                                    : -1;

               var logPaths = slots.Select(x => Path.Combine(_prefs.CacheDirectory, x.Settings.CachedFahLogFileName())).Distinct();
               foreach (var cachedFahlogPath in logPaths)
               {
                  if (File.Exists(cachedFahlogPath))
                  {
                     using (var stream = FileSystem.TryFileOpen(cachedFahlogPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                     {
                        if (stream == null)
                        {
                           Logger.WarnFormat("Could not open {0} for FTP upload.", cachedFahlogPath);
                           continue;
                        }
                        _networkOps.FtpUploadHelper(server, port, ftpPath, Path.GetFileName(cachedFahlogPath), stream, maximumLength, username, password, ftpMode);
                     }
                  }
               }
            }
         }
         finally
         {
            Logger.InfoFormat("HTML upload finished in {0}", sw.GetExecTime());
         }
      }

      private void FtpXmlUpload(string server, int port, string ftpPath, string username, string password, IEnumerable<string> xmlFilePaths)
      {
         var sw = Stopwatch.StartNew();

         try
         {
            // Get the FTP Type
            var ftpMode = _prefs.Get<FtpMode>(Preference.WebGenFtpMode);

            // Upload each XML File
            foreach (string filePath in xmlFilePaths)
            {
               _networkOps.FtpUploadHelper(server, port, ftpPath, filePath, username, password, ftpMode);
            }
         }
         finally
         {
            Logger.InfoFormat("XML upload finished in {0}", sw.GetExecTime());
         }
      }
   }
}
