local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniHookSetEntity"
entity.placements = {
    {
        name = "Lani Hook Set Entity",
        data = {
			sprite = "MeliHelper_LaniHookEntity",
			dialogueOnCollect = "",
			flagOnCollect = "",
			periodBurst = 1.2,
			distFloating = 8,
			hitboxSize = 6,
			addLight = true,
			
			hookDirection = "Horizontal",
			hookLength = 120,
			hookSpeed = 450,
			hookSpeedMovePlayer = 300,
			hookCooldown = 0,
			hookColor = "0000FF",
			hookOpacity = 0.5,
			hookAllowHypers = true,
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"hookDirection", "hookLength", "hookSpeed", "hookSpeedMovePlayer", "hookCooldown", "hookColor", "hookOpacity", "hookAllowHypers",
	"sprite", "dialogueOnCollect", "flagOnCollect", "periodBurst", "distFloating", "hitboxSize",
	"addLight"
}

entity.fieldInformation = {
    hookDirection = {
		options = { "Horizontal", "All directions" },
		editable = false
    },
	hookColor = { fieldType = "color" }
}

function entity.texture(room, entity)
	return "Evidence02/objects_lani/hook/idle00"
end

return entity
