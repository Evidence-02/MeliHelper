local trigger = {}
trigger.name = "MeliHelper/ObjectGravityTrigger"
trigger.placements = {
	{
		name = "Object Gravity Trigger",
		data = {
			objectType = "TheoCrystal"
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
