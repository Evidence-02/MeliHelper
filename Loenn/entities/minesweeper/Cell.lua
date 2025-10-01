local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/MinesweeperCell"
entity.depth = 0
entity.placements = {
    {
        name = "Minesweeper Cell",
        data = {
			neighbors = 0,
			bomb = false,
			opened = false
        }
    }
}

entity.fieldInformation = {
	neighbors = {
		fieldType = "integer",
		minimumValue = 0
	}
}


function entity.selection(room, entity)
    local x, y, w = entity.x or 0, entity.y or 0, entity.width or 8
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function entity.texture(room, entity)
	if entity.bomb or false then
		return "Evidence02/objects_melihelper/minesweeper/cellBomb"
	end
	return "Evidence02/objects_melihelper/minesweeper/cell0" .. tostring(entity.neighbors or 0) 
end

return entity