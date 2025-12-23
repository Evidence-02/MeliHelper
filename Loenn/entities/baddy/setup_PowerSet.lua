local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/BaddyPowerSet"
entity.placements = {
    {
        name = "Badeline Power Set",
        data = {
			uiTexture = "",
			uiLocation = "BottomLeft",
			
			fullPower = 2,
			shootPower = 1,
			laserPower = 2,
			boostPower = 1.5,
			addMaxPowerOnStrawberryCollect = 0.1,
			addMaxPowerOnGem = 0.2,
			
			shootEnabled = true,
			laserEnabled = true,
			boostEnabled = true,
			affectPlayerSkin = true,
			showUI = true,
			loadOnce = true			
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"uiTexture", "uiLocation",
	"fullPower", "shootPower", "laserPower", "boostPower", "addMaxPowerOnStrawberryCollect", "addMaxPowerOnGem",
	"shootEnabled", "laserEnabled", "boostEnabled", "affectPlayerSkin", "showUI",
	"loadOnce"
}

entity.fieldInformation = { 
	uiLocation = {
		options = { "BottomLeft", "BottomLeftMid", "BottomRightMid", "BottomRight", "TopLeft", "TopLeftMid", "TopRightMid", "TopRight" },
		editable = false
	}
}

function entity.texture(room, entity)
	return "Evidence02/objects_baddy/loenn/powerSet"
end

return entity
