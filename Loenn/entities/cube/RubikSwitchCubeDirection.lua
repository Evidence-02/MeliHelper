
local entity = {}
entity.name = "MeliHelper/RubikSwitchCubeDirection"
entity.depth = -9999999
entity.placements = {
    {
        name = "Rubik Switch Cube Direction",
        data = {
			size = 0.7,
			button = "Talk"
        }
    }
}

entity.fieldInformation = {
	button = { 
		options = { "Talk" }, 
		editable = false
	}
}

function entity.texture(room, entity)
	return "@Internal@/core_message"
end

return entity
