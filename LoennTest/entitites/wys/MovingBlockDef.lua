local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/WYSMovingBlockDef"
entity.placements = {
    {
        name = "WYS Moving Block (default, horizontal)",
        data = {
			direction = "Horizontal",
			color = "FF6600",
			killPlayerOnMoving = false
        }
    },
    {
        name = "WYS Moving Block (default, vertical)",
        data = {
			direction = "Vertical",
			color = "FF6600",
			killPlayerOnMoving = false
        }
    }
}

entity.fieldOrder = { "x", "y",
	"direction", "color", "killPlayerOnMoving"
}

entity.fieldInformation = {
    direction = {
        options = { "Up", "Down", "Left", "Right", "Vertical", "Horizontal", "Any" },
		editable = false
    },
	color = { fieldType = "color" },
}

function entity.selection(room, entity)
	local x, y, w, h = entity.x, entity.y, entity.width or 16, entity.height or 16
    return utils.rectangle(x, y, w, h)
end

function entity.draw(room, entity)
	local x, y, w, h = entity.x, entity.y, entity.width or 16, entity.height or 16
    local r, g, b, a = love.graphics.getColor()
	
	local success, tr, tg, tb = utils.parseHexColor(entity.colorInside or '181818')
    love.graphics.setColor(tr, tg, tb, 1.0)
    love.graphics.rectangle("fill", x + 0.5, y + 0.5, w - 1, h - 1)
	
	local success, tr, tg, tb = utils.parseHexColor(entity.color or 'FF6600')
	love.graphics.setColor(tr, tg, tb, 1.0)
    love.graphics.rectangle("line", x + 1.5, y + 1.5, w - 3, h - 3)
	
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity
