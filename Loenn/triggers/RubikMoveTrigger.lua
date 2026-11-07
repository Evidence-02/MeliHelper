local trigger = {}
trigger.name = "MeliHelper/RubikMoveTrigger"
trigger.placements = {
	{
		name = "Rubik Move Trigger",
		data = {
			minX = -0.6,
			maxX = 0.6,
			minY = -0.3,
			maxY = 0.3,
			koefRotate = 1
		}
	}
}
trigger.fieldOrder = { "x", "y", "width", "height", "minX", "maxX", "minY", "maxY", "koefRotate" }
return trigger
