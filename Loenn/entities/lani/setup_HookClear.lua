local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniHookClear"
entity.placements = {
    {
        name = "Lani Hook Clear",
        data = {
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
	return "Evidence02/objects_lani/loenn/hookClear"
end

return entity
