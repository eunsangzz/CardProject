using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
//버튼이름에서 커맨트 매핑
public static class CardCommandRegistry
{
    public static Dictionary<string, ICardCommand> Bulid()
    {
        return new Dictionary<string, ICardCommand>
        {
            { "Tree", new TreeToWoodCommand() },
            { "Rock", new RockToStoneCommand() },
            { "BananaTree", new BananaTreeToBananaCommand() },
            { "StrawBerryTree", new StrawberryTreeToStrawberryCommand() },
            { "Wood", new WoodToBranchCommand() },

            { "ForgeIron", new ForgeIronCommand() },
            { "ForgeGold", new ForgeGoldCommand() },
            { "Timber", new TimberToPanelCommand() },
            { "Mine", new MineToBrickCommand() },

            { "House", new HouseSpawnPlayerCommand() },
        };
    }
}
