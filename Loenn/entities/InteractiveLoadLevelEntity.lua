local jautils = require("mods").requireFromPlugin("libraries.jautils")
local drawing = require("utils.drawing")
local utils = require("utils")

local entity = {}
entity.name = "MeliHelper/InteractiveLevelLoadEntity"
entity.placements = {
    {
        name = "Interactive Room Loader (line)",
        data = {
			width = 16,
			height = 16,
			levelLoad = "a-00",
			introTypes = "WakeUp",
			spawnpointX = 0,
			spawnpointY = 0,
			colorBorder = "FFFFFF",
			colorBack = "000000",
			colorFill = "FF8080",
			valueAdd = 0.18,
			valueSubPerSecond = 0.5,
			delayBeforeSub = 1.4,
			text = "",
			colorText = "AAAAAA",
			dialogueBefore = "",
			dashAffective = true
			
        }
    }
}

entity.fieldOrder = { "x", "y", "width", "height", 
	"levelLoad", "introTypes", "spawnpointX", "spawnpointY",
	"colorBorder", "colorBack", "colorFill", "valueAdd", "valueSubPerSecond", "delayBeforeSub", 
	"text", "colorText", "dialogueBefore", "dashAffective"
	}

entity.fieldInformation = {
    introTypes = {
		options = { "Respawn", "WalkInRight", "WalkInLeft", "Jump", "WakeUp", "Fall", "TempleMirrorVoid", "None", "ThinkForABit" },
		editable = false
    },
	spawnpointX = { fieldType = "integer" },
	spawnpointY = { fieldType = "integer" },
	colorBorder = { fieldType = "color" },
	colorBack = { fieldType = "color" },
	colorFill = { fieldType = "color" },
	colorText = { fieldType = "color" }
}

function entity.draw(room, entity)
	local x, y, w, h = entity.x, entity.y, entity.width or 48, entity.height or 16
    local r, g, b, a = love.graphics.getColor()
	
	local success, tr, tg, tb = utils.parseHexColor(entity.colorBack or '000000')
    love.graphics.setColor(tr, tg, tb, 1.0)
    love.graphics.rectangle("fill", x + 0.5, y + 0.5, w - 1, h - 1)
	
	local success, tr, tg, tb = utils.parseHexColor(entity.colorFill or 'FF8080')
    love.graphics.setColor(tr, tg, tb, 1.0)
    love.graphics.rectangle("fill", x + 1.5, y + 1.5, w / 3 - 3, h - 3)
	
	local success, tr, tg, tb = utils.parseHexColor(entity.colorBorder or 'FFFFFF')
	love.graphics.setColor(tr, tg, tb, 1.0)
    love.graphics.rectangle("line", x + 0.5, y + 0.5, w - 1, h - 1)
	
    love.graphics.rectangle("line", x + 0.5, y + 0.5, w - 1, h - 1)
	
	-- additional text
	local text = entity.text or ''
	if text ~= '' then
		local font = love.graphics.getFont()
		local textBatch = love.graphics.newText(font)
		drawing.callKeepOriginalColor(function()
			local success, tr, tg, tb = utils.parseHexColor(entity.colorText or 'AAAAAA')
			love.graphics.setColor(tr, tg, tb, 1.0)
			drawing.printCenteredText(text, x, y, w, h, font, 1)
		end)
		love.graphics.draw(textBatch)
	end
	
	
    -- reset color
    love.graphics.setColor(r, g, b, a)
end

return entity
