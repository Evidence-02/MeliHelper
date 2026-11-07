local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/CustomRubikBackground"
entity.depth = -9999999
entity.placements = {
    {
        name = "Custom Rubik Background",
        data = {
			width = 8,
			height = 8,
			colorUp = "FFFFFF",
			colorDown = "FFFF00",
			colorLeft = "FF8800",
			colorRight = "FF0000",
			colorFront = "00FF00",
			colorBack = "0000FF",
			alpha = 0.2,
			depth = -9999999,
			appear = "All the time"
        }
    }
}

entity.fieldOrder = { "x", "y", "width", "height", 
	"colorUp", "colorDown", "colorLeft", "colorRight", "colorFront", "colorBack",
	"alpha", "depth", "appear" }

entity.fieldInformation = {
	colorUp    = { fieldType = "color" },
	colorDown  = { fieldType = "color" },
	colorLeft  = { fieldType = "color" },
	colorRight = { fieldType = "color" },
	colorFront = { fieldType = "color" },
	colorBack  = { fieldType = "color" },
	appear = {
		options = {	"All the time", "Golden only", "Not golden only" },
		editable = false
	}
}

function entity.draw(room, entity)
    local x, y, w, h = entity.x or 0, entity.y or 0, entity.width or 8, entity.height or 8
	local clr = entity.colorDown or 'FFFF00'
	local alpha = entity.alpha or 0.2
	
    local r, g, b, a = love.graphics.getColor()
	local success, tr, tg, tb = utils.parseHexColor(clr)
	if success then 
		love.graphics.setColor(tr, tg, tb, alpha)
		love.graphics.rectangle("fill", entity.x + 0.5, entity.y + 0.5, entity.width - 1, entity.height - 1)
	end
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity