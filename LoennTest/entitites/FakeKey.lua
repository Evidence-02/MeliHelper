local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/FakeKey"
entity.placements = {
    {
        name = "Fake Key",
        data = {
			setFlagOnCollect = "",
			notAppearWhenFlag = "",
			spriteStop = true
		}
    }
}

entity.fieldOrder = { "x", "y", "setFlagOnCollect", "notAppearWhenFlag", "spriteChange", "spriteStop" }

function entity.texture(room, entity)
	return "collectables/key/normal00"
end

return entity
