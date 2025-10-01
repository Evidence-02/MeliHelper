local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local bccell = {}
bccell.name = "MeliHelper/BattleCityCellBrickRow"
bccell.depth = 0
bccell.placements = {
    {
        name = "Battle City Cell Brick Row",
        data = {
            row1 = "11111111",
			row2 = "10001001",
			row3 = "10010001",
			row4 = "11111111"
        }
    }
}


function bccell.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x, y, 16, 16)
end

function bccell.draw(room, entity)
    local x, y = (entity.x or 0) + 2, (entity.y or 0) + 2
	local brick1 = drawableSprite.fromTexture("Evidence02/objects_bc/tiles/brick01")
	local brick2 = drawableSprite.fromTexture("Evidence02/objects_bc/tiles/brick02")
	
	local x_saved = x
	local mass_bricks = (entity.row1 or '11111111') ..','.. (entity.row2 or '10001001') ..','.. (entity.row3 or '10010001') ..','.. (entity.row4 or '11111111')
	for str in string.gmatch(mass_bricks, "[^,]+") do
		for i = 1, #str do
			if string.sub(str,i,i) ~= '0' then
				local sprite = (math.fmod(x+y, 8) == 4) and brick1 or brick2
				sprite.x = x
				sprite.y = y
				sprite:draw()
			end
			x = x + 4
		end
		y = y + 4
		x = x_saved
	end
end

return bccell
