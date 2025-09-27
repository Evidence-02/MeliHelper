local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/MinesweeperTimer"
entity.depth = 0
entity.placements = {
    {
        name = "Minesweeper Timer",
        data = {
			digits = 3
        }
    }
}

entity.fieldInformation = {
	digits = {
		fieldType = "integer",
		minimumValue = 0
	}
}

function entity.selection(room, entity)
    local x, y, w = entity.x or 0, entity.y or 0, 13 * (entity.digits or 3)
    return utils.rectangle(x, y, w, 23)
end

function entity.draw(room, entity)
    local x, y, w = entity.x or 0, entity.y or 0, entity.digits or 3
	for i = 0, w - 1 do 
		-- sprite draw
		local entitySprite = drawableSprite.fromTexture("Evidence02/objects_melihelper/minesweeper/digit00")
		entitySprite.x = x + 13 * i + 6
		entitySprite.y = y + 11
		entitySprite:draw()
	end
end

return entity