local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "LanisTrek/Flag"
entity.texture = "Evidence02/objects_lani/flag/idle00"
entity.color = { 1.0, 0.0, 0.0, 1.0 }


entity.color = function(room, entity) 
	if entity.invisible then 
		return { 0.0, 1.0, 0.0, 1.0 } 
	else 
		return { 1.0, 0.0, 0.0, 1.0 }
	end
end

return entity
