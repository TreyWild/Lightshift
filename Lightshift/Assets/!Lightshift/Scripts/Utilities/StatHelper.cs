using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Lightshift.Scripts.Utilities
{
    public class StatHelper
    {
        public static List<GameModifier> GetStatsFromShip(Player player, ShipObject shipObject) 
        {
            var upgrades = new List<GameModifier>();
            var stats = new List<GameModifier>();
            var items = player.GetItems();

            var equippedItems = items.Where(e => shipObject.EquippedModules.Contains(e.Id));
            foreach (var equip in equippedItems)
            {
                var item = ItemService.GetItem(equip.ModuleId);

                if (equip.Upgrades != null)
                    foreach (var upgrade in equip.Upgrades)
                    {
                        var upgradeInfo = item.Upgrades.FirstOrDefault(e => e.Id == upgrade.Id);
                        if (upgradeInfo == null)
                            continue;

                        var modifier = new GameModifier
                        {
                            Type = upgradeInfo.Type,
                            Value = (upgradeInfo.Value / 10) * upgrade.Level
                        };

                        if (modifier.Value != 0)
                            upgrades.Add(modifier);
                    }

                if (item.Stats != null)
                    foreach (var stat in item.Stats)
                    {
                        var modifier = stats.FirstOrDefault(s => s.Type == stat.Type);
                        if (modifier == null)
                            stats.Add( new GameModifier { Type = stat.Type, Value = stat.Value});
                        else modifier.Value += stat.Value;
                    }
            }

            foreach (var stat in stats)
            {
                var upgrade = upgrades.FirstOrDefault(e => e.Type == stat.Type);
                if (upgrade == null)
                    continue;

                upgrades.Remove(upgrade);

                stat.Value += upgrade.Value;
            }

            foreach (var upgrade in upgrades)
            {
                var modifier = stats.FirstOrDefault(s => s.Type == upgrade.Type);
                if (modifier == null)
                    stats.Add(upgrade);
                else modifier.Value += upgrade.Value;
            }

            return stats;
        }

        public static List<GameModifier> GetStatsFromItem(Item item)
        {
            var upgrades = new List<GameModifier>();
            var stats = new List<GameModifier>();

  
            var module = ItemService.GetItem(item.ModuleId);

            if (item.Upgrades != null)
                foreach (var upgrade in item.Upgrades)
                {
                    var upgradeInfo = module.Upgrades.FirstOrDefault(e => e.Id == upgrade.Id);
                    if (upgradeInfo == null)
                        continue;

                    var modifier = new GameModifier
                    {
                        Type = upgradeInfo.Type,
                        Value = (upgradeInfo.Value / 10) * upgrade.Level
                    };

                    if (modifier.Value != 0)
                        upgrades.Add(modifier);
                }

            if (module.Stats != null)
                foreach (var stat in module.Stats)
                {
                    var modifier = stats.FirstOrDefault(s => s.Type == stat.Type);
                    if (modifier == null)
                        stats.Add(new GameModifier { Type = stat.Type, Value = stat.Value });
                    else modifier.Value += stat.Value;
                }


            foreach (var stat in stats)
            {
                var upgrade = upgrades.FirstOrDefault(e => e.Type == stat.Type);
                if (upgrade == null)
                    continue;

                upgrades.Remove(upgrade);

                stat.Value += upgrade.Value;
            }

            foreach (var upgrade in upgrades)
            {
                var modifier = stats.FirstOrDefault(s => s.Type == upgrade.Type);
                if (modifier == null)
                    stats.Add(upgrade);
                else modifier.Value += upgrade.Value;
            }

            return stats;
        }
    }
}
