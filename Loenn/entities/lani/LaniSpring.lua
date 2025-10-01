local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniSpring"
entity.placements = {
    {
        name = "Lani Spring",
        data = {
			sprite = "MeliHelper_LaniSpring",
			koefMultX = 0.6,
			speedY = 320,
			speedYWithJump = 420,
			refillDash = true,
			refillStamina = false,
			destroyCrumblePlatformUnder = true
        }
    },
	
	
    {
        name = "Lani Spring (empty, delete later)",
        data = {
        }
    }
}
entity.texture = "Evidence02/objects_lani/obj_spring/idle00"
entity.fieldOrder = { "x", "y", "sprite", "koefMultX", "speedY", "speedYWithJump", "refillDash", "refillStamina", "destroyCrumblePlatformUnder" }

return entity
