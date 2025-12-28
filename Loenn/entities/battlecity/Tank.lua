local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityTank"
entity.depth = -9999
entity.placements = { }
for tank, prms in pairs(meliLib.bcEnemyParams) do
	table.insert(entity.placements, {
        name = "Battle City Tank (".. tank ..")",
        data = {
			tankType = tank,
			points = prms.points,
			health = prms.health,
			speedMove = prms.speedMove,
			speedBullets = prms.speedBullets,
			shootFrequency = prms.shootFrequency,
			canDamageSteel = prms.canDamageSteel,
			bonusRandomPosition = true,
			containsBonus = false
        }
	})
end

entity.fieldInformation = {
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
	local tank_type = entity.tankType or 'Basic'
	local sprite_id = (	tank_type == 'Basic' and 4 or 
						tank_type == 'Fast'  and 5 or 
						tank_type == 'Power' and 6 or 7 )
	return "Evidence02/objects_bc/tanks/gray" .. sprite_id .. "0"
end

return entity