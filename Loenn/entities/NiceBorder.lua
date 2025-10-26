local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/NiceBorder"
entity.placements = {
    {
        name = "Nice Border",
        data = {
			width = 128,
			height = 64,
			colorBorder = "000000",
			colorInside = "00FF00",
			opacityBorder = 1,
			opacityInside = 1,
			borderLeft = true,
			borderRight = true,
			borderTop = true,
			borderBottom = true
        }
    }
}

entity.fieldOrder = { "x", "y", "width", "height", 
	"colorBorder", "colorInside", "opacityBorder", "opacityInside",
	"borderLeft", "borderRight", "borderTop", "borderBottom"
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
	
	local success, tr, tg, tb = utils.parseHexColor(clr_inside)
	if success then
		love.graphics.setColor(tr, tg, tb, opacity_inside)
		love.graphics.rectangle("fill", x + 0.5, y + 0.5, w - 1, h - 1)
	end
	
	local success, tr, tg, tb = utils.parseHexColor(clr_border)
	if success then
		love.graphics.setColor(tr, tg, tb, opacity_border)
		if entity.borderLeft then
			love.graphics.line(x + 0.5, y + 0.5, x + 0.5, y + h - 0.5)
		end
		if entity.borderRight then
			love.graphics.line(x + w - 0.5, y + 0.5, x + w - 0.5, y + h - 0.5)
		end
		if entity.borderTop then
			love.graphics.line(x + 0.5, y + 0.5, x + w - 0.5, y + 0.5)
		end
		if entity.borderBottom then
			love.graphics.line(x + 0.5, y + h - 0.5, x + w - 0.5, y + h - 0.5)
		end
	end
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity
