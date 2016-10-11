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
        public List<GameObject> spawnPoints = new List<GameObject>();
        public GameObject unitHolder;

        private int controllGroup = 0;
        private List<List<Seeker>> selectedUnits = new List<List<Seeker>>();

        private bool gameHasStarted = false;

        private float spawnTimer = 0;
        private float spawnInterval = 10.0f;
        private float tileTimer = 0f;
        private float tileInterval = 0.5f;

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

            for (int i = 0; i < 1; i++)
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
            Debug.Log(go.transform.name);
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
            
            go.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;

            selectedUnits[0].Add(go.GetComponent<Seeker>());
            newEnemy = !newEnemy;
            if (newEnemy)
                selectedUnits[1].Add(go.GetComponent<Seeker>());
            else
                selectedUnits[2].Add(go.GetComponent<Seeker>());


        }

        private void Inputs()
        {
            if (spawnTimer < spawnInterval)
                spawnTimer += Time.deltaTime; //Skal egentlig være += her

            else
            {
                Spawn();
                spawnTimer = 0;
            }
            if (tileTimer < tileInterval)
                tileTimer += Time.deltaTime;
            else
            {
                tileTimer = 0;
                Tile tile = pathfinder.worldData.tileManager.GetTileFromPosition(pathfinder.GetMousePosition());
                if (tile != null)
                {
                    pathfinder.FindPath(tile, selectedUnits[controllGroup]);
                }
            }
        }
    }
}
