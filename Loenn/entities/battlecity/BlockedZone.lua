local jautils = require("mods").requireFromPlugin("libraries.jautils")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BattleCityBlockedZone"
entity.depth = -9999999
entity.placements = {
    {
        name = "Battle City Blocked Zone",
        data = {
			width = 16,
			height = 16
        }
    }
}

function entity.draw(room, entity)
    local r, g, b, a = love.graphics.getColor()
    
	love.graphics.setColor(1.0, 1.0, 1.0, 0.3)
    love.graphics.rectangle("fill", entity.x + 0.5, entity.y + 0.5, entity.width - 1, entity.height - 1)
	love.graphics.setColor(1.0, 1.0, 1.0, 0.6)
    love.graphics.rectangle("line", entity.x + 0.5, entity.y + 0.5, entity.width - 1, entity.height - 1)
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity
