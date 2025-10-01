local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BattleCityFlag"
entity.depth = 0
entity.placements = {
    {
        name = "Battle City Flag",
        data = {
			sprite = "MeliHelper_BC_Flag",
			deathEffectColor = "000000",
			createDeathEffect = false,
			damageOnDash = true,
			killPlayerOnFail = false
        }
    }
}

entity.fieldInformation = {
    deathEffectColor = {
        fieldType = "color"
    }
}

function entity.texture(room, entity)
	return "Evidence02/objects_bc/flag/idle00"
end

return entity