local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/MinesweeperFlagGUI"
entity.placements = {
    {
        name = "Minesweeper Flag GUI",
        data = {
			messageOnFlagMode = "",
			messageOnNormalMode = "",
			size = 1.0,
			koefFloating = 0.2,
			distanceAppear = 66,
			distanceDisappear = 114,
			appearJustLikeCoreMessage = true
        }
    },
    {
        name = "Minesweeper Flag GUI (static)",
        data = {
			messageOnFlagMode = "",
			messageOnNormalMode = "",
			size = 1.0,
			koefFloating = 0,
			distanceAppear = 0,
			distanceDisappear = 0,
			appearJustLikeCoreMessage = false
        }
    }
}

entity.fieldOrder = { "x", "y", "messageOnFlagMode", "messageOnNormalMode", "size", "koefFloating", "distanceAppear", "distanceDisappear", "appearJustLikeCoreMessage" }

function entity.texture(room, entity)
	return "@Internal@/core_message"
end

return entity
