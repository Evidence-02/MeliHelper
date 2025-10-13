local trigger = {}
trigger.name = "MeliHelper/ObjectGravityTrigger"
trigger.placements = {
	{
		name = "Object Gravity Trigger",
		data = {
			objectType = "TheoCrystal",
			power = 1
		}
	}
}

trigger.fieldInformation = {
    objectType = {
        options = { "Player", "TheoCrystal", "LaniHoldable" },
		editable = false
    }
}

return trigger
