
local entity = {}
entity.name = "MeliHelper/RubikSwitchTexture"
entity.depth = -9999999
entity.placements = {
    {
        name = "Rubik Switch Texture",
        data = {
			texture = "",
			button = "Talk",
			inverted = false
        }
    }
}

entity.fieldInformation = {
	button = {
		options = {	"Talk" },
		editable = false
	}
}

function entity.texture(room, entity)
	return entity.texture
end

return entity
