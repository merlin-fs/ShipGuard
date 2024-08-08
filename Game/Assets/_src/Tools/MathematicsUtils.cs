using System;
using System.Runtime.CompilerServices;

namespace Unity.Mathematics
{
    public static partial class utils
    {
        public static bool SpheresIntersect(float3 sphere1, float radius1, float3 sphere2, float radius2, out float3 ip, float threshold = 0.1f)
        {
            // vector from sphere 1 -> sphere 2
            float3 ab = sphere1 - sphere2;

            // Calculate radius from Unity built-in sphere.
            // Unity spheres are unit spheres (diameter = 1)
            // So diameter = scale, thus radius = scale / 2.0f.
            // **Presumes uniform scaling.

            // When spheres are too close or too far apart, ignore intersection.
            var magnitude = ab.magnitude();

            float diff = radius1 + radius2 - magnitude;
            if (diff < threshold)
            {
                ip = float3.zero;
                return false;
            }
            // Intersection is the distance along the vector between
            // the 2 spheres as a function of the sphere's radius.
            ip = sphere1 + ab * radius1 / magnitude;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float magnitude(this float3 self)
        {
            return (float)math.sqrt(self.x * self.x + self.y * self.y + self.z * self.z);
        }

        public static float magnitude(this int2 self)
        {
            return (float)math.sqrt(self.x * self.x + self.y * self.y);
        }

        public static quaternion ClampRotation(this quaternion q, float3 bounds)
        {
            q = q.norm();
            float angleX = 2.0f * math.degrees(math.atan(q.value.x));
            angleX = math.clamp(angleX, -bounds.x, bounds.x);
            q.value.x = math.tan(0.5f * math.radians(angleX));

            float angleY = 2.0f * math.degrees(math.atan(q.value.y));
            angleY = math.clamp(angleY, -bounds.y, bounds.y);
            q.value.y = math.tan(0.5f * math.radians(angleY));

            float angleZ = 2.0f * math.degrees(math.atan(q.value.z));
            angleZ = math.clamp(angleZ, -bounds.z, bounds.z);
            q.value.z = math.tan(0.5f * math.radians(angleZ));

            return math.normalize(q);
        }

        private static quaternion norm(this quaternion q)
        {
            q.value.x /= q.value.w;
            q.value.y /= q.value.w;
            q.value.z /= q.value.w;
            q.value.w = 1.0f;
            return q;
        }

        public unsafe static quaternion ClampRotationX(this quaternion q, float min, float max, bool* isClamp = null)
        {
            q = q.norm();
            float input = 2.0f * math.degrees(math.atan(q.value.x));
            float angleX = math.clamp(input, min, max);
            if (isClamp != null)
                *isClamp = input != angleX;
            q.value.x = math.tan(0.5f * math.radians(angleX));
            return math.normalize(q);
        }

        public unsafe static quaternion ClampRotationY(this quaternion q, float min, float max, bool* isClamp = null)
        {
            q = q.norm();
            float input = 2.0f * math.degrees(math.atan(q.value.y));
            float angleY = math.clamp(input, min, max);
            if (isClamp != null)
                *isClamp = input != angleY;
            q.value.y = math.tan(0.5f * math.radians(angleY));
            return math.normalize(q);
        }

        public unsafe static quaternion ClampRotationZ(this quaternion q, float min, float max, bool* isClamp = null)
        {
            q = q.norm();
            float input = 2.0f * math.degrees(math.atan(q.value.z));
            float angleZ = math.clamp(input, min, max);
            if (isClamp != null)
                *isClamp = input != angleZ;
            q.value.z = math.tan(0.5f * math.radians(angleZ));
            return math.normalize(q);
        }
    }
}
