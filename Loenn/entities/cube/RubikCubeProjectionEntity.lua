local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/RubikCubeProjectionEntity"
entity.depth = 0
entity.placements = {
    {
        name = "Rubik Cube Projection Entity",
        data = {
			dx = 0,
			dy = 180,
			size = 0.3,
			showBorders = false
        }
    }
}

entity.fieldOrder = { 
	"x", "y", "dx", "dy", "size", "showBorders"
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 12, y - 12, 24, 24)
end

function entity.texture(room, entity)
	return "Evidence02/objects_cubes/loenn/projection"
end

return entity
