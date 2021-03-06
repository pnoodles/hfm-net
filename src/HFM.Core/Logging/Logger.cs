﻿/*
 * HFM.NET - Core Logger
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Castle.Core.Logging;

namespace HFM.Core.Logging
{
   [CoverageExclude]
   public class Logger : LevelFilteredLogger
   {
      public Logger(string path)
         : base("Default")
      {
         try
         {
            Initialize(path);
         }
         catch (IOException ex)
         {
            string message = String.Format(CultureInfo.CurrentCulture,
               "Logging failed to initialize.  Please check to be sure that the {0} or {1} file is not open or otherwise in use.",
               Constants.HfmLogFileName, Constants.HfmPrevLogFileName);
            throw new InvalidOperationException(message, ex);
         }
         // default level
         Level = LoggerLevel.Info;
      }

      public override ILogger CreateChildLogger(string loggerName)
      {
         throw new NotImplementedException();
      }

      private static readonly object LogLock = new object();

      protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
      {
         if (loggerLevel <= Level)
         {
            lock (LogLock)
            {
               var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => FormatMessage(loggerLevel, x)).ToList();
               OnTextMessage(new TextMessageEventArgs(lines));
               foreach (var line in lines)
               {
                  Trace.WriteLine(line);
               }

               if (exception != null)
               {
                  Trace.WriteLine(FormatMessage(loggerLevel, exception.ToString()));
               }
            }
         }
      }

      private static string FormatMessage(LoggerLevel loggerLevel, string message)
      {
         string messageIdentifier = String.Empty;

         switch (loggerLevel)
         {
            case LoggerLevel.Off:
               messageIdentifier = " ";
               break;
            case LoggerLevel.Error:
            case LoggerLevel.Fatal:
               messageIdentifier = "X";
               break;
            case LoggerLevel.Warn:
               messageIdentifier = "!";
               break;
            case LoggerLevel.Info:
               messageIdentifier = "-";
               break;
            case LoggerLevel.Debug:
               messageIdentifier = "+";
               break;
         }

         DateTime dateTime = DateTime.Now;
         return String.Format("[{0}-{1}] {2} {3}", dateTime.ToShortDateString(), dateTime.ToLongTimeString(), messageIdentifier, message);
      }

      #region Text Message Event

      public event EventHandler<TextMessageEventArgs> TextMessage;

      private void OnTextMessage(TextMessageEventArgs e)
      {
         var handler = TextMessage;
         if (handler != null)
         {
            handler(this, e);
         }
      }

      #endregion

      private static void Initialize(string path)
      {
         // ensure the path exists
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }

         string logFilePath = Path.Combine(path, Constants.HfmLogFileName);
         string prevLogFilePath = Path.Combine(path, Constants.HfmPrevLogFileName);

         var fi = new FileInfo(logFilePath);
         if (fi.Exists && fi.Length > 512000)
         {
            var fi2 = new FileInfo(prevLogFilePath);
            if (fi2.Exists)
            {
               fi2.Delete();
            }
            fi.MoveTo(prevLogFilePath);
         }

         Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));
         Trace.AutoFlush = true;
      }
   }

   [CoverageExclude]
   public class TextMessageEventArgs : EventArgs
   {
      private readonly ICollection<string> _messages;

      public ICollection<string> Messages
      {
         get { return _messages; }
      }

      public TextMessageEventArgs(ICollection<string> messages)
      {
         _messages = messages;
      }
   }
}
