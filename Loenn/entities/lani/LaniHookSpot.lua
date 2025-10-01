local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniHookSpot"
entity.placements = {
    {
        name = "Lani Hook Spot",
        data = {
			sprite = "MeliHelper_LaniHookSpot",
			radiusHook = 8,
			radiusLight = 48
        }
    }
}

entity.texture = "Evidence02/objects_lani/hook_spot/idle00"
entity.fieldInformation = {
	radiusHook = { fieldType = "integer" },
	radiusLight = { fieldType = "integer" }
}



return entity
