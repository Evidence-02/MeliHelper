local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/WYSBoss01"
entity.placements = {
    {
        name = "WYS Boss 01",
        data = {
			bulletSpeed = 120,
			bulletColor = "0000FF",
			followDelay = 0.5,
			periodShoot = 0.12,
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"bulletSpeed", "bulletColor", "followDelay", "periodShoot"
}

entity.fieldInformation = {
	bulletColor = { fieldType = "color" }
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function entity.texture(room, entity)
	return "Evidence02/objects_wys/loenn/boss01"
end

return entity
