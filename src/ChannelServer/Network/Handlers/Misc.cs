﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Shared.Network;
using Aura.Channel.Network.Sending;
using Aura.Shared.Util;
using Aura.Shared.Mabi.Const;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Sent regularly to request the current moon gates (?).
		/// </summary>
		/// <remarks>
		/// It seems strange that the moon gates are requested over and over,
		/// but the official answer is always the names of 2 moon gates.
		/// </remarks>
		/// <example>
		/// No Parameters.
		/// </example>
		[PacketHandler(Op.MoonGateInfoRequest)]
		public void MoonGateInfoRequest(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreature(packet.Id);
			if (creature == null)
				return;

			// Empty answer for now.
			Send.MoonGateInfoRequestR(creature);
		}

		/// <summary>
		/// Sent on login to request a list of new mails.
		/// </summary>
		/// <remarks>
		/// Only here to get rid of the unimplemented log for now.
		/// </remarks>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.MailsRequest)]
		public void MailsRequest(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreature(packet.Id);
			if (creature == null)
				return;

			// Empty answer for now.
			Send.MailsRequestR(creature);
		}

		/// <summary>
		/// Sent on login, answer determines whether the SOS button is displayed.
		/// </summary>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.SosButtonRequest)]
		public void SosButtonRequest(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreature(packet.Id);
			if (creature == null)
				return;

			// Disable by default, until we have the whole thing.
			Send.SosButtonRequestR(creature, false);
		}

		/// <summary>
		/// Sent on login to get homestead information.
		/// </summary>
		/// <example>
		/// 001 [..............00] Byte   : 0
		/// </example>
		[PacketHandler(Op.HomesteadInfoRequest)]
		public void HomesteadInfoRequest(ChannelClient client, Packet packet)
		{
			var unkByte = packet.GetByte();

			var creature = client.GetCreature(packet.Id);
			if (creature == null)
				return;

			// Default answer for now
			Send.HomesteadInfoRequestR(creature);
		}
	}
}