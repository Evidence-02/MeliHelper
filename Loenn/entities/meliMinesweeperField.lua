local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local field = {}
field.name = "MeliHelper/MinesweeperField"
field.depth = -9999999
field.placements = {
    {
        name = "Minesweeper Field",
        data = {
            fieldWidth = 10,
            fieldHeight = 10,
			bombs = 10,
			roomTeleportOnWin = "a-00",
			maxStartCell = 1,
			diagonalDashesOpenCells = "OnlyThroughFlags",
			placePlayerInside = false
		}
    },
}

field.fieldOrder = { "x", "y", "fieldWidth", "fieldHeight", "bombs", "roomTeleportOnWin", "maxStartCell", "diagonalDashesOpenCells", "placePlayerInside" }


field.fieldInformation = {
	diagonalDashesOpenCells = {
		options = { "None", "Always", "OnlyThroughFlags" },
		editable = false
	},
	fieldWidth = {
		fieldType = "integer",
		minimumValue = 1
	},
	fieldHeight = {
		fieldType = "integer",
		minimumValue = 1
	},
	bombs = {
		fieldType = "integer",
		minimumValue = 1
	},
	maxStartCell = {
		fieldType = "integer",
		minimumValue = 0,
		maximumValue = 8
	},
}


function field.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function field.draw(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.fieldWidth or 10, entity.fieldHeight or 16
	local cz = 16
    
	local r, g, b, a = love.graphics.getColor()
    
	love.graphics.setColor(1.0, 0.0, 0.0, 0.2)
    for i = 0, height do
		love.graphics.line(x, y + i * cz, x + cz * width, y + i * cz)
    end
    for i = 0, width do
		love.graphics.line(x + i * cz, y, x + i * cz, y + cz * height)
    end
		
    -- reset color
    love.graphics.setColor(r, g, b, a)
	
	-- sprite draw
	local entitySprite = drawableSprite.fromTexture("Evidence02/objects_melihelper/loenn/gridM")
	entitySprite.x = x
	entitySprite.y = y
	entitySprite:draw()
end

return field
