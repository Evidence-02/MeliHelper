local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityCustomEnemyType"
entity.depth = -9999
entity.placements = {
    {
        name = "Battle City Custom Enemy Type (basic)",
        data = {
			tankID = 'B',
			tankType = 'Basic',
			points = 100,
			health = 1,
			speedMove = 20,
			speedBullets = 100,
			shootFrequency = 2.4,
			canDamageSteel = false
        }
    },
    {
        name = "Battle City Custom Enemy Type (fast)",
        data = {
			tankID = 'F',
			tankType = 'Fast',
			points = 200,
			health = 1,
			speedMove = 40,
			speedBullets = 150,
			shootFrequency = 2.4,
			canDamageSteel = false
        }
    },
    {
        name = "Battle City Custom Enemy Type (power)",
        data = {
			tankID = 'P',
			tankType = 'Power',
			points = 300,
			health = 1,
			speedMove = 30,
			speedBullets = 200,
			shootFrequency = 1.8,
			canDamageSteel = true
        }
    },
    {
        name = "Battle City Custom Enemy Type (armored)",
        data = {
			tankID = 'A',
			tankType = 'Armored',
			points = 400,
			health = 4,
			speedMove = 30,
			speedBullets = 150,
			shootFrequency = 2.4,
			canDamageSteel = true
        }
    }
}

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