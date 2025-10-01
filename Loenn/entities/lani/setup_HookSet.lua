local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniHookSet"
entity.placements = {
    {
        name = "Lani Hook Set",
        data = {
			direction = "Horizontal",
			length = 120,
			speed = 300,
			cooldown = 0.3,
			color = "0000FF",
			opacity = 0.5,
			loadOnce = true
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
	return "Evidence02/objects_lani/loenn/hookSet"
end

return entity
