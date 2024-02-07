using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Hexagons
{
    public static float hexSize = 1f;

    public static void setHexSize(float size) {
        hexSize = size;
    }
    public static void DrawTextureLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }
    

    public static void HexUVTexture(string fileName) {

        string pattern = @"[\\/:*?\""<>|\@]";

        string cleanFilename = Regex.Replace(fileName, pattern, "");
        Debug.Log(cleanFilename);

        Texture2D testTexture = new Texture2D(256, 256);
        Debug.Log(string.Format("test texture pixel: {0}", testTexture.GetPixel(0, 0)));

        Texture2D textureBuffer = new Texture2D(256,256);
        for (int w = 0; w < textureBuffer.width; w++) {
            for (int h = 0; h < textureBuffer.height; h++) {
                textureBuffer.SetPixel(w, h, new Color(0, 0, 0, 0));
            }
        }

        Vector2Int[] pixelPositions = new Vector2Int[6];
        for (int i = 0; i < Hexagons.uvCorners.Length; i++) {
            Vector2 cornerUVPosition = (Hexagons.PointyHexCorner2D(i, 0.5f) + new Vector2(0.5f, 0.5f));
            pixelPositions[i] = new Vector2Int((int) (cornerUVPosition.x * 256f), (int) (cornerUVPosition.y * 256f));
        }
        for (int i = 0; i < pixelPositions.Length; i++) {
            int ir = (int)Mathf.Repeat(i+1, Hexagons.uvCorners.Length);
            DrawTextureLine(textureBuffer, pixelPositions[i], pixelPositions[ir], Color.black);
            DrawTextureLine(textureBuffer, new Vector2Int(testTexture.width / 2, testTexture.height / 2), pixelPositions[ir], Color.black);
        }

        textureBuffer.Apply();

        SaveTextureToFileUtility.SaveTexture2DToFile(textureBuffer, cleanFilename, SaveTextureToFileUtility.SaveTextureFileFormat.PNG);
    }


    public static Vector2[] uvCorners = new Vector2[6] {
        new Vector2(0.5f, 1f),
        new Vector2(0.935f, 0.75f),
        new Vector2(0.935f, 0.25f),
        new Vector2(0.5f, 0f),
        new Vector2(0.065f, 0.25f),
        new Vector2(0.065f, 0.75f)
    };


    public static Vector3[] corners = new Vector3[6] {
        new Vector3(0f, 0f, 1f),
        new Vector3(0.866f, 0f, 0.5f),
        new Vector3(0.866f, 0f, -0.5f),
        new Vector3(0f, 0f, -1f),
        new Vector3(-0.866f, 0f, -0.5f),
        new Vector3(-0.866f, 0f, 0.5f)
    };

    public static Vector2 PointyHexCorner2D(int index, float overrideHexSize = 0f) {
        Vector3 result = PointyHexCorner(index, overrideHexSize);
        return new Vector2(result.x, result.z);
    }

    public static Vector3 PointyHexCorner(int index, float overrideHexSize = 0f) {

        float angleDeg = 60 * index - 90;
        float angleRad = Mathf.PI / 180 * angleDeg;
        return new Vector3(Mathf.Cos(-angleRad), 0f, Mathf.Sin(-angleRad)) * (overrideHexSize != 0f ? overrideHexSize : hexSize);
    }

    public static Vector3 FlatHexCorner(int index, float overrideHexSize = 0f) {
        float angleDeg = 60 * index;
        float angleRad = Mathf.PI / 180 * angleDeg;
        return new Vector3(Mathf.Cos(-angleRad), 0f, Mathf.Sin(-angleRad)) * (overrideHexSize != 0f ? overrideHexSize : hexSize);
    }

    public static void DebugDrawHex(Vector2Int axialPosition, float duration = 1f, int colorIndex = -1) {
        Vector3 center = HexToWorld(axialPosition);

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < 6; i++) {
            points.Add(center + PointyHexCorner(i));
        }

        points.Add(points[0]);

        Color[] colors = new Color[6] {
            Color.red,
            Color.red + Color.yellow / 2f,
            Color.yellow,
            Color.green,
            Color.blue,
            Color.magenta
        };

        for (int i = 0; i < points.Count - 1; i++) {
            Debug.DrawLine(points[i], points[i + 1], colors[colorIndex > -1 ? colorIndex : i], duration);
        }

    }

    public static List<Vector2Int> GenerateHexGrid(int width, int height, bool centered = false) {

        List<Vector2Int> grid = new List<Vector2Int>();

        for (int row = 0; row < height; row++)
        {
            int offset = row / 2;
            for (int col = -offset; col < width - offset; col++)
            {
                grid.Add(new Vector2Int(col - (centered ? (width / 4) : 0), row - (centered ? (height / 2 ): 0) ));
            }
        }

        return grid;
    }

    public static Vector3 HexToWorld(Vector2Int axialCoordinates, Transform transform)
    {
        // Adjust for parent object's position
        return transform.TransformPoint(HexToWorld(axialCoordinates));
    }


    public static Mesh GenerateMesh() {
        List<Vector2Int> coordinates = GenerateHexGrid(5, 5);
        return GenerateMesh(coordinates);
    }
    public static Mesh GenerateMesh(List<Vector2Int> coordinates, List<Vector2Int> highlightCoordinates = null) {

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();

        coordinates.ForEach((coordinate) =>
        {
            Vector3 center = HexToWorld(coordinate);

            for (int i = 0; i < 6; i++)
            {
                int iNext = (int)Mathf.Repeat(i + 1, 6);
                int vertexIndex = vertices.Count;
                vertices.Add(center);
                vertices.Add(center + corners[i] * hexSize);
                vertices.Add(center + corners[iNext] * hexSize);

                if (highlightCoordinates != null && highlightCoordinates.Contains(coordinate)) {
                    colors.Add(Color.red);
                    colors.Add(Color.red);
                    colors.Add(Color.red);
                } else {
                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);
                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                // uvs for individual tile texture, like the grid outline
                Vector2[] triangleUVs = new Vector2[3] {
                        new Vector2(0.5f, 0.5f),
                        Hexagons.PointyHexCorner2D(i, 0.5f) + new Vector2(0.5f, 0.5f), //uvCorners[i],
                        Hexagons.PointyHexCorner2D(iNext, 0.5f) + new Vector2(0.5f, 0.5f) //uvCorners[iNext]
                    };
                uv.AddRange(triangleUVs);
            }
        });

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        return mesh;

    }

    public static List<Vector2Int> GetNeighborsInRange(Vector2Int center, int range) {

        List<Vector3Int> cubeRange = GetNeighborsInRangeCube(AxialToCube(center), range);

        List<Vector2Int> axialRange = new List<Vector2Int>();
        cubeRange.ForEach((cube) =>
        {
            axialRange.Add(CubeToAxial(cube));
        });

        return axialRange;

    }

    public static List<Vector3Int> GetNeighborsInRangeCube(Vector3Int center, int range)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                int dz = -dx - dy;

                int neighborX = center.x + dx;
                int neighborY = center.y + dy;
                int neighborZ = center.z + dz;

                neighbors.Add(new Vector3Int(neighborX, neighborY, neighborZ));
            }
        }

        return neighbors;
    }

    public static Vector2Int[] CornerGetHex(Vector2Int cornerCoordinate) {

        // Pointy-top hexes
        // 
        // if the 0,0 hex's top corner is considered 0,0...
        // the corner at 1,0 is the upper right corner of hex 0,0, the upper left corner of 1,0 and the bottom corner of 0,1
        // which also means that the top corner for hex at 1,0 is corner position 2,0
        // therefore the corner X coordinate naively appears to be definable as cX = hX * 2
        // which would mean hx = cX / 2
        // this works for top corners, but we need to solve for other corners, lets move clockwise to the upper right corner
        // if top corners have even x coordinates, we know that the diagonal corners will have odd coordinates
        // so we can use math to account for the offset of pointed corners
        // hXtop = cX / 2
        // hXupperRight = (cX % 2) + cX / 2 // but we dont really need this here at all:
        // We need to consider the y axis as well this will help us determine if the point is connected to two hex coordinates above or two hex below (and one in the opposite direction)
        // it just happens to be that when the sum of x and y are even there are two hexes on the top, when the sum is odd there is one hex on the top
        // therefore:
        // h1X = (cX / 2) - ((cX + cY) % 2)
        // h1Y = cY + ((cx + cY) % 2)
        // h2X = (cX / 2) - ((cX + cY) % 2)
        // h2Y = cy + ((1 + cx + cY) % 2)
        // h3X = (cX / 2) - ((1 + cX + cY) % 2)
        // h3Y = cY + 1
        // knowing that topCorner conditions is (cX + cY) % 2 = 0
        // h1X = (cX / 2)
        // h1Y = cY
        // h2X = (cX / 2)
        // h2Y = cY + 1
        // h3X = (cx / 2) - 1
        // h3Y = cY + 1
        // making the bottom corner positions (cX + cY) % 2 = 1
        // h1X = (cX / 2) - 1
        // h1Y = cY + 1
        // h2X = (cX / 2)
        // h2Y = cY
        // h3X = (cX / 2) - 1
        // h3Y = cY
        //
        // combining these, this should be correct:
        // h1X = (cX / 2) - ((cX + cY) % 2)
        // h1Y = cY + ((cX + cY) % 2)
        // h2X = (cX / 2)
        // h2Y = cY + ((1 + cX + cY) % 2)
        // h3X = (cX / 2) - 1
        // h3Y = cY + ((1 + cX + cY) % 2)

        int x = cornerCoordinate.x;
        int y = cornerCoordinate.y;

        // if x + y is odd, the orientation is two below, one above
        // if x + y is even, the orientation is one below, two above
        // tested and working with permutations including odd/even in all vector spaces (+,+, +,-, -,+, -,-) but more extensive testing may be necessary.
        if ((Mathf.Abs(x) + Mathf.Abs(y)) % 2 == 1) {
            // odd - two below, one above
            int offset = 0;
            if (x > 0) offset++;
            if (x < 0) offset--;
            Vector2Int centerCoord = new Vector2Int((Mathf.Abs(x) > 1 ? (x / 2) : x ) - offset, y + 1);
            Vector2Int lowerLeft = centerCoord + new Vector2Int(0, -1);
            Vector2Int lowerRight = centerCoord + new Vector2Int(1, -1);
            return new Vector2Int[3] {
                centerCoord, lowerLeft, lowerRight
            };
        } else {
            // even one below, two above
            int offset = 0;
            if (x < 0) offset++;
            Vector2Int centerCoord = new Vector2Int((Mathf.Abs(x) > 1 ? (x / 2) : x) + offset, y);
            Vector2Int upperLeft = centerCoord + new Vector2Int(-1, 1);
            Vector2Int upperRight = centerCoord + new Vector2Int(0, 1);
            return new Vector2Int[3] {
                centerCoord, upperLeft, upperRight
            };
        }



    }

    public static void CornerGetHexTest() {
        Vector2Int[] test1 = CornerGetHex(new Vector2Int(4, 1));
        Vector2Int[] test2 = CornerGetHex(new Vector2Int(1, 1));
        Vector2Int[] test3 = CornerGetHex(new Vector2Int(-1, -2));
        Vector2Int[] test4 = CornerGetHex(new Vector2Int(-3, -3));

        for (int i = 0; i < test1.Length; i++)
        {
            Debug.Log(string.Format("Test1 value {0}: {1}", i, test1[i]));
        }


        for (int i = 0; i < test2.Length; i++)
        {
            Debug.Log(string.Format("Test2 value {0}: {1}", i, test2[i]));
        }


        for (int i = 0; i < test3.Length; i++)
        {
            Debug.Log(string.Format("Test3 value {0}: {1}", i, test3[i]));
        }

        for (int i = 0; i < test4.Length; i++)
        {
            Debug.Log(string.Format("Test4 value {0}: {1}", i, test4[i]));
        }
    }



    public static Mesh GenerateVoxelMesh(int[,,] coordinateData) {

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();

        Vector3 depthOffset = new Vector3(0, 5f, 0);


        return mesh;

    }

    public static Mesh GenerateThickMesh(List<Vector2Int> coordinates, List<Vector2Int> highlightCoordinates = null)
    {

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();

        Vector3 depthOffset = new Vector3(0, 5f, 0);

        coordinates.ForEach((coordinate) =>
        {
            Vector3 center = HexToWorld(coordinate) - depthOffset * 0.5f;

            for (int i = 0; i < 6; i++)
            {
                int iNext = (int)Mathf.Repeat(i + 1, 6);
                int vertexIndex = vertices.Count;
                vertices.Add(center + corners[iNext] * hexSize);
                vertices.Add(center + corners[i] * hexSize);
                vertices.Add(center);
                
                

                // add side walls
                // wall face 1 next corner
                vertices.Add(center + corners[iNext] * hexSize);
                // wall face 1 corner extent
                vertices.Add(center + depthOffset + corners[i] * hexSize);
                // wall face 1 corner
                vertices.Add(center + corners[i] * hexSize);


                // wall face 2 next corner
                vertices.Add(center + corners[iNext] * hexSize);
                // wall face 2 next corner extent
                vertices.Add(center + depthOffset + corners[iNext] * hexSize);

                // wall face 2 corner
                vertices.Add(center + depthOffset + corners[i] * hexSize);
                

                // add the bottom cap
                vertices.Add(center + depthOffset);
                vertices.Add(center + depthOffset + corners[i] * hexSize);
                vertices.Add(center + depthOffset + corners[iNext] * hexSize);



                if (highlightCoordinates != null && highlightCoordinates.Contains(coordinate))
                {
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                    colors.Add(Color.black);
                }
                else
                {
                    Color brown = new Color(0.5f, 0.5f, 0);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);
                    colors.Add(brown);

                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 4);
                triangles.Add(vertexIndex + 5);
                triangles.Add(vertexIndex + 6);
                triangles.Add(vertexIndex + 7);
                triangles.Add(vertexIndex + 8);
                triangles.Add(vertexIndex + 9);
                triangles.Add(vertexIndex + 10);
                triangles.Add(vertexIndex + 11);


                // uvs for individual tile texture, like the grid outline
                Vector2[] triangleUVs = new Vector2[3] {
                    Hexagons.PointyHexCorner2D(iNext, 0.5f) + new Vector2(0.5f, 0.5f), //uvCorners[iNext]
                    Hexagons.PointyHexCorner2D(i, 0.5f) + new Vector2(0.5f, 0.5f), //uvCorners[i],
                    new Vector2(0.5f, 0.5f),
                        
                        
                };
                uv.AddRange(triangleUVs);

                // add the two side faces' UVs
                Vector2[] sideUVs = new Vector2[6] {
                        new Vector2(0f, 1f),
                        new Vector2(1f, 0.5f),
                        new Vector2(1f, 1f),
                        new Vector2(0f, 0f),
                        new Vector2(1f, 0f),
                        new Vector2(1f, 0.5f)
                    };

                uv.AddRange(sideUVs);
                // bottom UVs same as top UVs
                uv.AddRange(triangleUVs);

            }
        });

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        return mesh;

    }

    public static void Tests() {
        // Example usage of hexagonal grid functionality
        Vector2Int axialCoordinate = new Vector2Int(1, 1);
        Vector2Int offsetCoordinate = new Vector2Int(1, 1);
        Vector3 pixelPosition = HexToWorld(axialCoordinate);
        Debug.Log("Pixel Position: " + pixelPosition);

        Vector2Int newAxialCoordinate = WorldToHex(pixelPosition);
        Debug.Log("Axial Coordinate: " + newAxialCoordinate);

        Vector2Int[] neighbors = GetNeighbors(axialCoordinate);
        Debug.Log("Neighbors: " + string.Join(", ", neighbors));

        float distance = GetDistance(axialCoordinate, new Vector2Int(0, 0));
        Debug.Log("Distance: " + distance);

        Vector2Int[] line = GetLine(axialCoordinate, new Vector2Int(3, 1));
        Debug.Log("Line: " + string.Join(", ", line));

        Vector2Int rotated = Rotate(axialCoordinate, axialCoordinate + offsetCoordinate, 60);
        Debug.Log("Rotated Coordinate: " + rotated);

        Vector2Int reflectedQ = ReflectQ(axialCoordinate);
        Debug.Log("ReflectedQ Coordinate: " + reflectedQ);

        Vector2Int reflectedR = ReflectR(axialCoordinate);
        Debug.Log("ReflectedR Coordinate: " + reflectedR);

        Vector2Int reflectedS = ReflectS(axialCoordinate);
        Debug.Log("ReflectedS Coordinate: " + reflectedS);
    }

    // Hex to world
    public static Vector3 HexToWorld(Vector2Int hex)
    {
        float x = hexSize * (Mathf.Sqrt(3f) * hex.x + Mathf.Sqrt(3f) / 2f * (float)hex.y);
        float z = hexSize * (3f / 2f * (float)hex.y);
        return new Vector3(x, 0, z);
    }

    // Pixel to Hex
    public static Vector2Int WorldToHex(Vector3 pixel)
    {
        float q = (Mathf.Sqrt(3f) / 3f * pixel.x - 1f/3f * pixel.z) / hexSize;
        float r = (2f / 3f * pixel.z) / hexSize;
        return RoundToAxial(new Vector2(q, r));
    }

    // Get Neighbors
    public static Vector2Int[] GetNeighbors(Vector2Int hex)
    {
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, -1),
                                    new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1) };

        Vector2Int[] neighbors = new Vector2Int[6];
        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = hex + directions[i];
        }

        return neighbors;
    }

    public static int GetDistance(Vector3Int hex1, Vector3Int hex2) {
        Vector3Int offset = hex1 - hex2;
        return (Mathf.Abs(offset.x) + Mathf.Abs(offset.y) + Mathf.Abs(offset.z)) / 2;
    }

    public static int GetDistance(Vector2Int hex1, Vector2Int hex2) {
        return GetDistance(AxialToCube(hex1), AxialToCube(hex2));
    }

    // Get World Distance
    public static float GetWorldDistance(Vector2Int hex1, Vector2Int hex2)
    {
        Vector3 pos1 = Hexagons.HexToWorld(hex1);
        Vector3 pos2 = Hexagons.HexToWorld(hex2);
        return Vector3.Distance(pos1, pos2) / hexSize;
    }

    // Get Line
    public static Vector2Int[] GetLine(Vector2Int start, Vector2Int end)
    {
        int distance = (int)GetDistance(start, end);
        Vector2Int[] line = new Vector2Int[distance + 1];

        for (int i = 0; i <= distance; i++)
        {
            float t = 1f / distance * i;
            line[i] = RoundToAxial(Vector2.Lerp(start, end, t));
        }

        return line;
    }


    public static Vector2Int Rotate(Vector2Int center, Vector2Int hex, int rotationDelta)
    {
        Vector3Int centerCube = AxialToCube(center);
        Vector3Int targetCube = AxialToCube(hex);
        // Debug.Log(string.Format("Rotation center cube: {0}, axial: {1}", centerCube, center));
        // Debug.Log(string.Format("Rotation cube: {0}, axial: {1}", targetCube, hex));

        Vector3Int rotationVector = targetCube - centerCube;

        int distance = Hexagons.GetDistance(Vector3Int.zero, rotationVector);
        Debug.Log(string.Format("Rotation Vector length {0}", Hexagons.GetDistance(Vector3Int.zero, rotationVector)));

        if (rotationDelta > 0) {
            // rotate right, which just swaps the coordinate values to the left
            for (int i = 0; i < (rotationDelta); i++) {
                rotationVector = new Vector3Int(-rotationVector.z, -rotationVector.x, -rotationVector.y);
            }
        } else if (rotationDelta < 0) {
            // rotate left, which just swaps the coordinate values to the right
            for (int i = 0; i > (rotationDelta); i--)
            {
                rotationVector = new Vector3Int(-rotationVector.y, -rotationVector.z, -rotationVector.x);
            }
        }

        return CubeToAxial(rotationVector + centerCube);
    }

    public static List<Vector2Int> Rotate(List<Vector2Int> coordinates, int rotation)
    {

        List<Vector2Int> hexCoordinates = new List<Vector2Int>();

        int index = 0;
        coordinates.ForEach((buildingCoordinate) =>
        {
            if (index == 0)
            {
                hexCoordinates.Add(buildingCoordinate);
                index++;
                return;
            }

            Vector2Int newCoord = Hexagons.Rotate(coordinates[0], buildingCoordinate, rotation);
            hexCoordinates.Add(newCoord);
            index++;

        });

        return hexCoordinates;
    }

    public static Vector2Int CubeToAxial(Vector3Int cube) {
        return new Vector2Int(cube.x, cube.y);
    }

    public static Vector3Int AxialToCube(Vector2Int axial) {
        return new Vector3Int(axial.x, axial.y, -axial.x -axial.y);
    }

    public static Vector3 AxialToCube(Vector2 axial)
    {
        return new Vector3(axial.x, axial.y, -axial.x - axial.y);
    }

    public static void AxialToCubeTest() {
        // Test coordinates
        Vector2Int axialCoordinate1 = new Vector2Int(3, 3);
        Vector2Int axialCoordinate2 = new Vector2Int(3, 4);

        // Convert to cube coordinates
        Vector3Int cubeCoordinate1 = AxialToCube(axialCoordinate1);
        Vector3Int cubeCoordinate2 = AxialToCube(axialCoordinate2);

        // Log the results
        Debug.Log("Axial (0,0) to Cube: " + cubeCoordinate1);
        Debug.Log("Axial (0,1) to Cube: " + cubeCoordinate2);
    }

    // Reflect over Q/X axis (swap the y and z)
    public static Vector2Int ReflectQ(Vector2Int hex)
    {
        Vector3Int cube = AxialToCube(hex);
        return CubeToAxial(new Vector3Int(cube.x, cube.z, cube.y));
    }

    // Reflect over R/Y axis (swap the x an z)
    public static Vector2Int ReflectR(Vector2Int hex)
    {
        Vector3Int cube = AxialToCube(hex);
        return CubeToAxial(new Vector3Int(cube.z, cube.y, cube.x));
    }

    // Reflect over S/Z axis (swap x and y)
    public static Vector2Int ReflectS(Vector2Int hex)
    {
        Vector3Int cube = AxialToCube(hex);
        return CubeToAxial(new Vector3Int(cube.y, cube.x, cube.z));
    }


    public static Vector3Int CubeRound(Vector3 coord) {
        float x = Mathf.Round(coord.x);
        float y = Mathf.Round(coord.y);
        float z = Mathf.Round(coord.z);

        float xDiff = Mathf.Abs(x - coord.x);
        float yDiff = Mathf.Abs(y - coord.y);
        float zDiff = Mathf.Abs(z - coord.z);

        if (xDiff > yDiff && xDiff > zDiff) {
            x = -y - z;
        } else if (yDiff > zDiff) {
            y = -x - z;
        } else {
            z = -x - y;
        }

        return new Vector3Int((int) x, (int) y, (int) z);

    }

    // Helper method to round axial coordinates to integers
    public static Vector2Int RoundToAxial(Vector2 coord)
    {
        return CubeToAxial(CubeRound(AxialToCube(coord)));    
    }

    public static List<Vector3> GetOuterVertices(List<Vector2Int> hexagonCoordinates)
    {
        List<Vector3> outerVertices = new List<Vector3>();

        Vector3 offset = new Vector3(0f, 1f, 0f);

        foreach (Vector2Int coordinate in hexagonCoordinates)
        {

            Vector2Int[] neighbors = GetNeighbors(coordinate);

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighborCoordinate = neighbors[i];
                // Check if the neighbor is not in the collection
                if (!hexagonCoordinates.Contains(neighborCoordinate))
                {
                    // Get the corners of the missing edge
                    Vector3 corner1 = HexToWorld(coordinate) + corners[(int) Mathf.Repeat(i, 6)] * hexSize;
                    Vector3 corner2 = HexToWorld(coordinate) + corners[(int) Mathf.Repeat(i + 1, 6)] * hexSize;

                    // Add corners to the list
                    outerVertices.Add(corner1 + offset);
                    outerVertices.Add(corner2 + offset);
                }
            }
        }

        // Remove duplicate and very close points
        outerVertices = RemoveDuplicateAndClosePoints(outerVertices);


        // Arrange the list of points in clockwise order
        outerVertices = ArrangeClockwise(outerVertices);

        outerVertices.Add(outerVertices[0]);
        outerVertices.Add(outerVertices[1]);

        return outerVertices;
    }

    public static  List<Vector3> RemoveDuplicateAndClosePoints(List<Vector3> points)
    {
        List<Vector3> cleanedPoints = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            bool isClose = false;

            // Check if the point is close to any existing point in the cleaned list
            foreach (Vector3 cleanedPoint in cleanedPoints)
            {
                if (Vector3.Distance(point, cleanedPoint) < 0.01f)
                {
                    isClose = true;
                    break;
                }
            }

            if (!isClose)
            {
                cleanedPoints.Add(point);
            }
        }

        return cleanedPoints;
    }

    public static List<Vector3> ArrangeClockwise(List<Vector3> points)
    {
        // Calculate the center as the average of all points
        Vector3 center = Vector3.zero;
        foreach (Vector3 point in points)
        {
            center += point;
        }
        center /= points.Count;

        // Sort points based on the angle with respect to the center
        points.Sort((a, b) => Mathf.Atan2(a.z - center.z, a.x - center.x).CompareTo(Mathf.Atan2(b.z - center.z, b.x - center.x)));

        return points;
    }
}