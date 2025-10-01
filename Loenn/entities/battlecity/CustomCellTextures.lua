local entity = {}
entity.name = "MeliHelper/BattleCityCustomCellTextures"
entity.placements = {
    {
        name = "Battle City Custom Cell Textures",
        data = {
			brick   = "Evidence02/objects_bc/tiles/brick01",
			brick02 = "Evidence02/objects_bc/tiles/brick02",
			steel = "Evidence02/objects_bc/tiles/steel00"
			grass = "Evidence02/objects_bc/tiles/grass00",
			water = "Evidence02/objects_bc/tiles/water00",
			dirt  = "Evidence02/objects_bc/tiles/dirt00"
		}
    }
}

entity.fieldOrder = { "x", "y", "brick", "brick02", "steel", "grass", "water", "dirt" }

function entity.texture(room, entity)
    return "Evidence02/objects_bc/loenn/custom_textures"
end

return entity
