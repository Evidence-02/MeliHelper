local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/FakeCrystalHeart"
entity.placements = {
    {
        name = "Fake Crystal Heart",
        data = {
			sprite = "heartgem0",
			spriteAfterCollect = "heartGemGhost",
			setFlagOnCollect = "",
			notAppearWhenFlag = "",
			hitboxRadius = 12,
			burstPeriod = 1.2,
			spriteChange = true,
			spriteStop = true
		}
    }
}

entity.fieldOrder = { "x", "y", 
	"sprite", "spriteAfterCollect", 
	"setFlagOnCollect", "notAppearWhenFlag", "hitboxRadius", "burstPeriod", "spriteChange", "spriteStop" 
}

function entity.texture(room, entity)
	return "collectables/strawberry/normal05"
end

return entity
