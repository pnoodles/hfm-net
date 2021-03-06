﻿/*
 * HFM.NET - Legacy Data Aggregator Class Tests
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
using System.IO;
using System.Linq;

using AutoMapper;

using NUnit.Framework;

using HFM.Core.Configuration;
using HFM.Core.DataTypes;
using HFM.Log;
using HFM.Queue;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class LegacyDataAggregatorTests
   {
      // ReSharper disable InconsistentNaming
      private const string queue = "queue.dat";
      private const string FAHlog = "FAHlog.txt";
      private const string unitinfo = "unitinfo.txt";
      // ReSharper restore InconsistentNaming

      private LegacyDataAggregator _dataAggregator;

      [SetUp]
      public void Init()
      {
         _dataAggregator = new LegacyDataAggregator();
         // create maps
         Mapper.Initialize(c => c.AddProfile<AutoMapperProfile>());
      }

      // ReSharper disable InconsistentNaming

      [Test]
      public void SMP_3()
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_3";
         _dataAggregator.ClientName = "SMP_3";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 8, 25, 18, 11, 37), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.02", result.ClientVersion);
         Assert.IsNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(13, 18, 28), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.08f, unitInfoData.CoreVersion);
         Assert.AreEqual(2677, unitInfoData.ProjectID);
         Assert.AreEqual(14, unitInfoData.ProjectRun);
         Assert.AreEqual(69, unitInfoData.ProjectClone);
         Assert.AreEqual(39, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(1, unitInfoData.FramesObserved);
         Assert.AreEqual(0, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(13, 18, 49), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(TimeSpan.Zero, unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_7()
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_7";
         _dataAggregator.ClientName = "SMP_7";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNotNull(result.UnitInfos[2]);
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNotNull(result.UnitInfos[6]);
         Assert.IsNotNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNotNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 9, 7, 23, 11, 31), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.24beta", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[2].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.IsNotNull(result.UnitInfos[6].LogLines);
         Assert.IsNotNull(result.UnitInfos[7].LogLines);
         Assert.IsNotNull(result.UnitInfos[8].LogLines);
         Assert.IsNotNull(result.UnitInfos[9].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2009, 10, 3, 7, 52, 7), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 10, 6, 7, 52, 7), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(7, 52, 7), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.10f, unitInfoData.CoreVersion);
         Assert.AreEqual(2669, unitInfoData.ProjectID);
         Assert.AreEqual(13, unitInfoData.ProjectRun);
         Assert.AreEqual(159, unitInfoData.ProjectClone);
         Assert.AreEqual(153, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P2669R13C159G153", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(80000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(31, unitInfoData.FramesObserved);
         Assert.AreEqual(32, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(15, 4, 43), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 13, 8), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("A2", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_8_1()
      {
         /*** The Test below shows us that because there is no Project information available
          *   in the FAHlog for the current WU, the UnitLogLines for the Current Queue Index
          *   cannot be matched against the Current Queue Entry.  As of 1/31/10 the Queue Entries
          *   are left in tact and the CurrentWorkUnitLogLines are force parsed to match the
          *   Current Queue Index, so now we do know the the Project (R/C/G).
          ***/

         const string path = "..\\..\\..\\TestFiles\\SMP_8";
         _dataAggregator.ClientName = "SMP_8_1";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, queueData, null);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNotNull(result.UnitInfos[2]);
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNull(result.UnitInfos[4]);
         Assert.IsNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);
         Assert.IsNull(result.UnitInfos[7]);
         Assert.IsNull(result.UnitInfos[8]);
         Assert.IsNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(3, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 11, 20, 6, 6, 41), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.24R3", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[2].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2009, 11, 24, 21, 53, 46), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 11, 30, 21, 53, 46), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(21, 53, 46), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.10f, unitInfoData.CoreVersion);
         Assert.AreEqual(2683, unitInfoData.ProjectID);
         Assert.AreEqual(2, unitInfoData.ProjectRun);
         Assert.AreEqual(8, unitInfoData.ProjectClone);
         Assert.AreEqual(24, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual("P2683R2C8G24", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(0, unitInfoData.RawFramesTotal);
         Assert.AreEqual(0, unitInfoData.FramesObserved);
         Assert.AreEqual(null, unitInfoData.CurrentFrame);
         Assert.AreEqual("A2", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_8_2()
      {
         /*** The Test below now gives us access to the unitinfo.txt file
          *   but not the queue.dat.  This will allow us to still parse the
          *   logs but read the Project (R/C/G) from the unitinfo.txt file instead.
          ***/

         const string path = "..\\..\\..\\TestFiles\\SMP_8";
         _dataAggregator.ClientName = "SMP_8_2";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, null, unitInfo);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 11, 20, 6, 6, 41), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.24R3", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 11, 24, 21, 53, 46), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 11, 30, 21, 53, 46), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(21, 53, 46), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.10f, unitInfoData.CoreVersion);
         Assert.AreEqual(2683, unitInfoData.ProjectID);
         Assert.AreEqual(2, unitInfoData.ProjectRun);
         Assert.AreEqual(8, unitInfoData.ProjectClone);
         Assert.AreEqual(24, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P2683R2C8G24", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(0, unitInfoData.RawFramesTotal);
         Assert.AreEqual(0, unitInfoData.FramesObserved);
         Assert.AreEqual(null, unitInfoData.CurrentFrame);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_9()
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_9";
         _dataAggregator.ClientName = "SMP_9";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNull(result.UnitInfos[1]);
         Assert.IsNull(result.UnitInfos[2]);
         Assert.IsNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);
         Assert.IsNull(result.UnitInfos[7]);
         Assert.IsNull(result.UnitInfos[8]);
         Assert.IsNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(5, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 1, 19, 8, 43, 24), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.24R3", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("coccola", unitInfoData.FoldingID);
         Assert.AreEqual(86565, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 1, 22, 4, 37, 17), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 1, 28, 4, 37, 17), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(4, 37, 17), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.10f, unitInfoData.CoreVersion);
         Assert.AreEqual(2681, unitInfoData.ProjectID);
         Assert.AreEqual(9, unitInfoData.ProjectRun);
         Assert.AreEqual(8, unitInfoData.ProjectClone);
         Assert.AreEqual(55, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P2681R9C8G55", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(127500, unitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(50, unitInfoData.FramesObserved);
         Assert.AreEqual(51, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(7, 20, 44), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 59, 12), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("A2", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_11() // mangled Project string on current WU
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_11";
         _dataAggregator.ClientName = "SMP_11";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNull(result.UnitInfos[2]);     // not current WU, mangled project string
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNotNull(result.UnitInfos[6]);
         Assert.IsNotNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNotNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(5, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 12, 24, 13, 21, 20), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.24R3", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.IsNotNull(result.UnitInfos[6].LogLines);
         Assert.IsNotNull(result.UnitInfos[7].LogLines);
         Assert.IsNotNull(result.UnitInfos[8].LogLines);
         Assert.IsNotNull(result.UnitInfos[9].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("Felix_Pasqualli", unitInfoData.FoldingID);
         Assert.AreEqual(52523, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2009, 12, 29, 9, 7, 48), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 1, 1, 9, 7, 48), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(9, 7, 48), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.10f, unitInfoData.CoreVersion);
         Assert.AreEqual(2671, unitInfoData.ProjectID);
         Assert.AreEqual(30, unitInfoData.ProjectRun);
         Assert.AreEqual(81, unitInfoData.ProjectClone);
         Assert.AreEqual(165, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P2671R30C81G165", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(150000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(59, unitInfoData.FramesObserved);
         Assert.AreEqual(60, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(14, 33, 19), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 5, 22), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("A2", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_12() // CoreOutdated on last Queue Index 6
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_12";
         _dataAggregator.ClientName = "SMP_12";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNotNull(result.UnitInfos[2]);
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);     // not current WU, CORE_OUTDATED message
         Assert.IsNotNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNotNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(7, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 1, 28, 6, 7, 54), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.29", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[2].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.IsNotNull(result.UnitInfos[7].LogLines);
         Assert.IsNotNull(result.UnitInfos[8].LogLines);
         Assert.IsNotNull(result.UnitInfos[9].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 3, 24, 7, 21, 46), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 3, 30, 7, 21, 46), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(7, 21, 51), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.17f, unitInfoData.CoreVersion);
         Assert.AreEqual(6024, unitInfoData.ProjectID);
         Assert.AreEqual(0, unitInfoData.ProjectRun);
         Assert.AreEqual(9, unitInfoData.ProjectClone);
         Assert.AreEqual(78, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P6024R0C9G78", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(485000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(500000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(98, unitInfoData.FramesObserved);
         Assert.AreEqual(97, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(18, 8, 4), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 6, 38), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("A3", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_13() // ReadyForUpload queue status now populates the FinishedTime property
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_13";
         _dataAggregator.ClientName = "SMP_13";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNull(result.UnitInfos[1]);
         Assert.IsNull(result.UnitInfos[2]);
         Assert.IsNull(result.UnitInfos[3]);
         Assert.IsNull(result.UnitInfos[4]);
         Assert.IsNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);
         Assert.IsNull(result.UnitInfos[7]);
         Assert.IsNull(result.UnitInfos[8]);
         Assert.IsNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(0, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 5, 28, 17, 19, 4), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.29", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.SendingWorkPacket, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 5, 27, 12, 4, 26), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 6, 2, 12, 4, 26), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(17, 19, 05), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(new DateTime(2010, 5, 28, 21, 34, 4), unitInfoData.FinishedTime); // is populated
         Assert.AreEqual(2.19f, unitInfoData.CoreVersion);
         Assert.AreEqual(6073, unitInfoData.ProjectID);
         Assert.AreEqual(0, unitInfoData.ProjectRun);
         Assert.AreEqual(10, unitInfoData.ProjectClone);
         Assert.AreEqual(53, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P6073R0C10G53", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitInfoData.UnitResult);
         Assert.AreEqual(500000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(500000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(55, unitInfoData.FramesObserved);
         Assert.AreEqual(100, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(21, 33, 46), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 4, 32), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("A3", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_14() // GettingWorkPacket - queue entry available before any log data
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_14";
         _dataAggregator.ClientName = "SMP_14";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, queueData, null);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNull(result.UnitInfos[2]);
         Assert.IsNull(result.UnitInfos[3]);
         Assert.IsNull(result.UnitInfos[4]);
         Assert.IsNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);
         Assert.IsNull(result.UnitInfos[7]);
         Assert.IsNull(result.UnitInfos[8]);
         Assert.IsNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 11, 7, 9, 21, 55), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.24R3", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.GettingWorkPacket, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, fahLog.ClientRuns.First());

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2009, 11, 8, 13, 57, 45), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 11, 12, 13, 57, 45), unitInfoData.DueTime);
         Assert.AreEqual(TimeSpan.MinValue, unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(0, unitInfoData.CoreVersion);
         Assert.AreEqual(2653, unitInfoData.ProjectID);
         Assert.AreEqual(28, unitInfoData.ProjectRun);
         Assert.AreEqual(194, unitInfoData.ProjectClone);
         Assert.AreEqual(125, unitInfoData.ProjectGen);
         Assert.AreEqual(null, unitInfoData.ProteinName);
         Assert.AreEqual("P2653R28C194G125", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(0, unitInfoData.RawFramesTotal);
         Assert.AreEqual(0, unitInfoData.FramesObserved);
         // no CurrentFrame has been set
         //Assert.AreEqual(0, unitInfoData.CurrentFrame.FrameID);
         //Assert.AreEqual(TimeSpan.MinValue, unitInfoData.CurrentFrame.TimeOfFrame);
         //Assert.AreEqual(TimeSpan.Zero, unitInfoData.CurrentFrame.FrameDuration);
         Assert.IsNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("A1", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_15() // lots of Client-core communications error
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_15";
         _dataAggregator.ClientName = "SMP_15";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNotNull(result.UnitInfos[2]);
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNotNull(result.UnitInfos[6]);
         Assert.IsNotNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNotNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(0, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 9, 14, 2, 48, 27), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.30", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.EuePause, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[2].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.IsNotNull(result.UnitInfos[6].LogLines);
         Assert.IsNotNull(result.UnitInfos[7].LogLines);
         Assert.IsNotNull(result.UnitInfos[8].LogLines);
         Assert.IsNotNull(result.UnitInfos[9].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 10, 14, 9, 18, 26), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 10, 20, 9, 18, 26), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(9, 18, 31), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.22f, unitInfoData.CoreVersion);
         Assert.AreEqual(6071, unitInfoData.ProjectID);
         Assert.AreEqual(0, unitInfoData.ProjectRun);
         Assert.AreEqual(39, unitInfoData.ProjectClone);
         Assert.AreEqual(70, unitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", unitInfoData.ProteinName);
         Assert.AreEqual("P6071R0C39G70", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.ClientCoreError, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(500000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(1, unitInfoData.FramesObserved);
         Assert.AreEqual(0, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(9, 18, 38), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(TimeSpan.Zero, unitInfoData.CurrentFrame.FrameDuration);
         Assert.IsNotNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("A3", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void SMP_17() // v6.23 A4 SMP
      {
         const string path = "..\\..\\..\\TestFiles\\SMP_17";
         _dataAggregator.ClientName = "SMP_17";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 3, 20, 7, 52, 34), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.34", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(2, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("GreyWhiskers", unitInfoData.FoldingID);
         Assert.AreEqual(0, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(8, 46, 43), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.27f, unitInfoData.CoreVersion);
         Assert.AreEqual(8022, unitInfoData.ProjectID);
         Assert.AreEqual(11, unitInfoData.ProjectRun);
         Assert.AreEqual(318, unitInfoData.ProjectClone);
         Assert.AreEqual(25, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(15000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(7, unitInfoData.FramesObserved);
         Assert.AreEqual(6, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(8, 52, 30), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 0, 56), unitInfoData.CurrentFrame.FrameDuration);
         Assert.IsNotNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void GPU2_3()
      {
         const string path = "..\\..\\..\\TestFiles\\GPU2_3";
         _dataAggregator.ClientName = "GPU2_3";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 8, 18, 3, 54, 16), result.StartTime);
         Assert.IsNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(2, result.MachineID);
         Assert.AreEqual(SlotStatus.EuePause, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("JollySwagman", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(5, 59, 23), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(1.19f, unitInfoData.CoreVersion);
         Assert.AreEqual(5756, unitInfoData.ProjectID);
         Assert.AreEqual(6, unitInfoData.ProjectRun);
         Assert.AreEqual(32, unitInfoData.ProjectClone);
         Assert.AreEqual(480, unitInfoData.ProjectGen);
         Assert.AreEqual("", unitInfoData.ProteinName);
         Assert.AreEqual("", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.UnstableMachine, unitInfoData.UnitResult);
         Assert.AreEqual(4, unitInfoData.RawFramesComplete);
         Assert.AreEqual(100, unitInfoData.RawFramesTotal);
         Assert.AreEqual(4, unitInfoData.FramesObserved);
         Assert.AreEqual(4, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(6, 7, 44), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 1, 34), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void GPU2_6_1()
      {
         const string path = "..\\..\\..\\TestFiles\\GPU2_6";
         _dataAggregator.ClientName = "GPU2_6_1";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNotNull(result.UnitInfos[2]);
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNotNull(result.UnitInfos[6]);
         Assert.IsNotNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNotNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(8, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 11, 10, 13, 31, 6), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(3, result.MachineID);
         Assert.AreEqual(SlotStatus.GettingWorkPacket, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[2].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.IsNotNull(result.UnitInfos[6].LogLines);
         Assert.IsNotNull(result.UnitInfos[7].LogLines);
         Assert.IsNotNull(result.UnitInfos[8].LogLines);
         Assert.IsNotNull(result.UnitInfos[9].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2009, 11, 26, 1, 30, 40), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 11, 29, 1, 30, 40), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(1, 30, 40), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(new DateTime(2009, 11, 26, 2, 32, 20), unitInfoData.FinishedTime);
         Assert.AreEqual(1.19f, unitInfoData.CoreVersion);
         Assert.AreEqual(5770, unitInfoData.ProjectID);
         Assert.AreEqual(4, unitInfoData.ProjectRun);
         Assert.AreEqual(242, unitInfoData.ProjectClone);
         Assert.AreEqual(1366, unitInfoData.ProjectGen);
         Assert.AreEqual("Protein", unitInfoData.ProteinName);
         Assert.AreEqual("P5770R4C242G1366", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitInfoData.UnitResult);
         Assert.AreEqual(100, unitInfoData.RawFramesComplete);
         Assert.AreEqual(100, unitInfoData.RawFramesTotal);
         Assert.AreEqual(100, unitInfoData.FramesObserved);
         Assert.AreEqual(100, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(2, 32, 5), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 0, 38), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("11", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void GPU2_7()
      {
         const string path = "..\\..\\..\\TestFiles\\GPU2_7";
         _dataAggregator.ClientName = "GPU2_7";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 1, 31, 1, 57, 21), result.StartTime);
         Assert.IsNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(2, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("Zagen30", unitInfoData.FoldingID);
         Assert.AreEqual(46301, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(1, 57, 21), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(1.31f, unitInfoData.CoreVersion);
         Assert.AreEqual(5781, unitInfoData.ProjectID);
         Assert.AreEqual(2, unitInfoData.ProjectRun);
         Assert.AreEqual(700, unitInfoData.ProjectClone);
         Assert.AreEqual(2, unitInfoData.ProjectGen);
         Assert.AreEqual("", unitInfoData.ProteinName);
         Assert.AreEqual("", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(5, unitInfoData.RawFramesComplete);
         Assert.AreEqual(100, unitInfoData.RawFramesTotal);
         Assert.AreEqual(5, unitInfoData.FramesObserved);
         Assert.AreEqual(5, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(2, 4, 2), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 1, 19), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void GPU2_8_1()
      {
         const string path = "..\\..\\..\\TestFiles\\GPU2_8";
         _dataAggregator.ClientName = "GPU2_8_1";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, queueData, null);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNull(result.UnitInfos[1]);
         Assert.IsNull(result.UnitInfos[2]);
         Assert.IsNull(result.UnitInfos[3]);
         Assert.IsNull(result.UnitInfos[4]);
         Assert.IsNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);
         Assert.IsNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(8, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 4, 29, 4, 55, 24), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(2, result.MachineID);
         Assert.AreEqual(SlotStatus.Stopped, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNull(result.UnitInfos[8].LogLines);
         Assert.AreEqual(result.CurrentLogLines, fahLog.ClientRuns.First());

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 4, 28, 5, 5, 43), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 5, 7, 5, 5, 43), unitInfoData.DueTime);
         Assert.AreEqual(TimeSpan.MinValue, unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(new DateTime(2010, 4, 28, 6, 17, 25), unitInfoData.FinishedTime);
         Assert.AreEqual(0, unitInfoData.CoreVersion);
         Assert.AreEqual(6603, unitInfoData.ProjectID);
         Assert.AreEqual(10, unitInfoData.ProjectRun);
         Assert.AreEqual(601, unitInfoData.ProjectClone);
         Assert.AreEqual(62, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual("P6603R10C601G62", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(0, unitInfoData.RawFramesTotal);
         Assert.AreEqual(0, unitInfoData.FramesObserved);
         Assert.IsNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("11", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void GPU2_8_2()
      {
         const string path = "..\\..\\..\\TestFiles\\GPU2_8";
         _dataAggregator.ClientName = "GPU2_8_2";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 4, 29, 4, 55, 24), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(2, result.MachineID);
         Assert.AreEqual(SlotStatus.Stopped, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, fahLog.ClientRuns.First());

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(TimeSpan.MinValue, unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(0, unitInfoData.CoreVersion);
         Assert.AreEqual(0, unitInfoData.ProjectID);
         Assert.AreEqual(0, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(0, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.RawFramesComplete);
         Assert.AreEqual(0, unitInfoData.RawFramesTotal);
         Assert.AreEqual(0, unitInfoData.FramesObserved);
         Assert.IsNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void GPU3_1()
      {
         const string path = "..\\..\\..\\TestFiles\\GPU3_1";
         _dataAggregator.ClientName = "GPU3_1";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);
         Assert.IsNotNull(result.UnitInfos[2]);
         Assert.IsNotNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNotNull(result.UnitInfos[5]);
         Assert.IsNotNull(result.UnitInfos[6]);
         Assert.IsNotNull(result.UnitInfos[7]);
         Assert.IsNotNull(result.UnitInfos[8]);
         Assert.IsNotNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(8, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 11, 2, 9, 24, 59), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.30r2", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(2, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.IsNotNull(result.UnitInfos[2].LogLines);
         Assert.IsNotNull(result.UnitInfos[3].LogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.IsNotNull(result.UnitInfos[5].LogLines);
         Assert.IsNotNull(result.UnitInfos[6].LogLines);
         Assert.IsNotNull(result.UnitInfos[7].LogLines);
         Assert.IsNotNull(result.UnitInfos[8].LogLines);
         Assert.IsNotNull(result.UnitInfos[9].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex - 1];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 11, 19, 3, 45, 56), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 11, 29, 3, 45, 56), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(3, 45, 56), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(new DateTime(2010, 11, 19, 5, 56, 35), unitInfoData.FinishedTime);
         Assert.AreEqual(2.14f, unitInfoData.CoreVersion);
         Assert.AreEqual(6800, unitInfoData.ProjectID);
         Assert.AreEqual(595, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(32, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitInfoData.UnitResult);
         Assert.AreEqual(49999999, unitInfoData.RawFramesComplete);
         Assert.AreEqual(50000000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(100, unitInfoData.FramesObserved);
         Assert.IsNotNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("15", unitInfoData.CoreID);
         #endregion

         unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 11, 19, 5, 56, 56), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 11, 29, 5, 56, 56), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(5, 56, 56), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.14f, unitInfoData.CoreVersion);
         Assert.AreEqual(6800, unitInfoData.ProjectID);
         Assert.AreEqual(595, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(33, unitInfoData.ProjectGen);
         Assert.AreEqual("PEPTIDE (1-42)", unitInfoData.ProteinName);
         Assert.AreEqual("-", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(11500000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(50000000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(23, unitInfoData.FramesObserved);
         Assert.IsNotNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("15", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Standard_5() // multiple Project strings before WU start
      {
         const string path = "..\\..\\..\\TestFiles\\Standard_5";
         _dataAggregator.ClientName = "Standard_5";
         var queueData = QueueReader.ReadQueue(Path.Combine(path, queue));
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);
         var unitInfo = UnitInfoLog.Read(Path.Combine(path, unitinfo));

         var result = _dataAggregator.AggregateData(fahLog, queueData, unitInfo);
         Assert.AreEqual(10, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNull(result.UnitInfos[1]);
         Assert.IsNull(result.UnitInfos[2]);
         Assert.IsNull(result.UnitInfos[3]);
         Assert.IsNotNull(result.UnitInfos[4]);
         Assert.IsNull(result.UnitInfos[5]);
         Assert.IsNull(result.UnitInfos[6]);
         Assert.IsNull(result.UnitInfos[7]);
         Assert.IsNull(result.UnitInfos[8]);
         Assert.IsNull(result.UnitInfos[9]);

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(4, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 3, 24, 0, 41, 7), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(3, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[4].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("borden.b", unitInfoData.FoldingID);
         Assert.AreEqual(131, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2010, 3, 23, 22, 41, 07), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 4, 15, 23, 38, 42), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(0, 41, 07), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(23, unitInfoData.CoreVersion);
         Assert.AreEqual(10002, unitInfoData.ProjectID);
         Assert.AreEqual(19, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(51, unitInfoData.ProjectGen);
         Assert.AreEqual("ProtoMol p10002", unitInfoData.ProteinName);
         Assert.AreEqual("-", unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(110000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(1000000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(5, unitInfoData.FramesObserved);
         Assert.AreEqual(11, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(1, 30, 50), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 10, 10), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("B4", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Standard_7() // new ProtoMol - progress not on percent boundry
      {
         const string path = "..\\..\\..\\TestFiles\\Standard_7";
         _dataAggregator.ClientName = "Standard_7";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 4, 8, 23, 31, 9), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(4, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("NerdZone", unitInfoData.FoldingID);
         Assert.AreEqual(155945, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(23, 36, 17), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(23, unitInfoData.CoreVersion);
         Assert.AreEqual(10015, unitInfoData.ProjectID);
         Assert.AreEqual(3609, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(0, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(164800, unitInfoData.RawFramesComplete);
         Assert.AreEqual(499375, unitInfoData.RawFramesTotal);
         Assert.AreEqual(34, unitInfoData.FramesObserved);
         Assert.AreEqual(33, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(4, 58, 09), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 15, 05), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Standard_8() // Battery Pause w/SendingWorkPacket during pause
      {
         const string path = "..\\..\\..\\TestFiles\\Standard_8";
         _dataAggregator.ClientName = "Standard_8";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNotNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 3, 31, 13, 33, 32), result.StartTime);
         Assert.IsNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(1, result.MachineID);
         Assert.AreEqual(SlotStatus.RunningNoFrameTimes, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[0].LogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("DrSpalding", unitInfoData.FoldingID);
         Assert.AreEqual(48083, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(01, 50, 48), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(1.03f, unitInfoData.CoreVersion);
         Assert.AreEqual(4606, unitInfoData.ProjectID);
         Assert.AreEqual(26, unitInfoData.ProjectRun);
         Assert.AreEqual(185, unitInfoData.ProjectClone);
         Assert.AreEqual(6, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(175000, unitInfoData.RawFramesComplete);
         Assert.AreEqual(2500000, unitInfoData.RawFramesTotal);
         Assert.AreEqual(5, unitInfoData.FramesObserved);
         Assert.AreEqual(7, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(2, 5, 30), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(0, 3, 38), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Standard_9() // v6.23 A4 Uniprocessor
      {
         const string path = "..\\..\\..\\TestFiles\\Standard_9";
         _dataAggregator.ClientName = "Standard_9";
         var fahLog = FahLog.Read(File.ReadLines(Path.Combine(path, FAHlog)), FahLogType.Legacy);

         var result = _dataAggregator.AggregateData(fahLog, null, null);
         Assert.AreEqual(2, result.UnitInfos.Count);
         Assert.IsNull(result.UnitInfos[0]);
         Assert.IsNotNull(result.UnitInfos[1]);

         #region Check Data Aggregator

         Assert.IsNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(DateTime.UtcNow.Year, 3, 16, 18, 46, 15), result.StartTime);
         Assert.IsNotNull(result.Arguments);
         Assert.AreEqual("6.23", result.ClientVersion);
         Assert.IsNotNull(result.UserID);
         Assert.AreEqual(14, result.MachineID);
         Assert.AreEqual(SlotStatus.Stopped, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsNotNull(result.UnitInfos[1].LogLines);
         Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("Amaruk", unitInfoData.FoldingID);
         Assert.AreEqual(50625, unitInfoData.Team);
         Assert.AreEqual(SlotType.Unknown, unitInfoData.SlotType);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(18, 46, 15), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.27f, unitInfoData.CoreVersion);
         Assert.AreEqual(10741, unitInfoData.ProjectID);
         Assert.AreEqual(0, unitInfoData.ProjectRun);
         Assert.AreEqual(1996, unitInfoData.ProjectClone);
         Assert.AreEqual(3, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitInfoData.UnitResult);
         Assert.AreEqual(7000001, unitInfoData.RawFramesComplete);
         Assert.AreEqual(7000001, unitInfoData.RawFramesTotal);
         Assert.AreEqual(94, unitInfoData.FramesObserved);
         Assert.AreEqual(100, unitInfoData.CurrentFrame.FrameID);
         Assert.AreEqual(new TimeSpan(5, 37, 54), unitInfoData.CurrentFrame.TimeOfFrame);
         Assert.AreEqual(new TimeSpan(1, 40, 36), unitInfoData.CurrentFrame.FrameDuration);
         Assert.AreEqual("Unknown", unitInfoData.CoreID);
         #endregion
      }

      // ReSharper restore InconsistentNaming
   }
}
