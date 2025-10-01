local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BattleCityEnemySpawnpoint"
entity.depth = 0
entity.placements = {
    {
        name = "Battle City Enemy Spawnpoint",
        data = {
			order = 0
        }
    }
}


function entity.texture(room, entity)
	return "Evidence02/objects_bc/spawnpoint/idle02"
end

return entity