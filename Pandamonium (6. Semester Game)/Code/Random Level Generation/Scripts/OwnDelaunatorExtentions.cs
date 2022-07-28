using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using System.Linq;
using System;


namespace DelaunatorSharp.Unity.Extensions
{
    public static class OwnDelaunatorExtensions
    {
        public static IPoint[] OwnToPoints(this IEnumerable<Vector2> vertices) => vertices.Select(vertex => new Point(vertex.x, vertex.y)).OfType<IPoint>().ToArray();
        public static IPoint[] OwnToPoints(this Transform[] vertices) => vertices.Select(x => x.transform.position).OfType<Vector2>().OwnToPoints();

        public static Vector2[] OwnToVectors2(this IEnumerable<IPoint> points) => points.Select(point => point.OwnToVector2()).ToArray();
        public static Vector3[] OwnToVectors3(this IEnumerable<IPoint> points) => points.Select(point => point.OwnToVector3()).ToArray();

        public static Vector2 OwnToVector2(this IPoint point) => new Vector2((float)point.X, (float)point.Y);
        public static Vector3 OwnToVector3(this IPoint point) => new Vector3((float)point.X, 0f, (float)point.Y);
    }
}