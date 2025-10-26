local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/LaniSnowball"
entity.placements = {
    {
        name = "Lani Snowball",
        data = {
			sprite = "MeliHelper_LaniSnowball",
			bounces = 8,
			jumps = 6
        }
    }
}
entity.texture = "Evidence02/objects_lani/loenn/snowball"
entity.fieldOrder = { "x", "y", "sprite", "bounces", "jumps" }

return entity
