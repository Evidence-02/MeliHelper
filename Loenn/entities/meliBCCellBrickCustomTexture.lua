local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BattleCityCellBrickCustomTexture"
entity.depth = 0
entity.placements = {
    {
        name = "Battle City Cell Brick Custom Texture",
        data = {
			texture = ""
		}
    }
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

function entity.texture(room, entity)
    return (entity.texture or '')
end

return entity
