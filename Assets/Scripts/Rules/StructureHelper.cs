/*
	Made by Sunny Valle Studio
	(https://svstudio.itch.io)
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
	public class StructureHelper : MonoBehaviour
	{
		//public GameObject prefab;

		public HouseType[] buildTypes;
		public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>();

        public void Reset()
        {
            foreach(var it in structuresDictionary.Values)
            {
				Destroy(it);
            }

			structuresDictionary.Clear();
			foreach(var bui in buildTypes)
            {
				bui.Reset();
            }
        }
        public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions)
		{
			Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);

			foreach( var freeSpot in freeEstateSpots)
            {
				var rot = Quaternion.identity;
                switch (freeSpot.Value)
                {
					case Direction.Up:
						rot = Quaternion.Euler(0, 90, 0);
						break;
					case Direction.Down:
						rot = Quaternion.Euler(0, -90, 0);
						break;
					case Direction.Right:
						rot = Quaternion.Euler(0, 180, 0);
						break;
					default:
						break;
                }

				for (int i = 0; i < buildTypes.Length; i++)
				{
					if (buildTypes[i].quant == -1)
					{
						GameObject b = SpawnPrefab(buildTypes[i].getPrefab(), freeSpot.Key, rot);
						//Instantiate(prefab, position, Quaternion.identity, transform);
						structuresDictionary.Add(freeSpot.Key, b);
						break;
					}
					if (buildTypes[i].isBuildAvail())
					{
						if (buildTypes[i].sizeRequired > 1)
						{

						}
						else
						{
							GameObject b = SpawnPrefab(buildTypes[i].getPrefab(), freeSpot.Key, rot);
							//Instantiate(prefab, position, Quaternion.identity, transform);
							structuresDictionary.Add(freeSpot.Key, b);
							
						}
						break;
					}
				}
			}
			//foreach (var position in freeEstateSpots.Keys)
			//{
			//	Instantiate(prefab, position, Quaternion.identity, transform);
			//}
		}

        private GameObject SpawnPrefab(GameObject prefab, Vector3Int pos, Quaternion rot)
        {
			var nStr = Instantiate(prefab, pos, rot, transform);
			return nStr;
        }

        private Dictionary<Vector3Int, Direction> FindFreeSpacesAroundRoad(List<Vector3Int> roadPositions)
		{
			Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
			foreach (var position in roadPositions)
			{
				var neighbourDirections = PlacementHelper.FindNeighbour(position, roadPositions);
				foreach (Direction direction in Enum.GetValues(typeof(Direction)))
				{
					if (neighbourDirections.Contains(direction) == false)
					{
						var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
						if (freeSpaces.ContainsKey(newPosition))
						{
							continue;
						}
						freeSpaces.Add(newPosition, Direction.Right);
					}
				}
			}
			return freeSpaces;
		}
	}
}

