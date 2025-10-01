local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/DateTimeMessage"
entity.placements = {
    {
        name = "Datetime Message",
        data = {
			dateTimeFormat = "",
			colorText = "FFFFFF",
			opacity = 1,
			size = 1,
			outline = false
			
        }
    }
}


entity.fieldInformation = {
	colorText = { fieldType = "color" },
	colorOutline = { fieldType = "color" },
	size = { fieldType = "integer", minimumValue = 1 },
	opacity = {
		minimumValue = 0,
		maximumValue = 1
	}
}

function entity.texture(room, entity)
	return "@Internal@/core_message"
end

return entity
