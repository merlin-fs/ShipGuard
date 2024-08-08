
using Unity.Mathematics;

namespace UnityEngine
{
    public static class TextureCalculator
    {
        public static float ConvertTextureScaleToGame(this SpriteRenderer sr, float radius)
        {
            var sprite = sr.sprite;
            var divideValue = sprite.texture.width / sprite.pixelsPerUnit;
            return radius / divideValue;
        }

        public static float2 ConvertTextureScaleGame(this SpriteRenderer sr, float2 size)
        {
            var sprite = sr.sprite;
            var textureSize = new float2(sprite.texture.width, sprite.texture.height) / sprite.pixelsPerUnit;
            return new float2(size.x / textureSize.x, size.y / textureSize.y);
        }
    }
}
