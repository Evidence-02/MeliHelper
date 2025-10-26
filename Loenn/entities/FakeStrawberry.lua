local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/FakeStrawberry"
entity.placements = {
    {
        name = "Fake Strawberry",
        data = {
			setFlagOnCollect = "",
			notAppearWhenFlag = "",
			spriteChange = true,
			spriteStop = true
		}
    }
}

entity.fieldOrder = { "x", "y", "setFlagOnCollect", "notAppearWhenFlag", "spriteChange", "spriteStop" }

function entity.texture(room, entity)
	return "collectables/strawberry/normal05"
end

return entity
