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

namespace Assets
{
    public class TileManager : MonoBehaviour
    {
        private BuildingFactory _buildingFactory;
        private RoadFactory _roadFactory;
        private Transform _tileHost;

        private bool _loadImages;
        private int _zoom = 16; //detail level of TMS system
        private string _key = "vector-tiles-5sBcqh6"; //try getting your own key if this doesn't work
        private Dictionary<Vector2, Tile> _tiles; //will use this later on
        private Vector2 _centerTms; //tms tile coordinate
        private Vector2 _centerInMercator; //this is like distance (meters) in mercator 


        public void Init(BuildingFactory buildingFactory, RoadFactory roadFactory, World.Settings settings)
        {
            var v2 = GM.LatLonToMeters(settings.Lat, settings.Long);
            var tile = GM.MetersToTile(v2, settings.DetailLevel);

            _tileHost = new GameObject("Tiles").transform;
            _tileHost.SetParent(transform, false);

            _buildingFactory = buildingFactory;
            _roadFactory = roadFactory;
            _tiles = new Dictionary<Vector2, Tile>();
            _centerTms = tile;
            _centerInMercator = GM.TileBounds(_centerTms, _zoom).center;
            _zoom = settings.DetailLevel;
            _loadImages = settings.LoadImages;

            for (int i = -settings.Range; i <= settings.Range; i++)
            {
                for (int j = -settings.Range; j <= settings.Range; j++)
                {
                    var v = new Vector2(_centerTms.x + i, _centerTms.y + j);
                    var centerInMercator = GM.TileBounds(_centerTms, _zoom).center;
                    CoroutineStarter.StartCoroutine(CreateTile(v, centerInMercator));
                }
            }
        }

        private IEnumerator CreateTile(Vector2 tileTms, Vector2 centerInMercator)
        {
            var rect = GM.TileBounds(tileTms, _zoom);
            var tile = new GameObject("tile " + tileTms.x + "-" + tileTms.y).AddComponent<Tile>();
            _tiles.Add(tileTms, tile);
            tile.transform.SetParent(_tileHost, true);
            tile.CreateTile(_buildingFactory, _roadFactory, 
                new Tile.Settings()
                {
                    Zoom = _zoom,
                    TileTms = tileTms,
                    TileCenter = rect.center,
                    LoadImages = _loadImages
                });
            tile.transform.position = (rect.center - centerInMercator).ToVector3xz();
            yield return null;
        }
    }
}
