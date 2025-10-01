local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BombermanStation"
entity.placements = {
    {
        name = "Bomberman Station",
        data = {
        }
    }
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 24, y - 16, 48, 48)
end

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/bomberman_station/idle00"
end

return entity
