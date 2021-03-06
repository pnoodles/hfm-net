﻿/*
 * HFM.NET - Log Line Parser Class
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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
using System.Globalization;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   internal sealed class LogLineParser : LogLineParserBase
   {
      #region Regex (Static)

      private static readonly Regex LogOpenRegex = new Regex(@"\*+ Log Started (?<StartTime>.+) \*+");

      // a copy of this regex exists in HFM.Core.DataTypes
      private static readonly Regex WorkUnitRunningRegex =
         new Regex("(?<Timestamp>.{8}):WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      // a copy of this regex exists in HFM.Core.DataTypes
      private static readonly Regex WorkUnitRunningRegex38 =
         new Regex("(?<Timestamp>.{8}):Unit (?<UnitIndex>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitWorkingRegex =
         new Regex("(?<Timestamp>.{8}):WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):Starting", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitWorkingRegex38 =
         new Regex("(?<Timestamp>.{8}):Starting Unit (?<UnitIndex>\\d{2})", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitCoreReturnRegex =
         new Regex("(?<Timestamp>.{8}):WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):FahCore returned: (?<UnitResult>.*) \\(.*\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitCoreReturnRegex38 =
         new Regex("(?<Timestamp>.{8}):FahCore, running Unit (?<UnitIndex>\\d{2}), returned: (?<UnitResult>.*) \\(.*\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitCleanUpRegex =
         new Regex("(?<Timestamp>.{8}):WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):Cleaning up", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitCleanUpRegex38 =
         new Regex("(?<Timestamp>.{8}):Cleaning up Unit (?<UnitIndex>\\d{2})", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      #endregion

      #region Methods
      
      internal override object GetLineData(LogLine logLine)
      {
         var data = base.GetLineData(logLine);
         if (data != null)
         {
            return data;
         }

         switch (logLine.LineType)
         {
            case LogLineType.LogOpen:
               Match logOpenMatch;
               if ((logOpenMatch = LogOpenRegex.Match(logLine.LineRaw)).Success)
               {
                  string startTime = logOpenMatch.Result("${StartTime}");
                  // Similar code found in HFM.Client.Converters.DateTimeConverter
                  // ISO 8601
                  DateTime value;
                  if (DateTime.TryParse(startTime, CultureInfo.InvariantCulture,
                      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out value))
                  {
                     return value;
                  }

                  // custom format for older v7 clients
                  if (DateTime.TryParseExact(startTime, "dd/MMM/yyyy-HH:mm:ss", CultureInfo.InvariantCulture,
                      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out value))
                  {
                     return value;
                  }
               }
               return new LogLineError(String.Format("Failed to parse Log Open value from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitWorking:
               Match workUnitWorkingMatch;
               if ((workUnitWorkingMatch = WorkUnitWorkingRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitWorkingMatch.Result("${UnitIndex}"));
               }
               if ((workUnitWorkingMatch = WorkUnitWorkingRegex38.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitWorkingMatch.Result("${UnitIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitRunning:
               Match workUnitRunningMatch;
               if ((workUnitRunningMatch = WorkUnitRunningRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitRunningMatch.Result("${UnitIndex}"));
               }
               if ((workUnitRunningMatch = WorkUnitRunningRegex38.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitRunningMatch.Result("${UnitIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCoreReturn:
               Match coreReturnMatch;
               if ((coreReturnMatch = WorkUnitCoreReturnRegex.Match(logLine.LineRaw)).Success)
               {
                  return new UnitResult
                         {
                            Index = Int32.Parse(coreReturnMatch.Result("${UnitIndex}")),
                            Value = coreReturnMatch.Result("${UnitResult}").ToWorkUnitResult()
                         };
               }
               if ((coreReturnMatch = WorkUnitCoreReturnRegex38.Match(logLine.LineRaw)).Success)
               {
                  return new UnitResult
                         {
                            Index = Int32.Parse(coreReturnMatch.Result("${UnitIndex}")),
                            Value = coreReturnMatch.Result("${UnitResult}").ToWorkUnitResult()
                         };
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Result value from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCleaningUp:
               Match workUnitCleanUpMatch;
               if ((workUnitCleanUpMatch = WorkUnitCleanUpRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitCleanUpMatch.Result("${UnitIndex}"));
               }
               if ((workUnitCleanUpMatch = WorkUnitCleanUpRegex38.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitCleanUpMatch.Result("${UnitIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
         }

         return null;
      }

      #endregion
   }

   internal sealed class UnitResult
   {
      public int Index { get; set; }

      public WorkUnitResult Value { get; set; }
   }
}
