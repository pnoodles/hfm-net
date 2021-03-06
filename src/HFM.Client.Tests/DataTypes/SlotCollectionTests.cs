﻿/*
 * HFM.NET - Slot Collection Data Class Tests
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

using NUnit.Framework;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests.DataTypes
{
   [TestFixture]
   public class SlotCollectionTests
   {
      [Test]
      public void FillTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, slotCollection.Count);
         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("smp:4", slotCollection[0].Description);
         Assert.AreEqual(true, slotCollection[0].SlotOptions.PauseOnStart);
      }

      [Test]
      public void FillTest2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_2\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, slotCollection.Count);
         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("uniprocessor", slotCollection[0].Description);
      }

      [Test]
      public void FillTest3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_3\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, slotCollection.Count);
         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("smp:8", slotCollection[0].Description);
      }

      [Test]
      public void FillTest4()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(3, slotCollection.Count);

         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("smp:4", slotCollection[0].Description);
         Assert.AreEqual("beta", slotCollection[0].SlotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotCollection[0].SlotOptions.FahClientTypeEnum);
         Assert.AreEqual("big", slotCollection[0].SlotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotCollection[0].SlotOptions.MaxPacketSizeEnum);
         Assert.AreEqual(100, slotCollection[0].SlotOptions.NextUnitPercentage);

         Assert.AreEqual(1, slotCollection[1].Id);
         Assert.AreEqual("RUNNING", slotCollection[1].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[1].StatusEnum);
         Assert.AreEqual("gpu:0:\"GT200 [GeForce GTX 260]\"", slotCollection[1].Description);
         Assert.AreEqual("beta", slotCollection[1].SlotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotCollection[1].SlotOptions.FahClientTypeEnum);
         Assert.AreEqual("low", slotCollection[1].SlotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Low, slotCollection[1].SlotOptions.CorePriorityEnum);

         Assert.AreEqual(2, slotCollection[2].Id);
         Assert.AreEqual("RUNNING", slotCollection[2].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[2].StatusEnum);
         Assert.AreEqual("gpu:1:\"G92 [GeForce 9800 GTX]\"", slotCollection[2].Description);
         Assert.AreEqual("beta", slotCollection[2].SlotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotCollection[2].SlotOptions.FahClientTypeEnum);
         Assert.AreEqual("low", slotCollection[2].SlotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Low, slotCollection[2].SlotOptions.CorePriorityEnum);
      }

      [Test]
      public void FillTest5()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(2, slotCollection.Count);

         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("smp:4", slotCollection[0].Description);

         Assert.AreEqual(1, slotCollection[1].Id);
         Assert.AreEqual("RUNNING", slotCollection[1].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[1].StatusEnum);
         Assert.AreEqual("gpu:0:\"Radeon HD 5870 (Cypress)\"", slotCollection[1].Description);
      }

      [Test]
      public void FillTest6()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_6\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, slotCollection.Count);
         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("uniprocessor", slotCollection[0].Description);
      }

      [Test]
      public void FillTest7()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(2, slotCollection.Count);

         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("FINISHING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Finishing, slotCollection[0].StatusEnum);
         Assert.AreEqual("smp:4", slotCollection[0].Description);
         Assert.AreEqual("beta", slotCollection[0].SlotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotCollection[0].SlotOptions.FahClientTypeEnum);
         Assert.AreEqual("big", slotCollection[0].SlotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotCollection[0].SlotOptions.MaxPacketSizeEnum);
         Assert.AreEqual(100, slotCollection[0].SlotOptions.NextUnitPercentage);

         Assert.AreEqual(1, slotCollection[1].Id);
         Assert.AreEqual("RUNNING", slotCollection[1].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[1].StatusEnum);
         Assert.AreEqual("gpu:0:\"GF106 [GeForce 450 GTS]\"", slotCollection[1].Description);
      }

      [Test]
      public void FillTest10()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(2, slotCollection.Count);

         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[0].StatusEnum);
         Assert.AreEqual("smp:4", slotCollection[0].Description);
         Assert.AreEqual(true, slotCollection[0].SlotOptions.PauseOnStart);

         Assert.AreEqual(1, slotCollection[1].Id);
         Assert.AreEqual("RUNNING", slotCollection[1].Status);
         Assert.AreEqual(FahSlotStatus.Running, slotCollection[1].StatusEnum);
         Assert.AreEqual("gpu:0:\"GT200b [GeForce GTX 285]\"", slotCollection[1].Description);
         Assert.AreEqual(true, slotCollection[0].SlotOptions.PauseOnStart);
      }

      [Test]
      public void FillTest12()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_12\\slots.txt");
         var slotCollection = new SlotCollection();
         slotCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, slotCollection.Count);

         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("PAUSED", slotCollection[0].Status);
         Assert.AreEqual(FahSlotStatus.Paused, slotCollection[0].StatusEnum);
         Assert.AreEqual("cpu:4", slotCollection[0].Description);
         Assert.AreEqual(-1, slotCollection[0].SlotOptions.Cpus);
         Assert.AreEqual(true, slotCollection[0].SlotOptions.PauseOnStart);
         Assert.AreEqual("paused", slotCollection[0].Reason);
      }
   }
}
