local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/NiceFrame"
entity.placements = {
    {
        name = "Nice Frame",
        data = {
			width = 128,
			height = 64,
			textureBorder = "Evidence02/objects_melihelper/niceframe/typeB",
			colorBorder = "FF9D14",
			colorInside = "000000",
			opacityBorder = 1,
			opacityInside = 1,
			topLeft = true,
			topRight = true,
			bottomLeft = true,
			bottomRight = true,
			solid = false
        }
    }
}

entity.fieldOrder = { "x", "y", "width", "height", 
	"textureBorder", 
	"colorBorder", "colorInside", "opacityBorder", "opacityInside",
	"topLeft", "topRight", "bottomLeft", "bottomRight", "solid"
	}

entity.fieldInformation = {
	colorBorder  = { fieldType = "color" },
	colorInside  = { fieldType = "color" },
	opacityBorder  = { minimumValue = 0, maximumValue = 1 },
	opacityInside  = { minimumValue = 0, maximumValue = 1 },
}

function entity.draw(room, entity)
	local x, y, w, h = entity.x, entity.y, entity.width or 48, entity.height or 16
    local r, g, b, a = love.graphics.getColor()
	local clr_border = entity.colorBorder or 'FF9D14'
	local clr_inside = entity.colorInside or '000000'
	local opacity_border = entity.opacityBorder or 1
	local opacity_inside = entity.opacityInside or 1
	local texture_border = entity.textureBorder or "Evidence02/objects_melihelper/niceframe/typeB"
	
	local success, tr, tg, tb = utils.parseHexColor(clr_inside)
	if success then
		love.graphics.setColor(tr, tg, tb, opacity_inside)
		love.graphics.rectangle("fill", x + 0.5, y + 0.5, w - 1, h - 1)
	end
	
	local success, tr, tg, tb = utils.parseHexColor(clr_border)
	if success then
		love.graphics.setColor(tr, tg, tb, opacity_border)
		love.graphics.rectangle("line", x + 0.5, y + 0.5, w - 1, h - 1)
	end
	
	if entity.topLeft then
		local entitySprite = drawableSprite.fromTexture(texture_border)
		entitySprite.x = x
		entitySprite.y = y
		entitySprite:setJustification(0, 0)
		entitySprite:draw()
	end
	
	if entity.topRight then
		local entitySprite = drawableSprite.fromTexture(texture_border)
		entitySprite.x = x + w
		entitySprite.y = y
		entitySprite.rotation = math.pi / 2
		entitySprite:setJustification(0, 0)
		entitySprite:draw()
	end
	
	if entity.bottomLeft then
		local entitySprite = drawableSprite.fromTexture(texture_border)
		entitySprite.x = x
		entitySprite.y = y + h
		entitySprite.rotation = -math.pi / 2
		entitySprite:setJustification(0, 0)
		entitySprite:draw()
	end
	
	if entity.bottomRight then
		local entitySprite = drawableSprite.fromTexture(texture_border)
		entitySprite.x = x + w
		entitySprite.y = y + h
		entitySprite.rotation = math.pi
		entitySprite:setJustification(0, 0)
		entitySprite:draw()
	end
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity
