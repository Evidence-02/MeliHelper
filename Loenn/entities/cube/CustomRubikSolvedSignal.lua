local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/CustomRubikSolvedSignal"
entity.depth = -9999999
entity.placements = {
    {
        name = "Custom Rubik Solved Signal",
        data = {
			width = 8,
			height = 8,
			cubeID = "",
			texture = "",
			color = "000000",
			opacity = 0.6,
			depth = -9999999
        }
    }
}

entity.fieldOrder = { "x", "y", "width", "height", "cubeID", "texture", "color", "opacity", "depth" }

entity.fieldInformation = {
	color = { fieldType = "color" }
}

function entity.draw(room, entity)
    local x, y, w, h = entity.x or 0, entity.y or 0, entity.width or 8, entity.height or 8
	local clr = entity.color or '000000'
	local opacity = entity.opacity or 0.6
	
    local r, g, b, a = love.graphics.getColor()
	local success, tr, tg, tb = utils.parseHexColor(clr)
	if success then 
		love.graphics.setColor(tr, tg, tb, opacity)
		love.graphics.rectangle("fill", entity.x + 0.5, entity.y + 0.5, entity.width - 1, entity.height - 1)
	end
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
	
	if entity.texture ~= '' then 
		local entitySprite = drawableSprite.fromTexture(entity.texture)
		entitySprite.x = x + w / 2
		entitySprite.y = y + h / 2
		entitySprite:draw()
	end
	
end

return entity