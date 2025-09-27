local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityTank"
entity.depth = -9999
entity.placements = {
    {
        name = "Battle City Tank (basic)",
        data = {
			tankType = 'Basic',
			points = 100,
			health = 1,
			speedMove = 36,
			speedBullets = 100,
			shootFrequency = 2.4,
			canDamageSteel = false,
			bonusRandomPosition = true,
			containsBonus = false
        }
    },
    {
        name = "Battle City Tank (fast)",
        data = {
			tankType = 'Fast',
			points = 200,
			health = 1,
			speedMove = 60,
			speedBullets = 150,
			shootFrequency = 2.4,
			canDamageSteel = false,
			bonusRandomPosition = true,
			containsBonus = false
        }
    },
    {
        name = "Battle City Tank (power)",
        data = {
			tankType = 'Power',
			points = 300,
			health = 1,
			speedMove = 48,
			speedBullets = 200,
			shootFrequency = 1.8,
			canDamageSteel = true,
			bonusRandomPosition = true,
			containsBonus = false
        }
    },
    {
        name = "Battle City Tank (armored)",
        data = {
			tankType = 'Armored',
			points = 400,
			health = 4,
			speedMove = 48,
			speedBullets = 150,
			shootFrequency = 2.4,
			canDamageSteel = true,
			bonusRandomPosition = true,
			containsBonus = false
        }
    }
}

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