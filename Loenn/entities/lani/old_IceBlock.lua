local fakeTilesHelper = require("helpers.fake_tiles")

local entity = {}
entity.name = "LanisTrek/IceBlock"
entity.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")
entity.depth = -9999999
entity.placements = {
}

function entity.draw(room, entity)
    local r, g, b, a = love.graphics.getColor()
    
	love.graphics.setColor(1.0, 0.0, 0.0, 0.3)
    love.graphics.rectangle("fill", entity.x + 0.5, entity.y + 0.5, entity.width - 1, entity.height - 1)
	love.graphics.setColor(1.0, 0.0, 0.0, 0.5)
    love.graphics.rectangle("line", entity.x + 0.5, entity.y + 0.5, entity.width - 1, entity.height - 1)
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity