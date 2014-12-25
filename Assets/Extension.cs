

namespace ExtensionMethod
{
    using UnityEngine;
    using System.Collections;
    public static class Vector3Extension
    {
        public static Vector2 ProjectPlane(this Vector3 v_)
        {
            return new Vector2(v_.x, v_.y);
        }
    }


    public static class Vector2Extension
    {
        public static float Determinant(this Vector2 a_, Vector2 b_)
        {
            return a_.x * b_.y - a_.y * b_.x;
        }

        public static Vector3 Embed(this Vector2 v_)
        {
            return new Vector3(v_.x, v_.y, 0f);
        }

        public static Vector2 Rotate90CCW(this Vector2 v_)
        {
            return new Vector2(v_.y, -v_.x);
        }

        public static float Dot(this Vector2 a_, Vector2 b_)
        {
            return a_.x * b_.x + a_.y * b_.y;
        }

        public static Vector2 Copy(this Vector2 v_)
        {
            return new Vector2(v_.x, v_.y);
        }
    }
}