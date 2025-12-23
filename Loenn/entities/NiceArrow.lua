local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/NiceArrow"
entity.placements = {
    {
        name = "Nice Arrow",
        data = {
			colorBorder = "000000",
			colorInside = "FF0000",
			opacityBorder = 1,
			opacityInside = 1,
			colorBorderChangeSpeed = 0,
			colorInsideChangeSpeed = 0,
			direction = "Right",
			angle = 0,
			showInside = true,
			showBorder = true,
        }
    }
}

entity.fieldOrder = { "x", "y",
	"colorBorder", "colorInside", "opacityBorder", "opacityInside",
	"colorBorderChangeSpeed", "colorInsideChangeSpeed",
	"direction", "angle",
	"showBorder", "showInside",
	}

entity.fieldInformation = {
	colorBorder   = { fieldType = "color" },
	colorInside   = { fieldType = "color" },
	opacityBorder = { minimumValue = 0, maximumValue = 1 },
	opacityInside = { minimumValue = 0, maximumValue = 1 },
	direction = { 
		options = { "Left", "Right", "Up", "Down", "Custom" }, 
		editable = false 
	}
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 6, y - 6, 12, 12)
end

function entity.draw(room, entity)
	local x, y = entity.x, entity.y
    local r, g, b, a = love.graphics.getColor()
	local clr_border = entity.colorBorder or '000000'
	local clr_inside = entity.colorInside or 'FF0000'
	local opacity_border = entity.opacityBorder or 1
	local opacity_inside = entity.opacityInside or 1
	local direction = entity.direction or 'Right'
	local angle = (	direction == 'Custom' 	and (math.pi * entity.angle / 180)	or 
					direction == 'Left' 	and math.pi							or 
					direction == 'Right' 	and 0								or 
					direction == 'Up' 		and (3 * math.pi / 2)				or 
					direction == 'Down' 	and (1 * math.pi / 2)				or 
					0
					)
	
	
	local success, tr, tg, tb = utils.parseHexColor(clr_border)
	if entity.showBorder and success then 
		--love.graphics.setColor(tr, tg, tb, opacity_border)
		local entitySprite = drawableSprite.fromTexture('Evidence02/objects_melihelper/nicearrow/border')
		entitySprite.x = x
		entitySprite.y = y
		entitySprite.rotation = angle
		entitySprite:setColor(tr, tg, tb)
		entitySprite:draw()
	end
	
	local success, tr, tg, tb = utils.parseHexColor(clr_inside)
	if entity.showInside and success then 
		--love.graphics.setColor(tr, tg, tb, opacity_inside)
		local entitySprite = drawableSprite.fromTexture('Evidence02/objects_melihelper/nicearrow/inside')
		entitySprite.x = x
		entitySprite.y = y
		entitySprite.rotation = angle
		entitySprite:setColor(tr, tg, tb)
		entitySprite:draw()
	end
		
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity
