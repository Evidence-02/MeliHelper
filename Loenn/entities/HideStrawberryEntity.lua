local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/HideStrawberryEntity"
entity.placements = {
    {
        name = "Hide Strawberry Entity",
        data = {
			affect = "All Strawberries",
			hideSprite = false,
			hideLight = true,
			hideBloom = true,
			setDepth = true,
			depth = 9000
        }
    }
}

entity.fieldInformation = {
    affect = {
		options = { "All Strawberries", "Nearest" },
		editable = false
    },
	depth = { fieldType = "integer" }
}

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/loenn/hideStrawberry"
end

return entity
