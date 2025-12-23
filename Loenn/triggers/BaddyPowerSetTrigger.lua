local trigger = {}
trigger.name = "MeliHelper/BaddyPowerSetTrigger"
trigger.placements = {
    {
        name = "Badeline Power Set Trigger",
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
			oneUse = true,
			clearOnLeave = false
        }
    }
}

trigger.fieldOrder = { "x", "y", "width", "height",
	"uiTexture", "uiLocation",
	"fullPower", "shootPower", "laserPower", "boostPower", "addMaxPowerOnStrawberryCollect", "addMaxPowerOnGem",
	"shootEnabled", "laserEnabled", "boostEnabled", "affectPlayerSkin", "showUI",
	"oneUse", "clearOnLeave"
}

trigger.fieldInformation = {
    hookDirection = {
		options = { "Horizontal", "All directions" },
		editable = false
    },
	hookColor = { fieldType = "color" }
}

return trigger
