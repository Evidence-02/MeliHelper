local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/CustomEntityPortal"
entity.placements = {
    {
        name = "Custom Entity Portal",
        data = {
			portalID = "1",
			portalType = "In",
			sprite = "MeliHelper_WarpZone",
			radius = 16,
			color = "0000FF",
			opacity = 1,
			onlyOnce = true,
			ignorePlayer = false
        }
    }
}

entity.fieldOrder = { "x", "y", "portalType", "portalID", "sprite", "radius", "color", "opacity", "onlyOnce", "ignorePlayer" }

entity.fieldInformation = {
	color  = { fieldType = "color" },
	portalType = {
		options = {	"In", "Out" },
		editable = false
	}
}

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/warpzone/idle00"
end

return entity
