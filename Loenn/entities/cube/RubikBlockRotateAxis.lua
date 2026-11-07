local block = {}
block.name = "MeliHelper/RubikBlockRotateAxis"
block.placements = {
    {
        name = "Rubik Rotate Axis Block",
        data = {
			width = 16,
			height = 16,
			sprite = "",
			animationOnDash = "",
			axis = "X",
			clockwise = true
        }
    }
}
block.fieldInformation = {
	axis = {
		options = {	"X", "Y", "Z" }, editable = false
	}
}
block.fieldOrder = { "x", "y", "width", "height", "sprite", "animationOnDash", "axis", "clockwise" }
return block
