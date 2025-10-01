local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BombermanCapsule"
entity.placements = {
    {
        name = "Bomberman Capsule",
        data = {
			lifes = 10
        }
    }
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y, 16, 16)
end

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/bomberman_capsule/idle00"
end

return entity
