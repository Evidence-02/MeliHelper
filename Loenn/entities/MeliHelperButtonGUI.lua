local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/MeliHelperButtonGUI"
entity.placements = {
    {
        name = "Meli Helper Button GUI",
        data = {
			button = "BattleCity_Shoot",
			size = 1.0,
			koefFloating = 0.2,
			distanceAppear = 66,
			distanceDisappear = 114,
			appearJustLikeCoreMessage = true
        }
    },
    {
        name = "Meli Helper Button GUI (static)",
        data = {
			button = "BattleCity_Shoot",
			size = 1.0,
			koefFloating = 0,
			distanceAppear = 0,
			distanceDisappear = 0,
			appearJustLikeCoreMessage = false
        }
    }
}

entity.fieldOrder = { "x", "y", "button", "size", "koefFloating", "distanceAppear", "distanceDisappear", "appearJustLikeCoreMessage" }

entity.fieldInformation = {
    button = {
		options = { "BattleCity_Shoot", "Minesweeper_ChangeDashMode", "BadelinePower_Switch" },
		editable = false
    }
}

function entity.texture(room, entity)
	return "Evidence02/objects_melihelper/loenn/buttonGUI"
end

return entity
