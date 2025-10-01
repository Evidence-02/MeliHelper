local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BattleCityCampaignClearInfo"
entity.depth = 0
entity.placements = {
    {
        name = "Battle City Campaign Clear Info",
        data = {
        }
    }
}

function entity.texture(room, entity)
	return "Evidence02/objects_bc/loenn/campaign_clear"
end

return entity