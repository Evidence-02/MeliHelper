local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityCustomEnemyType"
entity.depth = -9999
entity.placements = { }
for tank, prms in pairs(meliLib.bcEnemyParams) do
	table.insert(entity.placements, {
        name = "Battle City Custom Enemy Type (".. tank ..")",
        data = {
			tankID = prms.tankID,
			tankType = tank,
			points = prms.points,
			health = prms.health,
			speedMove = prms.speedMove,
			speedBullets = prms.speedBullets,
			shootFrequency = prms.shootFrequency,
			canDamageSteel = prms.canDamageSteel
        }
	})
end

entity.fieldInformation = {
	tankID = {
		editable = false
	},
    tankType = {
        options = meliLib.bcEnemyOptions,
		editable = false
    },
	points = {
		fieldType = "integer"
	},
	health = {
		fieldType = "integer",
		minimumValue = 0
	},
	speedMove = {
		fieldType = "integer",
		minimumValue = 0
	},
	speedBullets = {
		fieldType = "integer",
		minimumValue = 0
	},
	shootFrequency = {
		minimumValue = 0.02
	}
}

function entity.texture(room, entity)
	return "Evidence02/objects_bc/loenn/custom_enemies"
end

return entity