﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Linq;
using System.Collections.Generic;
using Aura.Shared.Util;
using System.Reflection;

namespace Aura.Shared.Network
{
	/// <summary>
	/// Packet handler manager base class.
	/// </summary>
	/// <typeparam name="TClient"></typeparam>
	public abstract class PacketHandlerManager<TClient> where TClient : BaseClient
	{
		public delegate void PacketHandlerFunc(TClient client, Packet packet);

		private Dictionary<int, PacketHandlerFunc> _handlers;

		public PacketHandlerManager()
		{
			_handlers = new Dictionary<int, PacketHandlerFunc>();
		}

		/// <summary>
		/// Adds and/or overwrites handler.
		/// </summary>
		/// <param name="op"></param>
		/// <param name="handler"></param>
		public void Add(int op, PacketHandlerFunc handler)
		{
			if (_handlers.ContainsKey(op))
				Log.Warning("PacketHandlerManager: Overwriting handler for '{0:X4}' with '{1}'.", op, handler.Method.DeclaringType + "." + handler.Method.Name);

			_handlers[op] = handler;
		}

		/// <summary>
		/// Adds all methods with a Handler attribute.
		/// </summary>
		public void AutoLoad()
		{
			// Methods
			foreach (var method in this.GetType().GetMethods())
			{
				foreach (PacketHandlerAttribute attr in method.GetCustomAttributes(typeof(PacketHandlerAttribute), false))
				{
					var del = (PacketHandlerFunc)Delegate.CreateDelegate(typeof(PacketHandlerFunc), this, method);
					foreach (var op in attr.Ops)
						this.Add(op, del);
				}
			}

			// Classes
			foreach (var type in Assembly.GetCallingAssembly().GetTypes().Where(a => a.GetInterfaces().Contains((typeof(IPacketHandler<TClient>)))))
			{
				foreach (PacketHandlerAttribute attr in type.GetCustomAttributes(typeof(PacketHandlerAttribute), false))
				{
					var handler = Activator.CreateInstance(type) as IPacketHandler<TClient>;

					foreach (var op in attr.Ops)
						this.Add(op, handler.Handle);
				}
			}
		}

		/// <summary>
		/// Runs handler for packet's op, or logs it as unimplemented.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		public void Handle(TClient client, Packet packet)
		{
			// Don't log internal packets
			//if (packet.Op < Op.Internal.ServerIdentify)
			//    Log.Debug("R: " + packet);

			PacketHandlerFunc handler;
			if (!_handlers.TryGetValue(packet.Op, out handler))
			{
				this.UnknownPacket(client, packet);
				return;
			}

			try
			{
				handler(client, packet);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "There has been a problem while handling '{0:X4}'.", packet.Op);
			}
		}

		public virtual void UnknownPacket(TClient client, Packet packet)
		{
			Log.Unimplemented("PacketHandlerManager: Handler for '{0:X4}'", packet.Op);
			Log.Debug(packet);
		}
	}

	/// <summary>
	/// Methods having this attribute are registered as packet handlers,
	/// for the ops.
	/// </summary>
	public class PacketHandlerAttribute : Attribute
	{
		public int[] Ops { get; protected set; }

		public PacketHandlerAttribute(params int[] ops)
		{
			this.Ops = ops;
		}
	}
}
