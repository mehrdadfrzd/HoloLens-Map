using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Helpers;
using UnityEngine;

namespace Assets.Models.Factories
{
    public class BuildingFactory : MonoBehaviour
    {
        [SerializeField] private Building.Settings _settings;
        //using building center for key is quite wrong actually, but works for now
        Dictionary<Vector3, Building> BuildingDictionary { get; set; }
        private HashSet<Vector3> _buildingList { get; set; }

        public void Start()
        {
            BuildingDictionary = new Dictionary<Vector3, Building>();
            _buildingList = new HashSet<Vector3>();
        }

        public void CreateBuilding(Vector2 tileMercPos, JSONObject geo, Transform parent = null)
        {
            parent = parent ?? transform;
            var buildingCorners = new List<Vector3>();
            //foreach (var bb in geo["geometry"]["coordinates"].list)
            //{
            var bb = geo["geometry"]["coordinates"].list[0]; //this is wrong but cant fix it now
            for (int i = 0; i < bb.list.Count - 1; i++)
            {
                var c = bb.list[i];
                var dotMerc = GM.LatLonToMeters(c[1].f, c[0].f);
                var localMercPos = new Vector2(dotMerc.x - tileMercPos.x, dotMerc.y - tileMercPos.y);
                buildingCorners.Add(localMercPos.ToVector3xz());
            }

            //prevents duplicates coming from different tiles
            //it sucks yea but works for now, cant use propery/id in json for some reason
            var uniqPos = parent.transform.position + buildingCorners[0];
            if (_buildingList.Contains(uniqPos))
            {
                return;
            }
            _buildingList.Add(uniqPos);

            try
            {
                var buildingCenter = buildingCorners.Aggregate((acc, cur) => acc + cur) / buildingCorners.Count;
                if (!BuildingDictionary.ContainsKey(buildingCenter))
                {
                    for (int i = 0; i < buildingCorners.Count; i++)
                    {
                        //using corner position relative to building center
                        buildingCorners[i] = buildingCorners[i] - buildingCenter;
                    }
                    var building = new GameObject().AddComponent<Building>();
                    var kind = "";
                    if (geo["properties"].HasField("landuse_kind"))
                        kind = geo["properties"]["landuse_kind"].str;
                    if (geo["properties"].HasField("name"))
                        building.Name = geo["properties"]["name"].str;

                    building.Init(buildingCenter, buildingCorners, kind, _settings);
                    BuildingDictionary.Add(buildingCenter, building);

                    building.name = "building";
                    building.transform.parent = parent;
                    building.transform.localPosition = buildingCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            //}
        }

    }
}
