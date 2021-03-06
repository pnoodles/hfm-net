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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IClientFactory
   {
      IEnumerable<IClient> CreateCollection(IEnumerable<ClientSettings> settingsCollection);

      IClient Create(ClientSettings settings);

      void Release(IClient client);
   }

   public class ClientFactory : IClientFactory
   {
      private ILogger _logger;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger ?? (_logger = NullLogger.Instance); }
         [CoverageExclude]
         set { _logger = value; }
      }

      public IFahClientFactory FahClientFactory { get; set; }

      public ILegacyClientFactory LegacyClientFactory { get; set; }

      public IEnumerable<IClient> CreateCollection(IEnumerable<ClientSettings> settingsCollection)
      {
         if (settingsCollection == null) throw new ArgumentNullException("settingsCollection");

         return settingsCollection.Select(CreateWrapper).Where(client => client != null).ToList().AsReadOnly();
      }

      private IClient CreateWrapper(ClientSettings settings)
      {
         try
         {
            return Create(settings);
         }
         catch (ArgumentException ex)
         {
            Logger.ErrorFormat(ex, "{0}", ex.Message);
            return null;
         }
      }
      
      public IClient Create(ClientSettings settings)
      {
         if (settings == null) throw new ArgumentNullException("settings");
         
         if (String.IsNullOrEmpty(settings.Name))
         {
            throw new ArgumentException("Failed to create client.  No name given.");
         }

         if (settings.IsFahClient() && String.IsNullOrEmpty(settings.Server))
         {
            throw new ArgumentException("Failed to create client.  No server given.");
         }
         
         if (settings.IsLegacy() && String.IsNullOrEmpty(settings.Path))
         {
            throw new ArgumentException("Failed to create client.  No path given.");
         }
      
         string preCleanName = settings.Name;
         ICollection<string> cleanupWarnings = CleanupSettings(settings);
         if (!Validate.ClientName(settings.Name))
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "Failed to create client.  Client name '{0}' is not valid after cleaning.", preCleanName));
         }
         
         if (cleanupWarnings.Count != 0)
         {
            Logger.Warn("------------------------");
            Logger.Warn("Client Settings Warnings");
            Logger.Warn("------------------------");
            Logger.WarnFormat("Client Name: {0}", settings.Name);

            foreach (var error in cleanupWarnings)
            {
               Logger.Warn(error);
            }
         }

         IClient client;
         if (settings.IsFahClient())
         {
            client = FahClientFactory != null ? FahClientFactory.Create() : null;
         }
         else if (settings.IsLegacy())
         {
            client = LegacyClientFactory != null ? LegacyClientFactory.Create() : null;
         }
         else
         {
            // no External support yet
            throw new InvalidOperationException("Client type is not supported.");
         }

         if (client != null)
         {
            client.Settings = settings;
         }
         return client;
      }

      private static ICollection<string> CleanupSettings(ClientSettings settings)
      {
         var warnings = new List<string>();

         #region General Settings (common to FahClient and Legacy)

         if (!Validate.ClientName(settings.Name))
         {
            // remove illegal characters
            warnings.Add(String.Format(CultureInfo.InvariantCulture,
               "Client Name '{0}' contained invalid characters and was cleaned.", settings.Name));
            settings.Name = Validate.CleanClientName(settings.Name);
         }

         if (settings.ClientTimeOffset < Constants.MinOffsetMinutes ||
             settings.ClientTimeOffset > Constants.MaxOffsetMinutes)
         {
            warnings.Add("Client time offset is out of range, defaulting to 0.");
            settings.ClientTimeOffset = 0;
         }

         #endregion

         if (settings.IsLegacy())
         {
            #region Legacy Settings

            if (String.IsNullOrEmpty(settings.FahLogFileName))
            {
               warnings.Add("No remote FAHlog.txt filename, loading default.");
               settings.FahLogFileName = Constants.FahLogFileName;
            }

            if (String.IsNullOrEmpty(settings.UnitInfoFileName))
            {
               warnings.Add("No remote unitinfo.txt filename, loading default.");
               settings.UnitInfoFileName = Constants.UnitInfoFileName;
            }

            if (String.IsNullOrEmpty(settings.QueueFileName))
            {
               warnings.Add("No remote queue.dat filename, loading default.");
               settings.QueueFileName = Constants.QueueFileName;
            }

            #endregion
         }
         else if (settings.IsFahClient())
         {
            if (!Validate.ServerPort(settings.Port))
            {
               warnings.Add("Server port is invalid, loading default.");
               settings.Port = Constants.DefaultFahClientPort;
            }
         }
         
         return warnings.AsReadOnly();
      }

      public void Release(IClient client)
      {
         var fahClient = client as FahClient;
         if (fahClient != null)
         {
            if (FahClientFactory != null)
            {
               FahClientFactory.Release(fahClient);
            }
            return;
         }

         var legacyClient = client as LegacyClient;
         if (legacyClient != null)
         {
            if (LegacyClientFactory != null)
            {
               LegacyClientFactory.Release(legacyClient);
            }
         }
      }
   }
}
