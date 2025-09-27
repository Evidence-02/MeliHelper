local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/WarpZone"
entity.placements = {
    {
        name = "Warp Zone",
        data = {
			sprite = "MeliHelper_WarpZone",
			color = "FF0000",
			roomTeleport = "a-00",
			flagNotAppear = "",
			ttl = 20,
			radius = 16,
			opacity = 0.6,
			sound = "",
			textureType = 2
        }
    }
}

entity.fieldInformation = {
    radius = { fieldType = "integer" },
	color  = { fieldType = "color" },
	textureType = {
		field_type = "integer",
		options = {
			["Dark"] = 1,
			["Light"] = 2
		},
		editable = false
	}
}

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/warpzone/idle00"
end

return entity
