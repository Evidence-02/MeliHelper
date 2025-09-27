local backdrop = {}

backdrop.name = "MeliHelper/CirclineDisasterBackdrop"
backdrop.canBackground = true
backdrop.defaultData = {
	only ="*", exclude="", flag = "", notflag = "", texture = "Evidence02/objects_melihelper/bubsybackdrop/textureWhite", points = 16, links = 12, widthMin = 5, widthMax = 9, startSpeed = 24, maxSpeed = 50, linksMaxDistance = 100, maxLinksOnPoint = 4, lineColor = "FFFFFF", lineThickness = 1
}
backdrop.fieldOrder = {
    "texture", "only", "exclude", "tag", "flag", "notflag", "points", "links", "widthMin", "widthMax", "startSpeed", "maxSpeed", "linksMaxDistance", "maxLinksOnPoint", "lineColor", "lineThickness"
}

backdrop.fieldInformation = {
    points = {
        fieldType = "integer",
        minimumValue = 1
    },
    links = {
        fieldType = "integer",
        minimumValue = 0
    },
    maxLinksOnPoint = {
        fieldType = "integer",
        minimumValue = 0
    },
    lineColor = {
        fieldType = "color"
    },
}

return backdrop