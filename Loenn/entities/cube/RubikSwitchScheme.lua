
local entity = {}
entity.name = "MeliHelper/RubikSwitchScheme"
entity.depth = -9999999
entity.placements = {
    {
        name = "Rubik Switch Scheme",
        data = {
			size = 0.7,
			button = "Talk",
			whenPressed = "Main",
			whenReleased = "Scheme1"
        }
    }
}

entity.fieldInformation = {
	button = { 
		options = { "Talk" }, 
		editable = false
	},
	whenPressed = {
		options = {	"Main", "Scheme1", "Scheme2", "Scheme3" },
		editable = false
	},
	whenReleased = {
		options = {	"Main", "Scheme1", "Scheme2", "Scheme3" },
		editable = false
	}
}

function entity.texture(room, entity)
	return "@Internal@/core_message"
end

return entity
