using UnityEngine;
using UnityEditor;

public class Make_Terrain_Prefab
{
    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
    [MenuItem("2D Map Helper/Create Terrain Prefab/Desert")]
    static void CreatePrefab()
    {
        // Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        // Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            PolygonCollider2D polygonCollider = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            Texture2D spriteTexture = spriteRenderer.sprite.texture;
            TerrainData terrainData = gameObject.AddComponent(typeof(TerrainData)) as TerrainData;
            Rect spriteRect = spriteRenderer.sprite.rect;
            int pixelsPerUnit = Mathf.RoundToInt(spriteRect.width / spriteRenderer.sprite.bounds.size.x);
            Debug.Log(pixelsPerUnit.ToString() + "PPU");
            Debug.Log(spriteRect.width.ToString() + " rect width " + spriteRect.height.ToString() + " Height");
            polygonCollider.offset = new Vector2(0, -0.15f);
            terrainData.width = spriteRenderer.sprite.bounds.size.x;
            terrainData.height = spriteRenderer.sprite.bounds.size.y;
            
            //Get left edge corner
            for(int y = 0; y < spriteRect.height; y++)
            {
                Color pixelColor = spriteTexture.GetPixel(0,y);
                if(pixelColor.a == 0.000f)
                {
                    terrainData.leftEdgeCorner = new Vector2(gameObject.transform.position.x - (terrainData.width/2), gameObject.transform.position.y - (terrainData.height/2) + (Mathf.CeilToInt(y)/100));
                    break;
                }
            }
            GameObject leftLock = new GameObject("TerrainLeftCorner");
            leftLock.transform.position = terrainData.leftEdgeCorner;
            leftLock.transform.SetParent(gameObject.transform);
            // get right edge corner
            for(int y = 0; y < spriteRect.height; y++)
            {
                Color pixelColor = spriteTexture.GetPixel((int)spriteRect.width,y);
                if(pixelColor.a == 0)
                {
                    terrainData.rightEdgeCorner = new Vector2(gameObject.transform.position.x + (terrainData.width/2), gameObject.transform.position.y - (terrainData.height/2) + (Mathf.CeilToInt(y)/100));
                    break;
                }
            }
            GameObject rightLock = new GameObject("TerrainRightCorner");
            rightLock.transform.position = terrainData.rightEdgeCorner;
            rightLock.transform.SetParent(gameObject.transform);
            
            

            // Set the path as within the Assets folder,
            // and name it as the GameObject's name with the .Prefab format
            string localPath = "Assets/Prefabs/Terrain/Desert" + gameObject.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab.
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
        }
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("Examples/Create Prefab", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }
}