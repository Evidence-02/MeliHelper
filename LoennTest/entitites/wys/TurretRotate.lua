local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/WYSTurretRotate"
entity.placements = {
    {
        name = "WYS Turret Rotate",
        data = {
			playerDistance = 32,
			startDirection = "Up",
			bullets = 8,
			bulletSpeed = 120,
			bulletColor = "0000FF",
			bulletScatter = 0,
			delayShoot = 1.2,
			moveTime = 0.2,
			periodShoot = 0.12,
			
			left = true,
			right = true,
			up = true,
			down = true,
			moveClockwise = false,
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"playerDistance", "startDirection", "bullets", "bulletSpeed", "bulletColor", "bulletScatter", "delayShoot", "moveTime", "periodShoot", 
	"moveClockwise",
	"left", "right", "up", "down"
}

entity.fieldInformation = {
    startDirection = {
        options = { "Up", "Down", "Left", "Right" },
		editable = false
    },
    bullets = { fieldType = "integer" },
	bulletColor = { fieldType = "color" },
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function entity.texture(room, entity)
	return "Evidence02/objects_wys/loenn/turret"
end

return entity
