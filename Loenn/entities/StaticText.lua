local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/StaticText"
entity.placements = {
    {
        name = "StaticText",
        data = {
			text = "",
			textType = "CoreMessage",
			colorText = "FFFFFF",
			colorOutline = "FFFFFF",
			size = 1,
			strokeThickness = 2,
			koefFloating = 0.2,
			opacity = 1
        }
    }
}

entity.fieldOrder = { 
	"x", "y", "text", "textType", 
	"colorText", "colorOutline", "size", "strokeThickness", "koefFloating", "opacity"
}

entity.fieldInformation = {
	textType = {
		options = { "CoreMessage", "CoreMessageOutline", "NES", "Classic", "ClassicOutline" },
		editable = false
	},
	colorText = { fieldType = "color" },
	colorOutline = { fieldType = "color" },
	opacity = {
		minimumValue = 0,
		maximumValue = 1
	}
}

function entity.texture(room, entity)
	return "@Internal@/core_message"
end

return entity
