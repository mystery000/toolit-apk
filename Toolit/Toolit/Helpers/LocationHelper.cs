using System.Collections.Generic;
using System.Text.Json;
using GeoJSON.Net;
using Xamarin.Essentials;

namespace Toolit.Helpers
{
    public class LocationHelper
    {
        public class FeatureCollection
        {
            public GeoJSONObjectType Type => GeoJSONObjectType.FeatureCollection;
            public GeometryFeature[] Features { get; set; }
        }
        
        public class GeometryFeature
        {
            public GeoJSONObjectType Type => GeoJSONObjectType.Feature;
            public object Properties => new object();
            public GeometryPoint Geometry { get; set; }
        }
        
        public class GeometryPoint
        {
            public GeoJSONObjectType Type => GeoJSONObjectType.Point;
            public double[] Coordinates { get; set; }
        }
        
        public static GeometryPoint BuildGeometryCollection(Location location)
        {
            if (location == null)
            {
                return null;
            }

            return new GeometryPoint()
            {
                Coordinates = new[] {location.Longitude, location.Latitude}
            };
        }

        public static string TestJson(FeatureCollection collection)
        {
            return JsonSerializer.Serialize(collection);
        }
    }
}