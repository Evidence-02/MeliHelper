local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BattleCityCampaignStartEntity"
entity.depth = 0
entity.placements = {
    {
        name = "Battle City Campaign Start Entity",
        data = {
			name = "Vanilla",
			lifes = 3
        }
    }
}

entity.fieldInformation = {
    lifes = {
        fieldType = "integer",
		minimumValue = 0
    }
}

function entity.texture(room, entity)
	return "Evidence02/objects_bc/loenn/campaign"
end

return entity