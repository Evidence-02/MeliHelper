local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniHookSet"
entity.placements = {
    {
        name = "Lani Hook Set",
        data = {
			hookDirection = "Horizontal",
			hookLength = 120,
			hookSpeed = 450,
			hookSpeedMovePlayer = 300,
			hookCooldown = 0,
			hookColor = "0000FF",
			hookOpacity = 0.5,
			hookAllowHypers = true,
			loadOnce = true
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"hookDirection", "hookLength", "hookSpeed", "hookSpeedMovePlayer", "hookCooldown", "hookColor", "hookOpacity", "hookAllowHypers",
	"loadOnce"
}

entity.fieldInformation = {
    hookDirection = {
		options = { "Horizontal", "All directions" },
		editable = false
    },
	hookColor = { fieldType = "color" }
}

function entity.texture(room, entity)
	return "Evidence02/objects_lani/loenn/hookSet"
end

return entity
