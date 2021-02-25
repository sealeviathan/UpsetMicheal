using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public static MapBuilder instance;
    public GameObject[] TilesNormal;
    public GameObject[] PitJump;
    public GameObject[] Items;
    
    [Range(4,10)]
    public int ChunkSize = 4;
    public int startPush = 500;
    public int maxChunks = 3;
    public int leftBorder = -100;
    public int specialOdds = 5;
    public int speed = 30;
    public int distance = 0;
    Queue<GameObject> TerrainChunks = new Queue<GameObject>();
    GameObject mostRecentChunk;

    Coroutine SlowRoutine = null;
    public bool slowing = false;
    
    // Start is called before the first frame update

    GameObject CreateChunk(bool special)
    {
        GameObject[] chunk = new GameObject[ChunkSize];
        GameObject fullChunk = new GameObject("Terrain Chunk");
        fullChunk.tag = "TerrainChunk";
        Vector2 farLeftCorner;
        Vector2 farRightCorner;
        int specialIndex = 0;
        for(int i = 0; i < ChunkSize;i++)
        {
            int randomChoice = Random.Range(0, TilesNormal.Length);
            Vector2 origin;
            TerrainData curChunk = TilesNormal[randomChoice].GetComponent<TerrainData>();

            if(i == 0)
            {
                origin=transform.position;
                GameObject piece = Instantiate(TilesNormal[randomChoice],origin, Quaternion.identity) as GameObject;
                farLeftCorner = piece.transform.Find("TerrainLeftCorner").transform.position;

                GameObject farLeft = new GameObject("ChunkLeftCorner");
                farLeft.transform.position = farLeftCorner;
                farLeft.transform.SetParent(fullChunk.transform);

                piece.transform.SetParent(fullChunk.transform);
                chunk[i]=piece;
            }
            else
            {
                TerrainData prevChunk = chunk[i-1].GetComponent<TerrainData>();
                Vector2 rightCornerPrev = prevChunk.transform.Find("TerrainRightCorner").transform.position;

                origin = new Vector2(rightCornerPrev.x + curChunk.width/2,rightCornerPrev.y);
                if(!special || specialIndex >= PitJump.Length)
                {
                    GameObject piece = Instantiate(TilesNormal[randomChoice],origin, Quaternion.identity) as GameObject;

                    float cornerDist = piece.transform.Find("TerrainLeftCorner").transform.position.y - rightCornerPrev.y;
                    piece.transform.position = new Vector2(piece.transform.position.x, piece.transform.position.y - cornerDist);
                    if(i == ChunkSize - 1)
                    {
                        farRightCorner = piece.transform.Find("TerrainRightCorner").transform.position;
                        GameObject farRight = new GameObject("ChunkRightCorner");
                        
                        farRight.transform.position = farRightCorner;
                        farRight.transform.SetParent(fullChunk.transform);
                    }
                    piece.transform.SetParent(fullChunk.transform);
                    chunk[i]=piece;
                }
                else
                {
                    GameObject piece = Instantiate(PitJump[specialIndex],origin, Quaternion.identity) as GameObject;

                    float cornerDist = piece.transform.Find("TerrainLeftCorner").transform.position.y - rightCornerPrev.y;
                    piece.transform.position = new Vector2(piece.transform.position.x, piece.transform.position.y - cornerDist);
                    if(i == ChunkSize - 1)
                    {
                        farRightCorner = piece.transform.Find("TerrainRightCorner").transform.position;
                        GameObject farRight = new GameObject("ChunkRightCorner");
                        
                        farRight.transform.position = farRightCorner;
                        farRight.transform.SetParent(fullChunk.transform);
                    }
                    piece.transform.SetParent(fullChunk.transform);
                    chunk[i]=piece;
                    specialIndex++;
                }
            }
            
        }
        return fullChunk;
    }
    void AddItem(Transform parentChunk)
    {
        int randChance = Random.Range(0,2);

        if(randChance == 1)
        {
            int randChoice = Random.Range(0, Items.Length);
            float additionalHeight = 2f;
            GameObject newItem = Instantiate(Items[randChoice], parentChunk.transform.position, Quaternion.identity, parentChunk) as GameObject;
            
            RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(parentChunk.position.x, parentChunk.position.y + 10),Vector2.down);
            Vector2 groundpoint = hit2D.point;
    
            newItem.transform.position = new Vector2(groundpoint.x, groundpoint.y + additionalHeight);
        }
    }
    IEnumerator SlowMap(float interval)
    {
        
        while(true)
        {
            yield return new WaitForSeconds(interval);
            if(speed > 0 && slowing)
            {
                speed--;
            }
            else if(speed <= 0)
            {
                break;
            }
        }
    }
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Cannot be more than once instance of MapBuilder. Destroying instance on " + gameObject.name + "...");
        }
        
    }
    void Start()
    {
        GameObject newChunk = CreateChunk(false);
        mostRecentChunk = newChunk;
        newChunk.transform.Translate(new Vector2(-startPush, 0));
        TerrainChunks.Enqueue(newChunk);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Straight up the gen code V V V
        //Uses a queue, because I have never used a queue before. I think it's faster?
        //mostRecentChunk is used because I can't access queue indices without ruining the point of a queue. Using
        //something like a dictionary would make this obsolete.

        foreach(GameObject chunkPiece in TerrainChunks)
        {
            chunkPiece.transform.Translate(new Vector2(-speed * Time.deltaTime, 0));
        }
        if(TerrainChunks.Count > maxChunks)
        {
            Debug.LogWarning("Poopie >:( 2 many chunks");
        }
        if(TerrainChunks.Count < maxChunks)
        {
            //When it's time to make a new chunk, execute the following
            bool isSpecial = Random.Range(0,specialOdds) == 0;

            GameObject newChunk = CreateChunk(isSpecial);
            Vector2 curChunkLeftCorner = newChunk.transform.Find("ChunkLeftCorner").transform.position;
            Vector2 prevChunkRightCorner = mostRecentChunk.transform.Find("ChunkRightCorner").transform.position;
            Vector2 distToMove = new Vector2(prevChunkRightCorner.x - curChunkLeftCorner.x, prevChunkRightCorner.y - curChunkLeftCorner.y);

            
            newChunk.transform.Translate(distToMove);
            TerrainChunks.Enqueue(newChunk);
            mostRecentChunk=newChunk;
            AddItem(newChunk.transform);
        }
        else if(TerrainChunks.Peek().transform.position.x < leftBorder && TerrainChunks.Count >= maxChunks)
        {
            GameObject toDestroy = TerrainChunks.Dequeue();
            Destroy(toDestroy);   
        }

        //Other update stuff
        distance += (int)(speed * Time.deltaTime * 2);
    }
}
