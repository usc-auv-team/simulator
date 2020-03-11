using System.Collections;
using System.Collections.Generic;

public class BasicBoundingBox
{
    public string name;
    public int xMax, xMin, yMax, yMin;

    public BasicBoundingBox(string name, int xMax, int xMin, int yMax, int yMin) {
        this.name = name;
        this.xMax = xMax;
        this.xMin = xMin;
        this.yMax = yMax;
        this.yMin = yMin;
    }
}
