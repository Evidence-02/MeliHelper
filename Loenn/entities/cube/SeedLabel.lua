
local entity = {}
entity.name = "MeliHelper/SeedLabel"
entity.depth = -9999999
entity.placements = {
    {
        name = "Seed Label",
        data = {
			size = 0.3
        }
    }
}

function entity.texture(room, entity)
	return "@Internal@/core_message"
end

return entity
