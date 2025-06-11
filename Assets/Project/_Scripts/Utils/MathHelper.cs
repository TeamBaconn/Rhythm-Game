public static class MathHelper
{ 
    public static float SafeDivide(float a, float b)
    {
        return b == 0f ? 0f : a / b;
    }
}