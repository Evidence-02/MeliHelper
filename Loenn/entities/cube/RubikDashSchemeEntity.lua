local drawableSprite = require("structs.drawable_sprite")

local entity = {}
entity.name = "MeliHelper/RubikDashSchemeEntity"
entity.depth = 0
entity.placements = {
    {
        name = "Rubik Dash Scheme Entity",
        data = {
			name = "",
			scheme = "Main",
			dashUp = "None",
			dashDown = "None",
			dashLeft = "None",
			dashRight = "None",
			dashUpLeft = "None",
			dashUpRight = "None",
			dashDownLeft = "None",
			dashDownRight = "None",
			dashUpDirection = "Default",
			dashDownDirection = "Default",
			dashLeftDirection = "Default",
			dashRightDirection = "Default",
			dashUpLeftDirection = "Default",
			dashUpRightDirection = "Default",
			dashDownLeftDirection = "Default",
			dashDownRightDirection = "Default"
        }
    },
    {
        name = "Rubik Dash Scheme Entity (default)",
        data = {
			name = "Clockwise",
			scheme = "Main",
			dashUp = "Up",
			dashDown = "Down",
			dashLeft = "Left",
			dashRight = "Right",
			dashUpLeft = "Front",
			dashUpRight = "None",
			dashDownLeft = "None",
			dashDownRight = "Back",
			dashUpDirection = "Default",
			dashDownDirection = "Default",
			dashLeftDirection = "Default",
			dashRightDirection = "Default",
			dashUpLeftDirection = "Default",
			dashUpRightDirection = "Default",
			dashDownLeftDirection = "Default",
			dashDownRightDirection = "Default"
        }
    },
    {
        name = "Rubik Dash Scheme Entity (default, clockwise)",
        data = {
			name = "Clockwise",
			scheme = "Main",
			dashUp = "Up",
			dashDown = "Down",
			dashLeft = "Left",
			dashRight = "Right",
			dashUpLeft = "Front",
			dashUpRight = "None",
			dashDownLeft = "None",
			dashDownRight = "Back",
			dashUpDirection = "Clockwise",
			dashDownDirection = "Clockwise",
			dashLeftDirection = "Clockwise",
			dashRightDirection = "Clockwise",
			dashUpLeftDirection = "Clockwise",
			dashUpRightDirection = "Default",
			dashDownLeftDirection = "Default",
			dashDownRightDirection = "Clockwise"
        }
    },
    {
        name = "Rubik Dash Scheme Entity (default, otherwise)",
        data = {
			name = "Otherwise",
			scheme = "Main",
			dashUp = "Up",
			dashDown = "Down",
			dashLeft = "Left",
			dashRight = "Right",
			dashUpLeft = "Front",
			dashUpRight = "None",
			dashDownLeft = "None",
			dashDownRight = "Back",
			dashUpDirection = "Otherwise",
			dashDownDirection = "Otherwise",
			dashLeftDirection = "Otherwise",
			dashRightDirection = "Otherwise",
			dashUpLeftDirection = "Otherwise",
			dashUpRightDirection = "Default",
			dashDownLeftDirection = "Default",
			dashDownRightDirection = "Otherwise"
        }
    },
    {
        name = "Rubik Dash Scheme Entity (axises)",
        data = {
			name = "Axises",
			scheme = "Scheme1",
			dashUp = "AxisX",
			dashDown = "AxisX",
			dashLeft = "AxisY",
			dashRight = "AxisY",
			dashUpLeft = "None",
			dashUpRight = "None",
			dashDownLeft = "None",
			dashDownRight = "None",
			dashUpDirection = "Otherwise",
			dashDownDirection = "Clockwise",
			dashLeftDirection = "Clockwise",
			dashRightDirection = "Otherwise",
			dashUpLeftDirection = "Default",
			dashUpRightDirection = "Default",
			dashDownLeftDirection = "Default",
			dashDownRightDirection = "Default"
        }
    },
    {
        name = "Rubik Dash Scheme Entity (diagonal)",
        data = {
			name = "diagonal",
			scheme = "Main",
			dashUp = "AxisX",
			dashDown = "AxisX",
			dashLeft = "AxisY",
			dashRight = "AxisY",
			dashUpLeft = "UpLeft",
			dashUpRight = "UpRight",
			dashDownLeft = "DownLeft",
			dashDownRight = "DownRight",
			dashUpDirection = "Otherwise",
			dashDownDirection = "Clockwise",
			dashLeftDirection = "Clockwise",
			dashRightDirection = "Otherwise",
			dashUpLeftDirection = "Default",
			dashUpRightDirection = "Default",
			dashDownLeftDirection = "Default",
			dashDownRightDirection = "Default"
        }
    }
}

entity.fieldOrder = { 
	"x", "y", 
	"scheme", "name", 
	"dashUp", "dashUpDirection", 
	"dashDown", "dashDownDirection", 
	"dashLeft", "dashLeftDirection", 
	"dashRight", "dashRightDirection", 
	"dashUpLeft", "dashUpLeftDirection", 
	"dashUpRight", "dashUpRightDirection", 
	"dashDownLeft", "dashDownLeftDirection", 
	"dashDownRight", "dashDownRightDirection"
}

local standartDashAction = {
	options = {	
		"Front", "Back", "Up", "Down", "Left", "Right", "UpLeft", "UpRight", "DownLeft", "DownRight", "AxisX", "AxisY", "AxisZ", "RotateCenter", 
		"None",
		"Change direction", "Go to main scheme", "Go to scheme 1", "Go to scheme 2", "Go to scheme 3"
	},
	editable = false
}

local standartDirectionAction = {
	options = {	"Default", "Clockwise", "Otherwise" },
	editable = false
}

entity.fieldInformation = {
    dashUp = standartDashAction,
    dashDown = standartDashAction,
    dashLeft = standartDashAction,
    dashRight = standartDashAction,
    dashUpLeft = standartDashAction,
    dashUpRight = standartDashAction,
    dashDownLeft = standartDashAction,
    dashDownRight = standartDashAction,
    dashUpDirection = standartDirectionAction,
    dashDownDirection = standartDirectionAction,
    dashLeftDirection = standartDirectionAction,
    dashRightDirection = standartDirectionAction,
    dashUpLeftDirection = standartDirectionAction,
    dashUpRightDirection = standartDirectionAction,
    dashDownLeftDirection = standartDirectionAction,
    dashDownRightDirection = standartDirectionAction,
	scheme = {
		options = {	"Main", "Scheme1", "Scheme2", "Scheme3" },
		editable = false
	}
}

function entity.texture(room, entity)
	return "Evidence02/objects_cubes/loenn/dash_scheme"
end

return entity
