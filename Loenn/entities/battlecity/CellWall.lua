local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local meliLib = require('mods').requireFromPlugin('libraries.meliLib')

local entity = {}
entity.name = "MeliHelper/BattleCityCellWall"
entity.depth = 0
entity.placements = {}

for _, cell in ipairs(meliLib.bcCellOptions) do
    table.insert(entity.placements, {
        name = "Battle City Wall ".. cell,
        data = {
			width = 16,
			height = 16,
            cellType = cell
        }
    })
end

entity.fieldInformation = {
    cellType = {
        options = meliLib.bcCellOptions,
		editable = false
    }
}

function entity.draw(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local w, h = entity.width or 0, entity.height or 0
	local spritepath = "Evidence02/objects_bc/tiles/" .. string.lower(entity.cellType or "Brick") .. "00"
	
    for i = 0, w-8, 8 do
		for j = 0, h-8, 8 do
			local entitySprite = drawableSprite.fromTexture(spritepath)
			entitySprite.x = x + i + 4
			entitySprite.y = y + j + 4
			entitySprite:draw()
		end
    end
end

return entity