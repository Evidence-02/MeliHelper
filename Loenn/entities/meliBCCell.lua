local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local bccell = {}
bccell.name = "MeliHelper/BattleCityCell"
bccell.depth = 0
bccell.placements = {}

for _, cell in ipairs(meliLib.bcCellOptions) do
    table.insert(bccell.placements, {
        name = "Battle City Cell ".. cell,
        data = {
            cellType = cell,
            fill = "Full"
        }
    })
end


bccell.fieldInformation = {
    cellType = {
        options = meliLib.bcCellOptions,
		editable = false
    },
    fill = {
        options = { "Full", "Left", "Right", "Top", "Bottom" },
		editable = false
    }
}



function bccell.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function bccell.draw(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local fill = entity.fill or "Full"
	local spritepath = "Evidence02/objects_bc/tiles/" .. string.lower(entity.cellType or "Brick") .. "00"
	
	local topLeft     = fill == 'Full' or fill == 'Left'  or fill == 'Top'    --or fill == 'Custom' and entity.topLeft
	local topRight    = fill == 'Full' or fill == 'Right' or fill == 'Top'    --or fill == 'Custom' and entity.topRight
	local bottomLeft  = fill == 'Full' or fill == 'Left'  or fill == 'Bottom' --or fill == 'Custom' and entity.bottomLeft
	local bottomRight = fill == 'Full' or fill == 'Right' or fill == 'Bottom' --or fill == 'Custom' and entity.bottomRight
	
	-- sprite draw
	if topLeft then 
		local entitySprite = drawableSprite.fromTexture(spritepath)
		entitySprite.x = x - 4
		entitySprite.y = y - 4
		entitySprite:draw()
	end

	if topRight then 
		local entitySprite = drawableSprite.fromTexture(spritepath)
		entitySprite.x = x + 4
		entitySprite.y = y - 4
		entitySprite:draw()
	end
	
	if bottomLeft then 
		local entitySprite = drawableSprite.fromTexture(spritepath)
		entitySprite.x = x - 4
		entitySprite.y = y + 4
		entitySprite:draw()
	end
	
	if bottomRight then 
		local entitySprite = drawableSprite.fromTexture(spritepath)
		entitySprite.x = x + 4
		entitySprite.y = y + 4
		entitySprite:draw()
	end
end

return bccell
