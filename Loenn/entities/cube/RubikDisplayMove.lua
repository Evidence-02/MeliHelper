local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local display = {}
display.name = "MeliHelper/RubikDisplayMove"
display.depth = 0
display.placements = {
    {
        name = "Rubik Move Display",
        data = {
			width = 88,
			height = 152,
			texture = "decals/Evidence02/RCM/displayMovesDefault",
			colorTextTitle = "ff8300",
			colorTextMoves = "292929",
			colorTextMovesUnreg = "E5000B",
			columns = 6,
			titleY = 3,
			colX = 3,
			colY = 13,
			colDX = 13,
			colDY = 6,
			bottomMargin = 12
        }
    }
}

display.fieldOrder = { "x", "y", "width", "height", "texture", "colorTextTitle", "colorTextMoves", "colorTextMovesUnreg", "columns", "titleY", "colX", "colY", "colDX", "colDY", "bottomMargin" }

display.fieldInformation = {
    colorBackground = { fieldType = "color" },
    colorBorder = { fieldType = "color" },
    colorBorderLines = { fieldType = "color" },
    colorTextTitle = { fieldType = "color" },
    colorTextMoves = { fieldType = "color" },
    colorTextMovesUnreg = { fieldType = "color" },
}

function display.draw(room, entity)
	
	local x, y, w, h = entity.x, entity.y, entity.width, entity.height
    local r, g, b, a = love.graphics.getColor()
	local texture = entity.texture or "decals/Evidence02/RCM/displayMovesDefault"
    	
	love.graphics.setColor(0.0, 0.0, 0.8, 0.3)
    love.graphics.rectangle("fill", x + 0.5, y + 0.5, w - 1, h - 1)
	love.graphics.setColor(0.0, 0.0, 0.8, 0.5)
    love.graphics.rectangle("line", x + 0.5, y + 0.5, w - 1, h - 1)
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
	
	-- draw texture
	local entitySprite = drawableSprite.fromTexture(texture)
	entitySprite.x = x
	entitySprite.y = y
	entitySprite:setJustification(0, 0)
	entitySprite:draw()
	
	-- title and moves
    local font = love.graphics.getFont()
    local textBatch = love.graphics.newText(font)
    drawing.callKeepOriginalColor(function()
		
		local success, tr, tg, tb = utils.parseHexColor(entity.colorTextTitle or 'FFFFFF')
		if success then
			love.graphics.setColor(tr, tg, tb, 1.0)
			drawing.printCenteredText('TITLE', x + w / 2 - 11, y + (entity.titleY or 3) - 1, 20, 5, font, 1)
		end
		
		local success, tr, tg, tb = utils.parseHexColor(entity.colorTextMoves or 'FFFFFF')
		if success then
			love.graphics.setColor(tr, tg, tb, 1.0)
			drawing.printCenteredText('MOVES: R2 L2 U', x + (entity.colX or 3) - 1, y + (entity.colY or 13) - 1, 56, 5, font, 1)
		end
	end)
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return display
