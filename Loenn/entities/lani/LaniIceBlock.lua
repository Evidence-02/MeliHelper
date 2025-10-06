local fakeTilesHelper = require("helpers.fake_tiles")

local entity = {}
entity.name = "MeliHelper/LaniIceBlock"
entity.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")
entity.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
entity.placements = {
    {
        name = "Lani Ice Block",
        data = {
			width = 8,
			height = 8,
			tiletype = "3",
			blendin = false
        }
    },
    {
        name = "Lani Ice Block (ice, delete later!)",
        data = {
			width = 8,
			height = 8,
			tiletype = "A",
			blendin = false
        }
    },
	
	
}

return entity