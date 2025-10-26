local fakeTilesHelper = require("helpers.fake_tiles")

local entity = {}
entity.name = "MeliHelper/KillPlayerDashBlock"
entity.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
entity.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")
entity.placements = {
    {
        name = "Kill Player Dash Block",
        data = {
			width = 8,
			height = 8,
			tiletype = "3",
			blendin = false,
			canDash = true,
			permanent = false
        }
    }
}

entity.fieldOrder = { "x", "y", "width", "height", "tiletype", "blendin", "canDash", "permanent" }
return entity