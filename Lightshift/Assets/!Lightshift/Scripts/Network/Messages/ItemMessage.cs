using Mirror;
using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


struct ItemMessage : NetworkMessage
{
    public List<Upgrade> Upgrades;
    public List<ResourceObject> SpentResources;
    public string Id;
    public string ModuleId;
    public string Color;
    public ModuleLocation ModuleLocation;
    public int MaxUpgrades;
}
