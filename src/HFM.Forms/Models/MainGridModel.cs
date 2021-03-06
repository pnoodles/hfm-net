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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

//using Castle.Core.Logging;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms.Models
{
   public sealed class MainGridModel
   {
      #region Events

      public event EventHandler BeforeResetBindings;
      public event EventHandler AfterResetBindings;
      public event EventHandler<IndexChangedEventArgs> SelectedSlotChanged;

      #endregion

      #region Properties

      //private ILogger _logger;
      //
      //public ILogger Logger
      //{
      //   [CoverageExclude]
      //   get { return _logger ?? (_logger = NullLogger.Instance); }
      //   [CoverageExclude]
      //   set { _logger = value; }
      //}

      private SlotModel _selectedSlot;
      public SlotModel SelectedSlot
      {
         get { return _selectedSlot; }
         set
         {
            if (!ReferenceEquals(_selectedSlot, value))
            {
               _selectedSlot = value;
               OnSelectedSlotChanged(new IndexChangedEventArgs(_bindingSource.Position));
            }
         }
      }

      /// <summary>
      /// Gets a value that specifies if the UI menu item 'View Client Files' is visible.
      /// </summary>
      public bool ClientFilesMenuItemVisible
      {
         get { return SelectedSlot != null && SelectedSlot.Settings.LegacyClientSubType == LegacyClientSubType.Path; }
      }

      /// <summary>
      /// Gets a value that specifies if the UI menu item 'View Cached Log File' is visible.
      /// </summary>
      public bool CachedLogMenuItemVisible
      {
         get
         {
            return SelectedSlot != null && (SelectedSlot.Settings.ClientType == ClientType.Legacy ||
                                            SelectedSlot.Settings.ClientType == ClientType.FahClient);
         }
      }

      /// <summary>
      /// Gets a value that specifies if the UI menu items 'xxx' are visible.
      /// </summary>
      public bool FahClientMenuItemsVisible
      {
         get { return SelectedSlot != null && SelectedSlot.Settings.ClientType == ClientType.FahClient; }
      }

      private string _sortColumnName;
      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      public string SortColumnName
      {
         get { return _sortColumnName; }
         set { _sortColumnName = String.IsNullOrEmpty(value) ? "Name" : value; }
      }

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      public ListSortDirection SortColumnOrder { get; set; }

      #endregion

      #region Fields

      private readonly IPreferenceSet _prefs;
      private readonly ISynchronizeInvoke _syncObject;
      private readonly IClientConfiguration _clientConfiguration;
      private readonly SlotModelSortableBindingList _slotList;
      private readonly BindingSource _bindingSource;

      private readonly object _slotsListLock = new object();

      #endregion

      public ICollection<SlotModel> SlotCollection
      {
         // ToList() to make a "copy" of the current list.
         // The value returned here is used by web generation
         // and if the collection changes the web generation
         // will not be able to enumerate the collection.
         get
         {
            lock (_slotsListLock)
            {
               return _slotList.ToList().AsReadOnly();
            }
         }
      }

      public SlotTotals SlotTotals
      {
         // use SlotCollection, it's provides synchronized access to the slot list
         get { return SlotCollection.GetSlotTotals(); }
      }

      public object BindingSource
      {
         get { return _bindingSource; }
      }

      public MainGridModel(IPreferenceSet prefs, ISynchronizeInvoke syncObject, IClientConfiguration clientConfiguration)
      {
         _prefs = prefs;
         _syncObject = syncObject;
         _clientConfiguration = clientConfiguration;
         _slotList = new SlotModelSortableBindingList(_syncObject);
         _slotList.OfflineClientsLast = _prefs.Get<bool>(Preference.OfflineLast);
         _slotList.Sorted += (sender, e) =>
                             {
                                SortColumnName = e.Name;
                                _prefs.Set(Preference.FormSortColumn, SortColumnName);
                                SortColumnOrder = e.Direction;
                                _prefs.Set(Preference.FormSortOrder, SortColumnOrder);
                             };
         _bindingSource = new BindingSource();
         _bindingSource.DataSource = _slotList;
         _bindingSource.CurrentItemChanged += (sender, args) => SelectedSlot = (SlotModel)_bindingSource.Current;
#if DEBUG
         _slotList.ListChanged += (s, e) => Debug.WriteLine("BindingList: " + e.ListChangedType);
         _bindingSource.ListChanged += (s, e) => Debug.WriteLine("BindingSource: " + e.ListChangedType);
#endif
         // Subscribe to PreferenceSet events
         _prefs.PreferenceChanged += (s, e) =>
                                     {
                                        switch (e.Preference)
                                        {
                                           case Preference.OfflineLast:
                                              _slotList.OfflineClientsLast = _prefs.Get<bool>(Preference.OfflineLast);
                                              Sort();
                                              break;
                                           case Preference.PpdCalculation:
                                           case Preference.DecimalPlaces:
                                           case Preference.BonusCalculation:
                                              ResetBindings();
                                              break;
                                        }

                                     };

         // Subscribe to ClientDictionary events
         _clientConfiguration.DictionaryChanged += (sender, args) => ResetBindings();
      }

      private readonly object _resetBindingsLock = new object();

      private void ResetBindings()
      {
         if (!Monitor.TryEnter(_resetBindingsLock))
         {
            Debug.WriteLine("Reset already in progress...");
            return;
         }
         try
         {
            ResetBindingsInternal();
         }
         finally
         {
            Monitor.Exit(_resetBindingsLock);
         }
      }

      private void ResetBindingsInternal()
      {
         var control = _syncObject as Control;
         if (control != null && control.IsDisposed)
         {
            return;
         }
         if (_syncObject.InvokeRequired)
         {
            _syncObject.Invoke(new MethodInvoker(ResetBindingsInternal), null);
            return;
         }

         OnBeforeResetBindings(EventArgs.Empty);
         lock (_slotsListLock)
         {
            // halt binding source updates
            _bindingSource.RaiseListChangedEvents = false;
            // see Revision 534 commit comments for the reason
            // _slotList.RaiseListChangedEvents = false is here.
            _slotList.RaiseListChangedEvents = false;
            // get slots from the dictionary
            var slots = _clientConfiguration.Slots as IList<SlotModel> ?? _clientConfiguration.Slots.ToList();
            // refresh the underlying binding list
            _bindingSource.Clear();
            foreach (var slot in slots)
            {
               _bindingSource.Add(slot);
            }
            Debug.WriteLine("Number of slots: {0}", _bindingSource.Count);
            // sort the list
            _bindingSource.Sort = null;
            _bindingSource.Sort = SortColumnName + " " + SortColumnOrder.ToDirectionString();
            // reset selected slot
            ResetSelectedSlot();
            // find duplicates
            slots.FindDuplicates();
            // enable binding source updates
            _bindingSource.RaiseListChangedEvents = true;
            // see Revision 534 commit comments for the reason
            // _slotList.RaiseListChangedEvents = false is here.
            _slotList.RaiseListChangedEvents = true;
            // reset AFTER RaiseListChangedEvents is enabled
            _bindingSource.ResetBindings(false);
         }
         OnAfterResetBindings(EventArgs.Empty);
      }

      /// <summary>
      /// Sort the grid model
      /// </summary>
      public void Sort()
      {
         lock (_slotsListLock)
         {
            _bindingSource.RaiseListChangedEvents = false;
            // see Revision 534 commit comments for the reason
            // _slotList.RaiseListChangedEvents = false is here.
            _slotList.RaiseListChangedEvents = false;
            // sort the list
            _bindingSource.Sort = null;
            _bindingSource.Sort = SortColumnName + " " + SortColumnOrder.ToDirectionString();
            // enable binding source updates
            _bindingSource.RaiseListChangedEvents = true;
            // see Revision 534 commit comments for the reason
            // _slotList.RaiseListChangedEvents = false is here.
            _slotList.RaiseListChangedEvents = true;
         }
      }

      public void ResetSelectedSlot()
      {
         if (SelectedSlot == null) return;

         int row = _bindingSource.Find("Name", SelectedSlot.Name);
         if (row > -1)
         {
            _bindingSource.Position = row;
         }
      }

      private void OnBeforeResetBindings(EventArgs e)
      {
         if (BeforeResetBindings != null)
         {
            BeforeResetBindings(this, e);
         }
      }

      private void OnAfterResetBindings(EventArgs e)
      {
         if (AfterResetBindings != null)
         {
            AfterResetBindings(this, e);
         }
      }

      private void OnSelectedSlotChanged(IndexChangedEventArgs e)
      {
         if (SelectedSlotChanged != null)
         {
            SelectedSlotChanged(this, e);
         }
      }
   }

   [CoverageExclude]
   public sealed class IndexChangedEventArgs : EventArgs
   {
      private readonly int _index;

      public int Index
      {
         get { return _index; }
      }

      public IndexChangedEventArgs(int index)
      {
         _index = index;
      }
   }
}
