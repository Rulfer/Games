using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FlowPathfinding
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager manager;
        private Dictionary<Tile, GameObject> obstacles = new Dictionary<Tile, GameObject>();

        public Pathfinder pathfinder;
        public GameObject unitHolder;

        private int controllGroup = 0;
        private List<List<Seeker>> selectedUnits = new List<List<Seeker>>();

        private bool gameHasStarted = false;

        private float timer = 0;
        private float rescanTimer = 1f;

        public GameObject enemy;
        bool newEnemy = true;

        void Start()
        {
            manager = this;
            for (int i = 0; i < 3; i++)
                selectedUnits.Add(new List<Seeker>());

            bool aSwitch = true;
            foreach (Transform child in unitHolder.transform)
            {
                selectedUnits[0].Add(child.GetComponent<Seeker>());

                aSwitch = !aSwitch;
                if(aSwitch)
                    selectedUnits[1].Add(child.GetComponent<Seeker>());
                else
                    selectedUnits[2].Add(child.GetComponent<Seeker>());
            }

            for (int i = 0; i < 2; i++)
            {
                Spawn();
            }
        }

        void Update()
        {
            if (gameHasStarted)
                Inputs();
            else
                gameHasStarted = true;
        }

        private void ResetUnits()
        {
            for (int i = 0; i < 3; i++)
                selectedUnits.Add(new List<Seeker>());

            bool aSwitch = true;
            foreach (Transform child in unitHolder.transform)
            {
                selectedUnits[0].Add(child.GetComponent<Seeker>());

                aSwitch = !aSwitch;
                if(aSwitch)
                    selectedUnits[1].Add(child.GetComponent<Seeker>());
                else
                    selectedUnits[2].Add(child.GetComponent<Seeker>());
            }
        }

        public void DeleteEnemy(GameObject go)
        {
            selectedUnits[0].Remove(go.GetComponent<Seeker>());
            if (newEnemy)
                selectedUnits[1].Remove(go.GetComponent<Seeker>());
            else
                selectedUnits[2].Remove(go.GetComponent<Seeker>());
            go.transform.parent = null;
            go.GetComponent<Unit>().enabled = false;
            go.GetComponent<CharacterController>().enabled = false;
            go.GetComponentInChildren<PlayAnimation>().DieAnimation();
            SeekerMovementManager.move.allSeekers.Remove(go.GetComponent<Seeker>());
            //Destroy(go);
            ResetUnits();
        }

        private void Spawn()
        {
            GameObject go = Instantiate(enemy);
            go.transform.parent = unitHolder.transform;
            go.transform.name = "Unit";
            go.transform.position = unitHolder.transform.position;

            selectedUnits[0].Add(go.GetComponent<Seeker>());
            newEnemy = !newEnemy;
            if (newEnemy)
                selectedUnits[1].Add(go.GetComponent<Seeker>());
            else
                selectedUnits[2].Add(go.GetComponent<Seeker>());

//            foreach (Transform child in unitHolder.transform)
//            {
//                selectedUnits[0].Add(child.GetComponent<Seeker>());
//
//                aSwitch = !aSwitch;
//                if(aSwitch)
//                    selectedUnits[1].Add(child.GetComponent<Seeker>());
//                else
//                    selectedUnits[2].Add(child.GetComponent<Seeker>());
//            }

        }

        private void Inputs()
        {
            if (timer < rescanTimer)
                timer += Time.deltaTime; //Skal egentlig være += her

            else
            {
                //Spawn();
                timer = 0;
                Tile tile = pathfinder.worldData.tileManager.GetTileFromPosition(pathfinder.GetMousePosition());
                Debug.Log(tile);
                if (tile != null)
                {
                    pathfinder.FindPath(tile, selectedUnits[controllGroup]);

                    if (Input.GetMouseButton(1) && Input.GetKey("b"))
                    {
                        if (!tile.blocked)
                        {
                            GameObject blockade = Resources.Load("Prefab/Obstacle") as GameObject;
                            GameObject b = Instantiate(blockade, pathfinder.worldData.tileManager.GetTileWorldPosition(tile, pathfinder.worldData.worldAreas[tile.worldAreaIndex]) + new Vector3(0, 0.1f, 0), Quaternion.identity) as GameObject;
                            b.transform.parent = transform;
                            obstacles.Add(tile, b);
                        }

                        pathfinder.worldData.worldManager.BlockTile(tile);
                    }


                    if (Input.GetMouseButton(1) && Input.GetKey("n"))
                    {
                        if (tile.blocked)
                        {
                            Destroy(obstacles[tile]);
                            obstacles.Remove(tile);
                        }

                        pathfinder.worldData.worldManager.UnBlockTile(tile);
                    }

                    if (Input.GetMouseButton(1) && Input.GetKey("c"))
                        pathfinder.worldData.worldManager.SetTileCost(tile, 10);


                    if (Input.GetKeyDown("0"))
                        controllGroup = 0;

                    if (Input.GetKeyDown("1"))
                        controllGroup = 1;

                    if (Input.GetKeyDown("2"))
                        controllGroup = 2;
                }

            }
        }
    }
}
