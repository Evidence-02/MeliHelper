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
			
		
			direction = "Horizontal",
			length = 120,
			speed = 300,
			cooldown = 0.3,
			color = "0000FF",
			opacity = 0.5
        }
    }
}

entity.fieldInformation = {
    direction = {
		options = { "Horizontal", "All directions" },
		editable = false
    },
	color = { fieldType = "color" }
}

function entity.texture(room, entity)
	return "Evidence02/objects_lani/hook/idle00"
end

return entity
