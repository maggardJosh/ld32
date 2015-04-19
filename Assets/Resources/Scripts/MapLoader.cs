using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MapLoader
{
    public static void LoadObjects(World world, List<XMLNode> objects)
    {
        foreach (XMLNode node in objects)
            switch (node.attributes["type"].ToUpper())
            {
                case "TRAVELPOINT":
                    string name = node.attributes["name"];
                    float x, y, width, height;
                    float.TryParse(node.attributes["x"], out x);
                    float.TryParse(node.attributes["y"], out y);
                    float.TryParse(node.attributes["width"], out width);
                    float.TryParse(node.attributes["height"], out height);
                    foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                    {

                    }
                    break;
            }
    }
}

