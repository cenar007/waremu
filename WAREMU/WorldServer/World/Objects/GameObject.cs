﻿/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class GameObject : Unit
    {
        public GameObject_spawn Spawn;

        public GameObject()
            : base()
        {

        }

        public GameObject(GameObject_spawn Spawn)
            : this()
        {
            this.Spawn = Spawn;
            Name = Spawn.Proto.Name;
        }

        public override void OnLoad()
        {
            Faction = Spawn.Proto.Faction;
            while (Faction >= 8) Faction -= 8;
            if (Faction < 2) Rank = 0;
            else if (Faction < 4) Rank = 1;
            else if (Faction < 6) Rank = 2;
            else if (Faction < 9) Rank = 3;
            Faction = Spawn.Proto.Faction;

            Level = Spawn.Proto.Level;
            MaxHealth = Math.Min(1,Spawn.Proto.HealthPoints);
            Health = TotalHealth;

            X = Zone.CalculPin((uint)(Spawn.WorldX), true);
            Y = Zone.CalculPin((uint)(Spawn.WorldY), false);
            Z = (ushort)(Spawn.WorldZ * 2);

            Heading = (ushort)Spawn.WorldO;
            SetOffset((ushort)(Spawn.WorldX >> 12), (ushort)(Spawn.WorldY >> 12));
            Region.UpdateRange(this);

            base.OnLoad();
        }

        public override void SendMeTo(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_CREATE_STATIC);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);

            Out.WriteUInt16((UInt16)Spawn.WorldO);
            Out.WriteUInt16((UInt16)Spawn.WorldZ);
            Out.WriteUInt32((UInt32)Spawn.WorldX);
            Out.WriteUInt32((UInt32)Spawn.WorldY);
            Out.WriteUInt16((ushort)Spawn.DisplayID);

            Out.WriteUInt16(Spawn.Proto.GetUnk(0));
            Out.WriteUInt16(Spawn.Proto.GetUnk(1));
            Out.WriteUInt16(Spawn.Proto.GetUnk(2));
            Out.WriteByte(0);
            Out.WriteUInt16(Spawn.Proto.GetUnk(3));
            Out.Fill(0, 5);
            Out.WriteUInt16(Spawn.Proto.GetUnk(4));
            Out.WriteUInt16(Spawn.Proto.GetUnk(5));

            Out.WriteUInt32(0);

            Out.WritePascalString(Name);
            Out.WriteByte(0);

            Plr.SendPacket(Out);

            base.SendMeTo(Plr);
        }

        public override void SendInteract(Player Plr, InteractMenu Menu)
        {
            Tok_Info Info = WorldMgr.GetTok((UInt32)Spawn.Proto.TokUnlock);

            if (!IsDead)
            {
                Plr.QtsInterface.HandleEvent(Objective_Type.QUEST_USE_GO, Spawn.Entry, 1);
            }

            Plr.TokInterface.AddTok(Info.Entry);

            base.SendInteract(Plr, Menu);
        }
    }
}
