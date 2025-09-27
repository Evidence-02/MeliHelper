local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityBonusRespawn"
entity.placements = {
    {
        name = "Battle City Bonus Respawn",
        data = {
			bonusType = "Star",
			period = 4,
			opacityInactive = 0.3,
			depth = 0
		}
    }
}

entity.fieldInformation = {
    bonusType = {
        editable = false,
		options = meliLib.bcBonusOptions
	},
	depth = {
		fieldType = "integer"
	}
}

function entity.texture(room, entity)
    return "Evidence02/objects_bc/bonuses/item" .. (entity.bonusType or 'Star')
end

return entity
