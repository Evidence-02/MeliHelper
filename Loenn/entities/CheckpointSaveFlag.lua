local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/CheckpointSaveFlag"
entity.placements = {
    {
        name = "Checkpoint Save Flag",
        data = {
			sprite = "MeliHelper_CheckpointSaveFlag",
			createConfetti = true
        }
    }
}

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/checkpoint_flag/idle00"
end

return entity
