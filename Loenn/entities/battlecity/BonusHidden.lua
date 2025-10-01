local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityBonusHidden"
entity.placements = {
    {
        name = "Battle City Bonus Hidden",
        data = {
			bonusType = "Star",
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
