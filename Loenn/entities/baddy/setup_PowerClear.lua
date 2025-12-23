local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BaddyPowerClear"
entity.placements = {
    {
        name = "Badeline Power Clear",
        data = {
			loadOnce = true
        }
    }
}

function entity.texture(room, entity)
	return "Evidence02/objects_baddy/loenn/powerClear"
end

return entity
