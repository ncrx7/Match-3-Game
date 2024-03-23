using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils.Extensions
{
    public static class LineRendererExtensions
    {
        /// <summary>
        /// EN : Draws a reflected line to LineRenderer.
        /// TR : LineRenderer'a yansıyan çizgi çizdirir.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="direction"></param>
        /// <param name="layer"></param>
        /// <param name="reflectionCount"></param>
        /// <param name="reflectionDistance"></param>
        public static void DrawReflectedAimLine(this LineRenderer renderer, Vector3 direction, LayerMask layer,
            int reflectionCount, float reflectionDistance = 50)
        {
            List<Vector3> positions = new List<Vector3> { Vector3.zero };
            
            var ray = new Ray(renderer.transform.position, direction);

            for (int i = 0; i < reflectionCount; i++)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layer))
                {
                    // Çarpışma noktasını bul
                    Vector3 hitPoint = hit.point;

                    // Çarpışma noktasına yeni bir çizgi çek
                    positions.Add(hitPoint - renderer.transform.position);

                    // Yansıma yap
                    ray = new Ray(hitPoint, Vector3.Reflect(ray.direction, hit.normal));
                }
                else
                {
                    // Çarpışma yoksa, çizgiyi hedefe kadar çek
                    positions.Add(ray.direction * reflectionDistance);
                    break;
                }
            }

            renderer.positionCount = positions.Count;
            for (int i = 0; i < positions.Count; i++)
                renderer.SetPosition(i, positions[i]);
        }
    }
}