﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Data;
using Aura.Channel.World.Entities;

namespace Aura.Channel.World
{
	public class WorldManager
	{
		public static readonly WorldManager Instance = new WorldManager();

		private Dictionary<int, Region> _regions;

		public int Count { get { return _regions.Count; } }

		private WorldManager()
		{
			_regions = new Dictionary<int, Region>();
		}

		/// <summary>
		/// Adds new region with regionId.
		/// </summary>
		/// <param name="regionId"></param>
		public void AddRegion(int regionId)
		{
			lock (_regions)
				_regions.Add(regionId, new Region(regionId));
		}

		/// <summary>
		/// Removes region with RegionId.
		/// </summary>
		/// <param name="regionId"></param>
		public void RemoveRegion(int regionId)
		{
			lock (_regions)
				_regions.Remove(regionId);
		}

		/// <summary>
		/// Returns region by id, or null if it doesn't exist.
		/// </summary>
		/// <param name="regionId"></param>
		/// <returns></returns>
		public Region GetRegion(int regionId)
		{
			Region result;
			lock (_regions)
				_regions.TryGetValue(regionId, out result);
			return result;
		}

		/// <summary>
		/// Initializes world (regions, etc)
		/// </summary>
		public void Initialize()
		{
			// Create default regions from data entries.
			foreach (var region in AuraData.RegionDb.Entries.Values)
			{
				this.AddRegion(region.Id);
			}
		}

		/// <summary>
		/// Returns first prop with the given id, from any region,
		/// or null, if none was found.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Prop GetProp(long id)
		{
			foreach (var region in _regions.Values)
			{
				var prop = region.GetProp(id);
				if (prop != null)
					return prop;
			}

			return null;
		}

		/// <summary>
		/// Returns player creature with the given name, or null.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public PlayerCreature GetPlayer(string name)
		{
			foreach (var region in _regions.Values)
			{
				var creature = region.GetPlayer(name);
				if (creature != null)
					return creature;
			}

			return null;
		}
	}
}