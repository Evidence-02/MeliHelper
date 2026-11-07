
local entity = {}
entity.name = "MeliHelper/SeedComputer"
entity.placements = {
    {
        name = "Seed Computer",
        data = {
			seedColor = "FFBF7F"
        }
    }
}

entity.fieldInformation = {
	seedColor = { fieldType = "color" }
}

function entity.texture(room, entity)
	return "Evidence02/objects_cubes/seed_computer/idle00"
end

return entity
