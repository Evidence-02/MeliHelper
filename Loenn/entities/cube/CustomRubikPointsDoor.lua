local drawing = require("utils.drawing")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")

local door = {}
door.name = "MeliHelper/CustomRubikPointsDoor"
door.placements = {
    {
        name = "Custom Rubik Points Door",
        data = {
			width = 32,
			height = 72,
			sprite = "Spiral",
			spriteSize = 12,
			columns = 2,
			
			color = "0000FF",
			colorSpriteFill = "FFFFFF",
			colorSpriteEmpty = "000000",
			delay = 0.03,
			delayClose = 0.01,
			speedOpen = 2,
			speedClose = 3,
			requires = 100,
			spritePoints = 100,
			spritesDY = 0,
			textDY = 8,
			direction = "LeftToRight",
			appear = "All the time",
			showSprites = true
        }
    }
}

door.fieldOrder = { "x", "y", "width", "height", "sprite", "spriteSize", "columns", "color", "colorSpriteFill", "colorSpriteEmpty", 
	"delay", "delayClose", "speedOpen", "speedClose", "requires", "spritePoints", "spritesDY", "textDY", "direction" }

door.fieldInformation = {
	color = { fieldType = "color" },
	colorSpriteFill = { fieldType = "color" },
	colorSpriteEmpty = { fieldType = "color" },
	sprite = {
		options = { "Spiral", "Heart full", "Heart empty", "Heart big", "Strawberry" },
		editable = false
	},
	direction = {
		options = { "LeftToRight", "RightToLeft" }, 
		editable = false
	},
	appear = {
		options = {	"All the time", "Golden only", "Not golden only" },
		editable = false
	}
}

function door.selection(room, entity)
    return utils.rectangle(entity.x or 0, entity.y or 0, entity.width or 32, entity.height or 72)
end

function door.draw(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local w, h = entity.width or 32, entity.height or 72
    local requires = entity.requires or 100
	local cols = entity.columns or 2
	
	local r, g, b, a = love.graphics.getColor()
    
	love.graphics.setColor(0.0, 0.0, 1.0, 1.0)
	love.graphics.rectangle("fill", x, y, w, h)
	
    love.graphics.setColor(1.0, 1.0, 1.0, 1.0)
	
	
	local entitySprite = drawableSprite.fromTexture("objects/heartdoor/edge")
	for row = 0, h / 8 - 1 do 
		entitySprite.x = entity.x + 4
		entitySprite.y = entity.y + row * 8 + 4
		entitySprite:setScale(-1, 1)
		entitySprite:draw()
		
		entitySprite.x = entity.x + w - 4
		entitySprite.y = entity.y + row * 8 + 4
		entitySprite:setScale(1, 1)
		entitySprite:draw()
	end

	--[[
	local hearts = requires / 10
	local entitySprite = drawableSprite.fromTexture("objects/heartdoor/icon00")
	local rows = math.ceil(hearts / cols)
	for hearts = 0, cnt_hearts-1 do 
		local r = math.floor(hearts / cols)
		local c = hearts % cols
	
		entitySprite.x = entity.x + w / 2 + (c - cols / 2 + 0.5) * 12
		entitySprite.y = entity.y + h / 2 + (r - rows / 2 + 0.5) * 12
		entitySprite:draw()
	end
	]]--
	
	
	
	-- requires
    local font = love.graphics.getFont()
    local textBatch = love.graphics.newText(font)
    drawing.callKeepOriginalColor(function()
		love.graphics.setColor(1.0, 1.0, 1.0, 1.0)
		drawing.printCenteredText('POINTS: ' .. requires, x, y - 4, w, h, font, 1)
		drawing.printCenteredText('CURRENT: 0', x, y + 4, w, h, font, 1)
	end)
	
	
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return door
