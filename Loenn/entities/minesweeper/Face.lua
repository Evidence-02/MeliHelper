local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/MinesweeperFace"
entity.depth = 0
entity.placements = {
    {
        name = "Minesweeper Face (normal)",
        data = {
			textureNormal = "Evidence02/objects_melihelper/minesweeper/faceNormalHappy",
			textureGameover = "Evidence02/objects_melihelper/minesweeper/faceNormalGameover",
			textureWin = "Evidence02/objects_melihelper/minesweeper/faceNormalWin"
        }
    },
    {
        name = "Minesweeper Face (isaac)",
        data = {
			textureNormal = "Evidence02/objects_melihelper/minesweeper/faceIsaacHappy",
			textureGameover = "Evidence02/objects_melihelper/minesweeper/faceIsaacGameover",
			textureWin = "Evidence02/objects_melihelper/minesweeper/faceIsaacWin"
        }
    }
}


function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 12, y - 12, 24, 24)
end

function entity.texture(room, entity)
	return entity.textureNormal
end

return entity