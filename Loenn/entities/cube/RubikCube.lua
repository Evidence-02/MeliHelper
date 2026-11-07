local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/RubikCube"
entity.placements = {
    {
        name = "Rubik Cube",
        data = {
			cubeType = "3x3x3",
			cubeSize = 50,		
			scrambleRotates = 50,
			scrambleRotateTime = 0.08,
			rotateTime = 0.4,
			delayScramble = 1.2,
			pointsOnSolve = 100,
			cubeID = "",
			
			colorUp = "FFFFFF",
			colorDown = "FFFF00",
			colorLeft = "FF8800",
			colorRight = "FF0000",
			colorFront = "00FF00",
			colorBack = "0000FF",
			colorBorders = "222222",
			colorInside  = "A4A4A4",
			generateBorders = true,
			generateInsides = false,
			flickering = false
        }
    }
}

entity.fieldOrder = { "x", "y", 
	"cubeType", "cubeSize", "scrambleRotates", "scrambleRotateTime", "rotateTime", "delayScramble", "pointsOnSolve", "cubeID", 
	"colorUp", "colorDown", "colorLeft", "colorRight", "colorFront", "colorBack",
	"colorBorders", "colorInside", "generateBorders", "generateInsides", "flickering"
}

entity.fieldInformation = {
	colorUp    = { fieldType = "color" },
	colorDown  = { fieldType = "color" },
	colorLeft  = { fieldType = "color" },
	colorRight = { fieldType = "color" },
	colorFront = { fieldType = "color" },
	colorBack  = { fieldType = "color" },
	colorBorders = { fieldType = "color" },
	colorInside  = { fieldType = "color" },
	cubeType = {
		options = {	"2x2x2", "2x2x3", "2x3x3", "3x3x3", "Mirror 2x2", "Mirror 3x3", "Mirror Floppy",
			"Floppy Cube", "Dino Cube", "Skewb", "Clover Cube", "Petal Pyraminx", "Kilominx", "Megaminx"
			},
		editable = false
	}
}

function entity.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 12, y - 12, 24, 24)
end

function entity.texture(room, entity)
	return "Evidence02/objects_cubes/loenn/cube"
end

return entity
