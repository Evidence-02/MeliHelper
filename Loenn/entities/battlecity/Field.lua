local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local field = {}
field.name = "MeliHelper/BattleCityField"
field.depth = -9999999
field.placements = {
    {
        name = "Battle City Field (extended)",
        data = {
            fieldWidth = 13,
            fieldHeight = 13,
			
			levelID = 1,
			name = "",
			nextLevel = "a-00",
			returnLevel = "a-00",
			
			enemyList = "Bx1,Fx1,Px1,Ax1",
			bonusList = "4,11,18",
			enemyRespawnPeriod = 4,
			enemyRespawnDelay = 1.2,
			enemiesOnScreen = 4,
			backgroundType = "Default",
			
			goal = "Kill enemies",
			finishEvent = "Default",
			
			typeUI = "Default",
			showUI = true,
			showIntro = true
        }
    },
	
    {
        name = "Battle City Field",
        data = {
            fieldWidth = 13,
            fieldHeight = 13,
			levelID = 1,
			name = "",
			nextLevel = "a-00",
			returnLevel = "a-00",
			enemyList = "Bx1,Fx1,Px1,Ax1",
			bonusList = "4,11,18",
			
        }
    }
}

field.fieldOrder = { "x", "y", "fieldWidth", "fieldHeight", "levelID", "name", "nextLevel", "returnLevel", "enemyList", "bonusList", "enemyRespawnPeriod", "enemyRespawnDelay", "enemiesOnScreen", "backgroundType", "goal", "finishEvent", "typeUI", "showUI", "showIntro" }


field.fieldInformation = {
    goal = {
        options = { "Nothing", "Kill enemies" },	-- , "Collect storby"
		editable = false
    },
    finishEvent = {
        options = { "Nothing", "Default", "Fast teleport" },
		editable = false
    },
    backgroundType = {
        options = { "Default", "None" },
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
	levelID = {
		fieldType = "integer",
		minimumValue = -1
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
    
	love.graphics.setColor(0.0, 1.0, 0.0, 0.2)
    for i = 0, height do
		love.graphics.line(x, y + i * cz, x + cz * width, y + i * cz)
    end
    for i = 0, width do
		love.graphics.line(x + i * cz, y, x + i * cz, y + cz * height)
    end
		
    -- reset color
    love.graphics.setColor(r, g, b, a)
	
	-- sprite draw
	local entitySprite = drawableSprite.fromTexture("Evidence02/objects_bc/loenn/grid")
	entitySprite.x = x
	entitySprite.y = y
	entitySprite:draw()
end

return field
