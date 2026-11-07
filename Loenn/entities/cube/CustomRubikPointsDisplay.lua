local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/CustomRubikPointsDisplay"
entity.placements = {
    {
        name = "Custom Rubik Points Display",
        data = {
			digits = 7,
			spacing = 6,
			colorText = "FFFFFF",
			depth = -9999999
        }
    }
}

entity.fieldInformation = {
	digits = {
		fieldType = "integer",
		minimumValue = 1
	},
	spacing = {
		fieldType = "integer",
		minimumValue = 1
	},
	colorText = { fieldType = "color" }
}

function entity.selection(room, entity)
    local x, y, w = entity.x or 0, entity.y or 0, (entity.digits or 7) * (entity.spacing or 6)
    return utils.rectangle(x, y, w, 9)
end

function entity.draw(room, entity)
    local x, y, w, spac = entity.x or 0, entity.y or 0, entity.digits or 7, entity.spacing or 6
    local r, g, b, a = love.graphics.getColor()
	local clr = entity.colorText or 'FFFFFF'
	
	local success, tr, tg, tb = utils.parseHexColor(clr)
	if success then 
		for i = 0, w - 1 do 
			-- texture size: width=5, height=9
			local entitySprite = drawableSprite.fromTexture("Evidence02/objects_cubes/display/num00")
			entitySprite.x = x + spac * i + 2
			entitySprite.y = y + 4
			entitySprite:setColor(tr, tg, tb)
			entitySprite:draw()
		end
	end
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity