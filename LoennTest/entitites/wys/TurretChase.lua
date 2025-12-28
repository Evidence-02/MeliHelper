local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/WYSTurretChase"
entity.placements = {
    {
        name = "WYS Turret Chase",
        data = {
			playerDistance = 32,
			direction = "Down",
			bullets = 8,
			bulletSpeed = 120,
			bulletColor = "0000FF",
			bulletScatter = 0,
			delayShoot = 1.2,
			periodShoot = 0.12,
			followDelay = 0.55
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"playerDistance", "direction", "bullets", "bulletSpeed", "bulletColor", "bulletScatter", "delayShoot", "periodShoot", "followDelay"
}

entity.fieldInformation = {
    direction = {
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
