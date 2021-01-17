using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVS;
using System.Linq;

public class RoadHelper : MonoBehaviour
{
    public GameObject straight, corner, r3way, r4way, end;
    Dictionary<Vector3Int, GameObject> roadDict = new Dictionary<Vector3Int, GameObject>();
    HashSet<Vector3Int> FixedRCand = new HashSet<Vector3Int>();
    public List<Vector3Int> GetRoadPos()
    {
        return roadDict.Keys.ToList();
    }
    public void Reset()
    {
        foreach (var it in roadDict.Values)
        {
            Destroy(it);
        }
        roadDict.Clear();
        FixedRCand=new HashSet<Vector3Int>();
    }
    public void PlaceStreet(Vector3 startPos, Vector3Int dir, int length) {

        var rotation = Quaternion.identity;
        if(dir.x == 0)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }

        for (int i = 0; i < length; i++)
        {
            var pos = Vector3Int.RoundToInt(startPos + dir * i);
            if (roadDict.ContainsKey(pos)) { continue; }
            var road = Instantiate(straight, pos, rotation, transform);
            roadDict.Add(pos, road);
            if (i == 0 || i == length - 1)
            {
                FixedRCand.Add(pos);
            }
        }
    }

    public void fixRoad()
    {

        foreach (var pos in FixedRCand)
        {
            List<Direction> neighborDirs = PlacementHelper.FindNeighbour(pos, roadDict.Keys);
            Quaternion rot = Quaternion.identity;

            //road end
            if (neighborDirs.Count == 1) {
                Destroy(roadDict[pos]);
                if (neighborDirs.Contains(Direction.Down))
                {
                    rot = Quaternion.Euler(0, 90, 0);
                }
                else if (neighborDirs.Contains(Direction.Left))
                {
                    rot = Quaternion.Euler(0, 180, 0);
                }
                else if (neighborDirs.Contains(Direction.Up))
                {
                    rot = Quaternion.Euler(0, -90, 0);
                }
                roadDict[pos] = Instantiate(end, pos, rot, transform);
            }
            else if (neighborDirs.Count == 2)
            {
                if (neighborDirs.Contains(Direction.Up) && neighborDirs.Contains(Direction.Down) || neighborDirs.Contains(Direction.Right) && neighborDirs.Contains(Direction.Left))
                {
                    continue;
                }
                Destroy(roadDict[pos]);
                if (neighborDirs.Contains(Direction.Up) && neighborDirs.Contains(Direction.Right))
                {
                    rot = Quaternion.Euler(0, 90, 0);
                }
                else if (neighborDirs.Contains(Direction.Right) && neighborDirs.Contains(Direction.Down))
                {
                    rot = Quaternion.Euler(0, 180, 0);
                }
                else if (neighborDirs.Contains(Direction.Down) && neighborDirs.Contains(Direction.Left))
                {
                    rot = Quaternion.Euler(0, -90, 0);
                }
                roadDict[pos] = Instantiate(corner, pos, rot, transform);
            }
            else if (neighborDirs.Count == 3)
            {
                Destroy(roadDict[pos]);
                if (neighborDirs.Contains(Direction.Right) && neighborDirs.Contains(Direction.Down) && neighborDirs.Contains(Direction.Left))
                {
                    rot = Quaternion.Euler(0, 90, 0);
                }
                else if (neighborDirs.Contains(Direction.Down) && neighborDirs.Contains(Direction.Left) && neighborDirs.Contains(Direction.Up))
                {
                    rot = Quaternion.Euler(0, 180, 0);
                }
                else if (neighborDirs.Contains(Direction.Left) && neighborDirs.Contains(Direction.Up) && neighborDirs.Contains(Direction.Right))
                {
                    rot = Quaternion.Euler(0, -90, 0);
                }
                roadDict[pos] = Instantiate(r3way, pos, rot, transform);
            }
            else
            {
                Destroy(roadDict[pos]);
                roadDict[pos] = Instantiate(r4way, pos, rot, transform);
            }

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
