local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityCustomRulesEntity"
entity.depth = -9999
entity.placements = {
    {
        name = "Battle City Custom Rules",
        data = {
			playerShotsAtOnce = 2,
			playerShotSpeed = 270,
			playerCanDestroySteel = false,
			playerCustomShooting = true,
			vanillaDeaths = true,
			shootOnlyCenter = false
			
        }
    }
}

entity.fieldInformation = {
	playerShotsAtOnce = {
		fieldType = "integer",
		minimumValue = 1
	}
}

function entity.texture(room, entity)
	return "Evidence02/objects_bc/loenn/custom_rules"
end

return entity