﻿/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Core.World;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Actors.Actions.Player
{
    internal class PlayerUseItemAction : IAction<Actors.Player>
    {
        private readonly int _slotNum;

        public PlayerUseItemAction(int slotNum)
        {
            _slotNum = slotNum;
        }

        public void Execute(Actors.Player player)
        {
            // Sanity check: is there actually an item in this slot?
            if (player.Inventory.GetSlot(_slotNum) == null)
            {
                // Log it!
                Engine.Services.Get<Logger>().LogEvent($"Player attempted to equip bad item! User: {player.Descriptor.Name} SlotNum: {_slotNum}.", LogTypes.GAME, new Exception($"Player attempted to equip bad item! User: {player.Descriptor.Name} SlotNum: {_slotNum}."));

                return;
            }

            Item item = player.Inventory.GetSlot(_slotNum).Item;

            if (item.Descriptor.ItemType == ItemTypes.Equipment)
            {
                player.Equipment.Equip(item);
                item.OnEquip(player);
                player.Inventory.RemoveItem(_slotNum, 1);
            }
            else if (item.Descriptor.ItemType == ItemTypes.Usable)
            {
                item.OnUse(player);
            }
        }
    }
}