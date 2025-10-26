local fakeTilesHelper = require("helpers.fake_tiles")

local entity = {}
entity.name = "MeliHelper/SolidAppearOnFlag"
entity.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")
entity.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
entity.placements = {
    {
        name = "Solid Appear On Flag",
        data = {
			width = 8,
			height = 8,
			flag = "",
			tiletype = "3",
			appearTime = 1.0,
			disappearTime = 0.3,
			appearSound = "",
			disappearSound = "",
			blendin = false
        }
    }
}

return entity