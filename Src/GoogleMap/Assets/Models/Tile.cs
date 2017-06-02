using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.Factories;
using UnityEngine;
using UniRx;

namespace Assets
{
    public class Tile :MonoBehaviour
    {
        private BuildingFactory _buildingFactory;
        private RoadFactory _roadFactory;

        private Settings _settings;
        private Rect _rect;

        public void CreateTile(BuildingFactory buildingFactory, RoadFactory roadFactory, Settings settings)
        {
            _buildingFactory = buildingFactory;
            _roadFactory = roadFactory;
            _settings = settings;
            
            _rect = GM.TileBounds(_settings.TileTms, _settings.Zoom);
            Load(settings.Key, _settings.Zoom, _settings.TileTms);
        }

        private void Load(string key, int zoom, Vector2 tileTms)
        {
            var template = "https://vector.mapzen.com/osm/{0}/{1}/{2}/{3}.{4}?api_key={5}";
            var layers = "buildings,roads";
            var format = "json";

            var url = string.Format(template, layers, zoom, tileTms.x, tileTms.y, format, key);
           
            ObservableWWW.Get(url)
                .Subscribe(
                    osmJson =>
                    {
                        StartCoroutine(ConstructTile(_settings.TileTms, _settings.TileCenter, osmJson));
                    }, //success
                    exp => Debug.Log("Error fetching -> " + url)); //failure
        }

        private IEnumerator ConstructTile(Vector2 tilePos, Vector2 mercPos, string text)
        {
            JSONObject mapData;
            string url;
            mapData = new JSONObject(text);

            
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = "map";
            go.transform.localScale = new Vector3(_rect.width/10, 1, _rect.width/10); 
            go.transform.rotation = Quaternion.AngleAxis(180, new Vector3(0, 1, 0));
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localPosition -= new Vector3(0, 1, 0);
            go.GetComponent<Renderer>().material.mainTexture = new Texture2D(512, 512, TextureFormat.DXT5, false);
            go.GetComponent<Renderer>().material.color = new Color(.1f, .1f, .1f, 1f);

            if (_settings.LoadImages)
            {
                go.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
                url = "http://b.tile.openstreetmap.org/" + _settings.Zoom + "/" + tilePos.x + "/" + tilePos.y + ".png";
                ObservableWWW.GetWWW(url).Subscribe(x =>
                {
                    x.LoadImageIntoTexture((Texture2D)go.GetComponent<Renderer>().material.mainTexture);
                });
            }
            yield return null;

            StartCoroutine(CreateBuildings(mapData["buildings"], mercPos));
            StartCoroutine(CreateRoads(mapData["roads"], mercPos));

        }

        private IEnumerator CreateBuildings(JSONObject mapData, Vector2 tileMercPos)
        {
            foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon"))
            {
                _buildingFactory.CreateBuilding(tileMercPos, geo, transform);
                yield return null;
            }
        }

        private IEnumerator CreateRoads(JSONObject mapData, Vector2 tileMercPos)
        {
            for (int index = 0; index < mapData["features"].list.Count; index++)
            {
                var geo = mapData["features"].list[index];
                _roadFactory.CreateRoad(tileMercPos, geo, index, transform);
                yield return null;
            }
        }

        public class Settings
        {
            public string Key = "vector-tiles-5sBcqh6";
            public int Zoom { get; set; }
            public Vector2 TileTms { get; set; }
            public Vector3 TileCenter { get; set; }
            public bool LoadImages { get; set; }
        }
    }
}
