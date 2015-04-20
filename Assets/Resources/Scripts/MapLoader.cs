using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapLoader
{
    public static void LoadObjects(World world, List<XMLNode> objects)
    {
                    string name = "";
                    string mapTarget = "";
                    string target = "";
        foreach (XMLNode node in objects)
            switch (node.attributes["type"].ToUpper())
            {
                case "TRAVELPOINT":
                    name = node.attributes["name"];
                    float x, y, width, height;
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    float.TryParse(node.attributes["width"], out width);
                    float.TryParse(node.attributes["height"], out height);
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {
                        switch (property.attributes["name"].ToUpper())
                        {
                            case "MAP": mapTarget = property.attributes["value"]; break;
                            case "TARGET": target = property.attributes["value"]; break;
                        }
                    }
                    world.addTravelPoint(new WorldTravelPoint(new Rect(x, -y - height, width, height), name, mapTarget, target));
                    break;
                case "DOOR":
                    name = node.attributes["name"];
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    float.TryParse(node.attributes["width"], out width);
                    float.TryParse(node.attributes["height"], out height);
                    if (width < height) // vertical
                    {
                        x += 16;
                        y = -y - 32;
                    }
                    else
                    {
                        x += 32;
                        y = -y - 16;
                    }

                    world.addDoor(new Door(x, y, name, width < height));
                    break;
                case "LEVER":
                    name = node.attributes["name"];
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    target = "";
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {
                        switch (property.attributes["name"].ToUpper())
                        {
                            case "DOOR": target = property.attributes["value"]; break;
                        }
                    }
                    world.addLever(new Lever(x + 16, -y - 16, name, target));
                    break;
                case "FLOORBUTTON":
                    name = node.attributes["name"];
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    target = "";
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {
                        switch (property.attributes["name"].ToUpper())
                        {
                            case "DOOR": target = property.attributes["value"]; break;
                        }
                    }
                    world.addFloorButton(new FloorButton(x, -y - 16, name, target));
                    break;
                case "CEILBUTTON":
                    name = node.attributes["name"];
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    target = "";
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {
                        switch (property.attributes["name"].ToUpper())
                        {
                            case "DOOR": target = property.attributes["value"]; break;
                        }
                    }
                    world.addCeilButton(new CeilButton(x, -y - 16, name, target));
                    break;
                case "WALLBUTTON":
                    name = node.attributes["name"];
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    target = "";
                    bool faceLeft = false;
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {
                        switch (property.attributes["name"].ToUpper())
                        {
                            case "DOOR": target = property.attributes["value"]; break;
                            case "LEFT": bool.TryParse(property.attributes["value"], out faceLeft); break;
                        }
                    }
                    world.addWallButton(new WallButton(x + 16, -y - 16, name, target, faceLeft));
                    break;
                case "POWERUP":
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    int powerup = 1;
                    target = "";
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {
                        switch (property.attributes["name"].ToUpper())
                        {
                            case "POWERUP": int.TryParse(property.attributes["value"], out powerup); break;
                        }
                    }
                    world.addPowerup(new Powerup(x + 16, -y - 16, powerup));
                    break; 
                case "BREAKABLEWALL":
                    name = node.attributes["name"];
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    world.addBreakableWall(new BreakableWall(x + 16, -y - 32, name));
                    break;
            }
    }
}

