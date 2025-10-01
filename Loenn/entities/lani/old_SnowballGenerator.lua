local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "LanisTrek/SnowballGenerator"
entity.placements = {
}
entity.fieldOrder = { "x", "y", "period", "delay", "speedX", "speedY", "offsetX", "offsetY", "bounces" }



function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function entity.draw(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    
	-- sprite draw
	local entitySprite = drawableSprite.fromTexture("Evidence02/objects_lani/loenn/snowball_gen")
	entitySprite.x = x
	entitySprite.y = y
	entitySprite:draw()
	
	-- additional text
    local font = love.graphics.getFont()
    local textBatch = love.graphics.newText(font)
    drawing.callKeepOriginalColor(function()
		love.graphics.setColor(1.0, 0.0, 0.0, 1.0)
		
		local cy = y - 2
		drawing.printCenteredText('Period: ' .. entity.period ..''.. (entity.delay > 0 and (' ('..entity.delay ..')') or ''), x + 5, cy - 8, 120, 12, font, 1)
		cy += 8
		
		drawing.printCenteredText('speed: ' .. entity.speedX ..','.. entity.speedY, x + 5, cy - 8, 120, 12, font, 1)
		cy += 8
		
		if (entity.offsetX or 0) ~= 0 or (entity.offsetY or 0) ~= 0 then
			drawing.printCenteredText('offset: ' .. entity.offsetX ..','.. entity.offsetY, x + 5, cy - 8, 120, 12, font, 1)
			cy += 8
		end
		
		drawing.printCenteredText('bounces: ' .. entity.bounces, x + 5, cy - 8, 120, 12, font, 1)
		cy += 8
		
		--drawing.printCenteredText('Figures: ' .. entity.figureSet,                  x + 5, y +  6, 120, 12, font, 1)
		--drawing.printCenteredText('Period: ' .. entity.periodFall,                  x + 5, y + 14, 120, 12, font, 1)
	end)
    love.graphics.draw(textBatch)
end

return entity
